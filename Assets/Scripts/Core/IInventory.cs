
using System;
using System.Collections;
using System.Collections.Generic;

namespace Weathering
{
    public interface IInventory : IEnumerable<KeyValuePair<Type, long>>
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

        long AddAsManyAsPossible<T>(IInventory value, long max = long.MaxValue);
        long AddAsManyAsPossible(Type type, IInventory value, long max = long.MaxValue);

        long AddAsManyAsPossible<T>(IValue value, long max = long.MaxValue);
        long AddAsManyAsPossible(Type type, IValue value, long max = long.MaxValue);

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
        void SetQuantity(long value);
    }

    public class Inventory : IInventoryDefinition
    {
        public int TypeCount { get => Dict.Count; }
        public int TypeCapacity { get; set; }

        public long Quantity { get; private set; }
        public long QuantityCapacity { get; set; }

        public void SetQuantity(long value) => Quantity = value;

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

        public bool Add(Type type, long val) {
            if (val < 0) throw new Exception();
            if (val == 0) return true;
            if (CanAdd(type) < val) throw new Exception();
            if (Dict.ContainsKey(type)) {
                Dict[type] += val;
            } else {
                Dict.Add(type, val);
            }
            Quantity += val;
            return true;
        }
        public bool Add<T>(long val) {
            return Add(typeof(T), val);
        }

        public long CanAdd(Type type) {
            long storage = QuantityCapacity - Quantity;
            if (storage == 0) return 0;
            if (TypeCount == TypeCapacity && !Dict.ContainsKey(type)) {
                return 0;
            }
            return storage;
        }
        public long CanAdd<T>() {
            return CanAdd(typeof(T));
        }

        public long AddAsManyAsPossible<T>(IInventory value, long max = long.MaxValue) {
            return AddAsManyAsPossible(typeof(T), value, max);
        }

        public long AddAsManyAsPossible(Type type, IInventory value, long max = long.MaxValue) {
            long add = CanAdd(type);
            long min = Math.Min(Math.Min(add, value.Get(type)), max);
            if (min < 0) throw new Exception(min.ToString());
            Add(type, min);
            value.Remove(type, min);
            return min;
        }

        public long AddAsManyAsPossible<T>(IValue value, long max = long.MaxValue) {
            return AddAsManyAsPossible(typeof(T), value, max);
        }

        public long AddAsManyAsPossible(Type type, IValue value, long max = long.MaxValue) {
            long add = CanAdd(type);
            long min = Math.Min(Math.Min(add, value.Val), max);
            if (min < 0) throw new Exception();
            Add(type, min);
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
                    } else {
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

        public static DataPersistence.InventoryData ToData(IInventory inventory) {
            if (inventory == null) return null;

            IInventoryDefinition definition = inventory as IInventoryDefinition;
            if (definition == null) throw new Exception();

            if (definition.Dict == null) throw new Exception();

            DataPersistence.InventoryData result = new DataPersistence.InventoryData();
            result.inventory_quantity = definition.Quantity;
            result.inventory_capacity = definition.QuantityCapacity;
            result.inventory_type_capacity = definition.TypeCapacity;
            result.inventory_dict = new Dictionary<string, long>();
            foreach (var pair in definition.Dict) {
                result.inventory_dict.Add(pair.Key.FullName, pair.Value);
            }
            return result;
        }

        public static IInventory FromData(DataPersistence.InventoryData data) {
            if (data == null) return null;
            if (data.inventory_dict == null) throw new Exception();

            IInventoryDefinition result = Create();

            long vertify = 0;
            foreach (var pair in data.inventory_dict) {
                vertify += pair.Value;
                result.Dict.Add(Type.GetType(pair.Key), pair.Value);
            }
            if (vertify != data.inventory_quantity) throw new Exception("存档背包物品数量错误");
            result.SetQuantity(data.inventory_quantity);
            result.QuantityCapacity = data.inventory_capacity;
            result.TypeCapacity = data.inventory_type_capacity;


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

        public IEnumerator<KeyValuePair<Type, long>> GetEnumerator() {
            return Dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Dict.GetEnumerator();
        }
    }
}

