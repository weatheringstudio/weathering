
using System;
using System.Collections.Generic;

namespace Weathering
{
    public interface IValues
    {
        IValue Create(Type type);
        IValue GetOrCreate(Type type);
        IValue Get(Type type);
        bool Has(Type type);
        bool Remove(Type type);

        IValue Create<T>();
        IValue GetOrCreate<T>();
        IValue Get<T>();
        bool Has<T>();
        bool Remove<T>();

        Dictionary<Type, IValue> Dict { get; }
    }

    public class Values : IValues
    {
        private Values() { }

        public Dictionary<Type, IValue> Dict { get; private set; } = null;

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
            IValues result = GetOne();
            foreach (var pair in data) {
                Type type = Type.GetType(pair.Key);
                IValue value = Value.FromData(pair.Value);
                result.Dict.Add(type, value);
            }
            return result;
        }


        public static IValues GetOne() {
            return new Values {
                Dict = new Dictionary<Type, IValue>()
            };
        }

        public IValue Get(Type type) {
            if (Dict.TryGetValue(type, out IValue value)) {
                return value;
            } else {
                throw new Exception(type.Name);
            }
        }
        public IValue Get<T>() {
            return Get(typeof(T));
        }

        public IValue Create(Type type) {
            if (Dict.TryGetValue(type, out IValue value)) {
                throw new Exception();
            } else {
                value = Value.Create(0, 0, 0, 0, 0, TimeUtility.GetTicks());
                Dict.Add(type, value);
                return value;
            }
        }
        public IValue Create<T>() {
            return Create(typeof(T));
        }

        public IValue GetOrCreate(Type type) {
            if (Dict.TryGetValue(type, out IValue value)) {
                return value;
            } else {
                value = Value.Create(0, 0, 0, 0, 0, TimeUtility.GetTicks());
                Dict.Add(type, value);
                return value;
            }
        }
        public IValue GetOrCreate<T>() {
            return GetOrCreate(typeof(T));
        }

        public bool Has(Type type) {
            if (Dict.TryGetValue(type, out IValue value)) {
                return true;
            } else {
                return false;
            }
        }
        public bool Has<T>() {
            return Has(typeof(T));
        }

        public bool Remove(Type type) {
            if (Dict.ContainsKey(type)) {
                Dict.Remove(type);
                return true;
            }
            return false;
        }

        public bool Remove<T>() {
            return Remove(typeof(T));
        }
    }
}

