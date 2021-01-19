
using System;
using System.Collections.Generic;

namespace Weathering
{
    public interface IInventory
    {
        void Clear<T>();
        void Clear(Type type);
        long Get<T>();
        long Get(Type type);
        bool Add<T>(long val);
        long CanAdd<T>();
        bool Remove<T>(long val);
        bool Remove(Type type, long val);
        long CanRemove<T>();

        long AddAsManyAsPossible<T>(IValue value);

        /// <summary>
        /// 背包物品种类
        /// </summary>
        int TypeCount { get; }
        /// <summary>
        /// 背包物品种类上限
        /// </summary>
        int TypeCapacity { get; set; }
        /// <summary>
        /// 背包物品总数量
        /// </summary>
        long Quantity { get; }
        /// <summary>
        /// 背包物品总数量上限
        /// </summary>
        long QuantityCapacity { get; set; }

    }

    public interface IInventoryDefinition : IInventory
    {
        Dictionary<Type, long> Dict { get; }
    }

    public class Inventory : IInventoryDefinition {
        public int TypeCount { get => Dict.Count; }
        public int TypeCapacity { get; set; }

        public long Quantity { get; private set; }
        public long QuantityCapacity { get; set; }

        private Inventory() { }

        public Dictionary<Type, long> Dict { get; private set; } = null;


        public long Get(Type type) {
            if (Dict.TryGetValue(type, out long value)) {
                return value;
            } else {
                return 0;
            }
        }
        public long Get<T>() {
            return Get(typeof(T));
        }

        public void Clear(Type type) {
            Remove(type, Get(type));
        }
        public void Clear<T>() {
            Clear(typeof(T));
        }

        public bool Add<T>(long val) {
            if (val < 0) throw new Exception();
            if (val == 0) return true;
            Type type = typeof(T);
            if (CanAdd<T>() < val) throw new Exception();
            if (Dict.ContainsKey(type)) {
                Dict[type] += val;
            }
            else {
                Dict.Add(type, val);
            }
            Quantity += val;
            return true;
        }
        public long CanAdd<T>() {
            Type type = typeof(T);
            long storage = QuantityCapacity - Quantity;
            if (storage == 0) return 0;
            if (TypeCount==TypeCapacity && !Dict.ContainsKey(type)) {
                return 0;
            }
            return storage;
        }
        public long AddAsManyAsPossible<T>(IValue value, long max) {
            long add = CanAdd<T>();
            long min = Math.Min(Math.Min(add, value.Val), max);
            if (min < 0) throw new Exception();
            Add<T>(min);
            value.Val -= min;
            return min;
        }
        public long AddAsManyAsPossible<T>(IValue value) {
            long add = CanAdd<T>();
            long min = Math.Min(add, value.Val);
            if (min < 0) throw new Exception();
            Add<T>(min);
            value.Val -= min;
            return min;
        }

        public bool Remove(Type type, long val) {
            if (val < 0) throw new Exception();
            if (val == 0) return true;
            if (Dict.ContainsKey(type)) {
                if (Dict[type] < val) {
                    return false;
                } else {
                    if (val == Dict[type]) {
                        Dict.Remove(type);
                    }
                    else {
                        Dict[type] -= val;
                    }
                    Quantity -= val;
                    return true;
                }
            } else {
                return false;
            }
        }
        public bool Remove<T>(long val) {
            return Remove(typeof(T), val);
        }

        public long CanRemove<T>() {
            long result;
            if (Dict.TryGetValue(typeof(T), out result)) {
                return result;
            }
            return 0;
        }


        public static Dictionary<string, long> ToData(IInventory inventory) {
            if (inventory == null) return null;
            IInventoryDefinition definition = inventory as IInventoryDefinition;
            if (definition == null) throw new Exception();
            Dictionary<string, long> result = new Dictionary<string, long>();
            foreach (var pair in definition.Dict) {
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

