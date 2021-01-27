
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IValue
    {
        long Val { get; set; } // 序列化
        long Max { get; set; } // 序列化
        long Inc { get; set; } // 序列化
        long Dec { get; set; }
        long Del { get; set; } // 序列化

        long Sur { get; }

        long Time { get; set; }
        bool Maxed { get; }
        bool IsMaxed();

        string RemainingTimeString { get; }
        long ProgressedTicks { get; }
    }


    // class for serialization, struct for memory allocation
    public class ValueData
    {
        public long time = 0;
        public long inc = 0;
        public long dec = 0;
        public long del = 0;
        public long val = 0;
        public long max = 0;
    }

    public class Value : IValue
    {
        private Value() { }

        public static ValueData ToData(IValue value) {
            Value v = value as Value;
            if (v == null) throw new Exception();
            return new ValueData {
                time = v.time,
                inc = v.inc,
                dec = v.dec,
                del = v.del,
                val = v.val,
                max = v.max,
            };
        }
        public static IValue FromData(ValueData data) {
            return Create(data.val, data.max, data.inc, data.dec, data.del, data.time);
        }

        public const long MiniSecond = 10000;
        public const long Second = 1000 * MiniSecond;
        public const long Minute = 60 * Second;
        public const long Hour = 60 * Minute;
        public const long Day = 24 * Hour;

        private long time = 0;
        private long val = 0;
        private long inc = 0; // val difference
        private long dec = 0; // val difference
        private long del = 1; // time diffence
        private long max = 0; // val limit


        public static Value Create(long val, long max, long inc, long dec, long del, long time) {
            return new Value {
                time = time,
                val = val,
                inc = inc,
                dec = dec,
                del = del,
                max = max
            };
        }

        private void Synchronize() {
            if (del == 0 || (inc - dec) == 0) return;
            long now = TimeUtility.GetTicks();
            long times = (now - time) / del;
            long newVal = val + times * (inc - dec);
            val = newVal > max ? max : newVal;
            time += times * del;
        }

        public long Time { get => time; set => time = value; }

        public long Max {
            get => max;
            set {
                Synchronize();
                if (Maxed) {
                    time = TimeUtility.GetTicks();
                }
                max = value;
            }
        }
        public long Del {
            get => del;
            set {
                Synchronize();
                del = value;
            }
        }

        public long Inc {
            get => inc;
            set {
                Synchronize();
                inc = value;
            }
        }

        public long Dec {
            get => dec;
            set {
                Synchronize();
                dec = value;
            }
        }

        public long Sur {
            get => inc - dec;
        }

        public long Val {
            get {
                long now = TimeUtility.GetTicks();
                long times = del == 0 ? 0 : (now - time) / del;
                long newVal = val + times * Sur;
                return newVal > max ? max : newVal;
            }
            set {
                Synchronize();
                if (value > max) {
                    val = max;
                    time = TimeUtility.GetTicks();
                } else {
                    val = value;
                }
            }
        }

        public long ProgressedTicks {
            get {
                long now = TimeUtility.GetTicks();
                long progressedTicks = del == 0 || Sur == 0 ? 0 : (now - time) % del;
                return progressedTicks;
            }
        }

        public string RemainingTimeString {
            get {
                if (del == 0 || Sur == 0) return "生产停止";
                long remainingTicks = del - ProgressedTicks;

                const long ms2tick = 10000;
                const long s2ms = 1000;
                const long min2s = 60;
                const long h2min = 60;

                long miniSeconds = remainingTicks / ms2tick;
                long seconds = miniSeconds / s2ms;
                long minutes = seconds / min2s;
                long hours = minutes / h2min;

                if (hours > 0) {
                    return $"{hours} 时 {minutes - hours * h2min} 分";
                } else if (minutes > 0) {
                    return $"{minutes} 分 {seconds - minutes * min2s} 秒";
                } else if (seconds > 0) {
                    return $"{seconds} 秒 ";
                } else if (miniSeconds > 0) {
                    return $"{miniSeconds} 毫秒";
                }

                return "< 1ms";
            }
        }

        public bool Maxed => Val >= Max;
        public bool IsMaxed() => Maxed;

    }
}

