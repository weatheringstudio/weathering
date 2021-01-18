
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IInventory
    {
        long Get<T>();
        void Set<T>(long val);
        bool Add<Type>(long val);
        bool CanAdd<Type>(long val);

        int TypeCapacity { get; set; }
        long Quantity { get; }
        long QuantityCapacity { get; set; }
        Dictionary<Type, long> Dict { get; }

    }

    public class Inventory : IInventory
    {

        public int TypeCapacity { get; set; }

        public long Quantity { get; private set; }
        public long QuantityCapacity { get; set; }

        private Inventory() { }

        public Dictionary<Type, long> Dict { get; private set; } = null;


        public bool Add<T>(long val) {
            Type type = typeof(T);
            if (!CanAdd<T>(val)) {
                return false;
            }
            if (Dict.ContainsKey(type)) {
                Dict[type] += val;
            }
            else {
                Dict.Add(type, val);
            }
            Quantity += val;
            return true;
        }
        public bool CanAdd<T>(long val) {
            if (val > QuantityCapacity-Quantity) {
                return false;
            }
            if (Dict.Count < TypeCapacity) {
                return true;
            }
            else if (Dict.Count == TypeCapacity) {
                return Dict.ContainsKey(typeof(T));
            }
            return false;
        }
        public long Get<T>() {
            Type type = typeof(T);
            if (Dict.TryGetValue(type, out long value)) {
                return value;
            } else {
                return 0;
            }
        }
        public void Set<T>(long val) {
            Type type = typeof(T);
            if (Dict.ContainsKey(type)) {
                if (val == 0) {
                    Quantity -= Dict[type];
                    Dict.Remove(type);
                } else {
                    Dict[type] = val;
                }
            } else {
                Dict.Add(type, val);
            }
            Quantity += val;
        }

        public static Dictionary<string, long> ToData(IInventory inventory) {
            if (inventory == null) return null;
            Dictionary<string, long> result = new Dictionary<string, long>();
            foreach (var pair in inventory.Dict) {
                result.Add(pair.Key.FullName, pair.Value);
            }
            return result;
        }

        public static IInventory FromData(Dictionary<string, long> dict, long quantity, long quantityCapacity, int typeCapacity) {
            if (dict == null) return null;
            Inventory result = Create();
            result.Quantity = quantity;
            result.QuantityCapacity = quantityCapacity;
            result.TypeCapacity = typeCapacity;
            foreach (var pair in dict) {
                result.Dict.Add(Type.GetType(pair.Key), pair.Value);
            }
            return result;
        }

        public static Inventory Create() {
            return new Inventory {
                Dict = new Dictionary<Type, long>(),
                Quantity = 0,
                QuantityCapacity = 0,
                TypeCapacity = 0,
            };
        }



    }
}

