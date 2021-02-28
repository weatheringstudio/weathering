
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IRefs
    {
        IRef Create<T>();
        IRef Create(Type type);
        IRef Get<T>();
        IRef Get(Type type);
        IRef GetOrCreate<T>();
        IRef GetOrCreate(Type type);
        void Remove<T>();
        void Remove(Type type);

        bool Has(Type type);
        bool Has<T>();
        Dictionary<Type, IRef> Dict { get; }
    }

    public class Refs : IRefs
    {
        private Refs() { }
        public Dictionary<Type, IRef> Dict { get; set; } = null;

        public static Dictionary<string, RefData> ToData(IRefs refs) {
            if (refs == null) return null;
            Dictionary<string, RefData> data = new Dictionary<string, RefData>();
            foreach (var pair in refs.Dict) {
                data.Add(pair.Key.FullName, Ref.ToData(pair.Value));
            }
            return data;
        }

        public static IRefs FromData(Dictionary<string, RefData> data) {
            if (data == null) return null;
            IRefs result = GetOne();
            foreach (var pair in data) {
                Type type = Type.GetType(pair.Key);
                IRef value = Ref.FromData(pair.Value);
                result.Dict.Add(type, value);
            }
            return result;
        }

        public static IRefs GetOne() {
            return new Refs {
                Dict = new Dictionary<Type, IRef>()
            };
        }

        public IRef Get<T>() {
            return Get(typeof(T));
        }
        public IRef Get(Type type) {
            if (Dict.TryGetValue(type, out IRef value)) {
                return value;
            } else {
                throw new Exception(type.Name);
            }
        }

        public IRef Create<T>() {
            return Create(typeof(T));
        }
        public IRef Create(Type type) {
            if (Dict.TryGetValue(type, out IRef value)) {
                throw new Exception("已有：" + type.FullName);
            } else {
                value = Ref.Create(null, null, 0, 0, null, null, 0, 0);
                Dict.Add(type, value);
                return value;
            }
        }

        public IRef GetOrCreate<T>() {
            return GetOrCreate(typeof(T));
        }
        public IRef GetOrCreate(Type type) {
            if (Dict.TryGetValue(type, out IRef value)) {
                return value;
            } else {
                value = Ref.Create(null, null, 0, 0, null, null, 0, 0);
                Dict.Add(type, value);
                return value;
            }
        }


        public bool Has(Type type) {
            if (Dict.TryGetValue(type, out IRef value)) {
                return true;
            } else {
                return false;
            }
        }
        public bool Has<T>() {
            return Has(typeof(T));
        }


        public void Remove<T>() {
            Remove(typeof(T));
        }
        public void Remove(Type type) {
            if (Dict.ContainsKey(type)) {
                Dict.Remove(type);
                return;
            }
            throw new Exception(type.FullName);
        }
    }
}

