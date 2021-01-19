
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
        bool CanAdd<T>(long val);
        bool Take<T>(long val);
        bool Take(Type type, long val);
        bool CanTake<T>(long val);

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


        public void Clear(Type type) {
            Take(type, Get(type));
        }
        public void Clear<T>() {
            Clear(typeof(T));
        }

        public bool Add<T>(long val) {
            if (val < 0) throw new Exception();
            if (val == 0) return true;
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
            if (val == 0) return true;
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

        public bool Take(Type type, long val) {
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
        public bool Take<T>(long val) {
            return Take(typeof(T), val);
        }
        public bool CanTake<T>(long val) {
            if (val < 0) throw new Exception();
            if (val == 0) return true;
            Type type = typeof(T);
            if (Dict.ContainsKey(type)) {
                if (Dict[type] < val) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        }

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

