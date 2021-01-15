
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
        public long Time = 0;
        public long Inc = 0;
        public long Dec = 0;
        public long Del = 0;
        public long Val = 0;
        public long Max = 0;
    }

    public class Value : IValue
    {
        private Value() { }

        public static ValueData ToData(IValue value) {
            Value v = value as Value;
            if (v == null) throw new Exception();
            return new ValueData {
                Time = v.time,
                Inc = v.inc,
                Dec = v.dec,
                Del = v.del,
                Val = v.val,
                Max = v.max,
            };
        }
        public static IValue FromData(ValueData data) {
            return Create(data.Val, data.Max, data.Inc, data.Dec, data.Del, data.Time);
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
            long now = Utility.GetTicks();
            long times = (now - time) / del;
            long newVal = val + times * (inc -dec);
            val = newVal > max ? max : newVal;
            time += times * del;
        }

        public long Time { get => time; set => time = value; }

        public long Max {
            get => max;
            set {
                Synchronize();
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
                long now = Utility.GetTicks();
                long times = del == 0 ? 0 : (now - time) / del;
                long newVal = val + times * Sur;
                return newVal > max ? max : newVal;
            }
            set {
                Synchronize();
                val = value > max ? max : value;
            }
        }

        public long ProgressedTicks {
            get {
                long now = Utility.GetTicks();
                long progressedTicks = del == 0 || Sur == 0 ? 0 : (now - time) % del;
                return progressedTicks;
            }
        }

        public string RemainingTimeString {
            get {
                if (del == 0 || Sur == 0) return "生产停止";
                long remainingTicks = del - ProgressedTicks;

                long minutes = (remainingTicks / (10000 * 1000 * 60));
                if (minutes > 0) {
                    return (1 + minutes) + "min";
                }
                long seconds = (remainingTicks / (10000 * 1000));
                if (seconds > 0) {
                    return (1 + seconds) + "s";
                }
                long miniseconds = (remainingTicks / (10000));
                if (miniseconds > 0) {
                    return (1 + miniseconds) + "ms";
                }

                return "< 1ms";
            }
        }

        public bool Maxed => Val >= Max;
        public bool IsMaxed() => Maxed;

    }
}

