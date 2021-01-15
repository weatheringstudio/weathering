
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IValues
    {
        IValue Get<T>();
        bool Has<T>();
        Dictionary<Type, IValue> Dict { get; }
    }

    public class Values : IValues
    {
        private Values() { }

        public Dictionary<Type, IValue> Dict { get; private set; }
        public static Dictionary<string, ValueData> ToData(IValues values) {
            if (values == null) return null;
            Dictionary<string, ValueData> dict = new Dictionary<string, ValueData>();
            foreach (var pair in values.Dict) {
                dict.Add(pair.Key.FullName, Value.ToData(pair.Value));
            }
            return dict;
        }
        public static IValues FromData(Dictionary<string, ValueData> data) {
            if (data == null) return null;
            IValues result = Create();
            foreach (var pair in data) {
                Type type = Type.GetType(pair.Key);

                IValue value = Value.FromData(pair.Value);
                result.Dict.Add(type, value);
            }
            return result;
        }


        public static IValues Create() {
            return new Values {
                Dict = new Dictionary<Type, IValue>()
            };
        }

        public IValue Get<T>() {
            Type type = typeof(T);
            if (Dict.TryGetValue(type, out IValue value)) {
                return value;
            } else {
                value = Value.Create(0, 0, 0, 0, 0, Utility.GetTicks());
                Dict.Add(type, value);
                return value;
            }
        }
        public bool Has<T>() {
            Type type = typeof(T);
            if (Dict.TryGetValue(type, out IValue value)) {
                return true;
            } else {
                return false;
            }
        }
    }

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

    public struct ValueData
    {
        public long Time;
        public long Inc;
        public long Dec;
        public long Del;
        public long Val;
        public long Max;
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
                long newVal = val + times * (inc - dec);
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
                long remainingTicks = del == 0 || (inc - dec) == 0 ? 0 : (now - time) % del;
                return remainingTicks;
            }
        }

        public string RemainingTimeString {
            get {
                if (del == 0 || (inc - dec) == 0) return "生产停止";
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

