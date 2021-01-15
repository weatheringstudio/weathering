
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IRefs
    {
        IRef Get<T>();
        bool Has<T>();
        Dictionary<Type, IRef> Dict { get; set; }
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
            IRefs result = Create();
            foreach (var pair in data) {
                Type type = Type.GetType(pair.Key);
                IRef value = Ref.FromData(pair.Value);
                result.Dict.Add(type, value);
            }
            return result;
        }

        public static IRefs Create() {
            return new Refs {
                Dict = new Dictionary<Type, IRef>()
            };
        }

        public IRef Get<T>() {
            Type type = typeof(T);
            if (Dict.TryGetValue(type, out IRef value)) {
                return value;
            } else {
                value = Ref.Create(null, 0, 0);
                Dict.Add(type, value);
                return value;
            }
        }
        public bool Has<T>() {
            Type type = typeof(T);
            if (Dict.TryGetValue(type, out IRef value)) {
                return true;
            } else {
                return false;
            }
        }
    }
}

