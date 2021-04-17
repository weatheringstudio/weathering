
using System;
using System.Collections;
using System.Collections.Generic;

namespace Weathering
{

    public struct InventoryItemData
    {
        public long value;
        public InventoryItemData SetVal(long val) {
            value = val;
            return this;
        }
        public InventoryItemData AddVal(long val) {
            value = val + value;
            return this;
        }
    }

    public interface IInventory : IEnumerable<KeyValuePair<Type, InventoryItemData>>
    {
        bool Maxed { get; }
        bool Empty { get; }
        void Clear<T>();
        void Clear(Type type);
        long CanRemove<T>();
        long CanRemove(Type type);
        bool CanRemove(ValueTuple<Type, long> pair);

        long GetWithTag(Type type);

        bool Add<T>(long val);
        bool Add(Type type, long val);
        bool Add(ValueTuple<Type, long> pair);

        long CanAdd<T>();
        long CanAdd(Type type);
        bool CanAdd(ValueTuple<Type, long> pair);

        bool Remove<T>(long val);
        bool Remove(Type type, long val);
        bool Remove(ValueTuple<Type, long> pair);
        //long CanRemove<T>();
        //long CanRemove(Type type);

        long AddFrom<T>(IInventory value, long max = long.MaxValue);
        long AddFrom(Type type, IInventory value, long max = long.MaxValue);
        long AddFrom<T>(IValue value, long max = long.MaxValue);
        long AddFrom(Type type, IValue value, long max = long.MaxValue);

        void AddEverythingFrom(IInventory other);

        bool RemoveWithTag<T>(long val, Dictionary<Type, InventoryItemData> canRemove = null, Dictionary<Type, InventoryItemData> removed = null);
        bool RemoveWithTag(Type type, long val, Dictionary<Type, InventoryItemData> canRemove = null, Dictionary<Type, InventoryItemData> removed = null);
        bool RemoveWithTag(ValueTuple<Type, long> pair);
        long CanRemoveWithTag<T>(Dictionary<Type, InventoryItemData> canRemove = null, long max = long.MaxValue);
        long CanRemoveWithTag(Type type, Dictionary<Type, InventoryItemData> canRemove = null, long max = long.MaxValue);
        bool CanRemoveWithTag(ValueTuple<Type, long> pair);

        bool AddFromWithTag<T>(IInventory value, long max);
        bool CanAddWithTag(Dictionary<Type, InventoryItemData> data, long val);

        bool CanAddEverything(Dictionary<Type, InventoryItemData> other);
        void AddEverything(Dictionary<Type, InventoryItemData> other);

        // bool CanAddFromWithTag<T>(IInventory other, long val);

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
        Dictionary<Type, InventoryItemData> Dict { get; }
        void SetQuantity(long value);
    }

    public class Inventory : IInventoryDefinition
    {
        public bool Maxed { get => Quantity == QuantityCapacity; }
        public bool Empty { get => Quantity == 0; }
        public int TypeCount { get => Dict.Count; }
        public int TypeCapacity { get; set; }

        public long Quantity { get; private set; }
        public long QuantityCapacity { get; set; }

        public void SetQuantity(long value) => Quantity = value;

        private Inventory() { }

        public Dictionary<Type, InventoryItemData> Dict { get; private set; } = null;


        public long CanRemove(Type type) {
            if (Dict.TryGetValue(type, out InventoryItemData value)) {
                return value.value;
            } else {
                return 0;
            }
        }
        public long CanRemove<T>() => CanRemove(typeof(T));
        public bool CanRemove(ValueTuple<Type, long> pair) => CanRemove(pair.Item1) >= pair.Item2;

        public void Clear(Type type) => Remove(type, CanRemove(type));
        public void Clear<T>() => Clear(typeof(T));

        public bool Add(Type type, long val) {
            if (val < 0) throw new Exception();
            if (val == 0) return true;
            if (CanAdd(type) < val) throw new Exception();
            if (Dict.ContainsKey(type)) {
                Dict[type] = Dict[type].AddVal(val);
            } else {
                Dict.Add(type, new InventoryItemData { value = val });
            }
            Quantity += val;
            return true;
        }
        public bool Add(ValueTuple<Type, long> pair) => Add(pair.Item1, pair.Item2);
        public bool Add<T>(long val) => Add(typeof(T), val);

        /// <summary>
        /// 计算能往背包里添加多少个资源
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <returns>数量</returns>
        public long CanAdd(Type type) {
            long storage = QuantityCapacity - Quantity;
            if (storage == 0) return 0;
            if (TypeCount == TypeCapacity && !Dict.ContainsKey(type)) {
                return 0;
            }
            return storage;
        }
        public bool CanAdd(ValueTuple<Type, long> pair) => CanAdd(pair.Item1) >= pair.Item2;
        public long CanAdd<T>() => CanAdd(typeof(T));

        /// <summary>
        /// 从背包移除资源
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="val">移除数量</param>
        /// <returns>是否成功移除</returns>
        public bool Remove(Type type, long val) {
            if (val < 0) throw new Exception();
            if (val == 0) return true;
            if (Dict.ContainsKey(type)) {
                if (Dict[type].value < val) {
                    throw new Exception();
                    // return false;
                } else {
                    if (val == Dict[type].value) {
                        Dict.Remove(type);
                    } else {
                        Dict[type] = Dict[type].AddVal(-val);
                    }
                    Quantity -= val;
                    return true;
                }
            } else {
                throw new Exception();
                // return false;
            }
        }
        public bool Remove(ValueTuple<Type, long> pair) {
            return Remove(pair.Item1, pair.Item2);
        }
        public bool Remove<T>(long val) {
            return Remove(typeof(T), val);
        }

        public long GetWithTag(Type type) {
            return CanRemoveWithTag(type);
        }

        /// <summary>
        /// 计算能从背包移除多少个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        //public long CanRemove<T>() {
        //    return CanRemove(typeof(T));
        //}
        //public long CanRemove(Type type) {
        //    InventoryItemData result;
        //    if (Dict.TryGetValue(type, out result)) {
        //        return result.value;
        //    }
        //    return 0;
        //}

        public long AddFrom<T>(IInventory other, long max = long.MaxValue) {
            return AddFrom(typeof(T), other, max);
        }

        /// <summary>
        /// 从其他背包获得资源
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="other">其他背包</param>
        /// <param name="max">最多拿多少个。默认long.MaxValue</param>
        /// <returns>实际拿了多少个, 0则不成功</returns>
        public long AddFrom(Type type, IInventory other, long max = long.MaxValue) {
            long add = CanAdd(type);
            long min = Math.Min(Math.Min(add, other.CanRemove(type)), max);
            if (min < 0) throw new Exception(min.ToString());
            Add(type, min);
            other.Remove(type, min);
            return min;
        }

        public long AddFrom<T>(IValue other, long max = long.MaxValue) {
            return AddFrom(typeof(T), other, max);
        }

        public long AddFrom(Type type, IValue other, long max = long.MaxValue) {
            long add = CanAdd(type);
            long min = Math.Min(Math.Min(add, other.Val), max);
            if (min < 0) throw new Exception();
            Add(type, min);
            other.Val -= min;
            return min;
        }

        /// <summary>
        /// 从背包里移除某些资源
        /// </summary>
        /// <typeparam name="T">资源必须是T的子类</typeparam>
        /// <param name="val">移除的数量</param>
        /// <param name="canRemove">canRemove提供的预计算</param>
        /// <param name="removed">具体移除的类型和数量, 在这里</param>
        /// <returns>是否移除成功。可能移除一半后发现不成功</returns>
        public bool RemoveWithTag<T>(long val, Dictionary<Type, InventoryItemData> canRemove = null, Dictionary<Type, InventoryItemData> removed = null) {
            return RemoveWithTag(typeof(T), val, canRemove, removed);
        }
        public bool RemoveWithTag(Type type, long val, Dictionary<Type, InventoryItemData> canRemove = null, Dictionary<Type, InventoryItemData> removed = null) {
            if (canRemove == null) {
                if (val == 0) return true;
                while (val != 0) {
                    foreach (var pair in Dict) {
                        if (Tag.HasTag(pair.Key, type)) {
                            long max = Math.Min(val, pair.Value.value);
                            bool result = Remove(pair.Key, max);
                            if (!result) throw new Exception(pair.Key.Name);
                            if (removed != null) {
                                removed.Add(pair.Key, new InventoryItemData { value = max });
                            }
                            val -= max;
                            break;
                        }
                    }
                }
                if (val != 0) throw new Exception();
                return true;
                // return val == 0;
            } else {
                if (val == 0) return true;
                foreach (var pair in canRemove) {
                    if (!Tag.HasTag(pair.Key, type)) throw new Exception($"{pair.Key} - {type}");
                    long max = Math.Min(val, pair.Value.value);
                    bool result = Remove(pair.Key, max);
                    if (!result) throw new Exception(pair.Key.Name);
                    if (removed != null) {
                        removed.Add(pair.Key, new InventoryItemData { value = max });
                    }
                    val -= max;
                    if (val == 0) {
                        break;
                    }
                }
                if (val != 0) throw new Exception();
                return true;
                // return val == 0;
            }
        }
        public bool RemoveWithTag(ValueTuple<Type, long> pair) {
            return RemoveWithTag(pair.Item1, pair.Item2);
        }

        /// <summary>
        /// 判断是否能够从背包移除资源
        /// </summary>
        /// <typeparam name="T">移除的资源必须是T的子类型</typeparam>
        /// <param name="canRemove">具体移除什么, 预计算记录在这里</param>
        /// <param name="val">需要移除多少个</param>
        /// <returns>可以移除多少个。若等于参数val则可以移除</returns>
        public long CanRemoveWithTag<T>(Dictionary<Type, InventoryItemData> canRemove = null, long val = long.MaxValue) {
            return CanRemoveWithTag(typeof(T), canRemove, val);
        }
        public long CanRemoveWithTag(Type type, Dictionary<Type, InventoryItemData> canRemoveAccumulated = null, long val = long.MaxValue) {
            long result = 0;
            foreach (var pair in Dict) {
                if (Tag.HasTag(pair.Key, type)) {
                    long min = Math.Min(val, pair.Value.value);
                    if (min == 0) continue;
                    val -= min;
                    result += min;
                    if (canRemoveAccumulated != null) {
                        if (canRemoveAccumulated.ContainsKey(pair.Key)) {
                            canRemoveAccumulated[pair.Key] = new InventoryItemData { value = canRemoveAccumulated[pair.Key].value + min };
                        } else {
                            canRemoveAccumulated.Add(
                                pair.Key,
                                new InventoryItemData { value = min }
                            );
                        }
                    }
                }
            }
            return result;
        }
        public bool CanRemoveWithTag(ValueTuple<Type, long> pair) {
            return CanRemoveWithTag(pair.Item1) >= pair.Item2;
        }

        /// <summary>
        /// 从另一个背包添加资源
        /// </summary>
        /// <typeparam name="T">资源必须是T的子类型</typeparam>
        /// <param name="other">其他背包</param>
        /// <param name="val">需要添加的资源的个数</param>
        /// <returns>是否成功</returns>
        public bool AddFromWithTag<T>(IInventory other, long val) {
            Dictionary<Type, InventoryItemData> canRemoveDict = new Dictionary<Type, InventoryItemData>();
            long canRemove = other.CanRemoveWithTag<T>(canRemoveDict, val);
            if (canRemove < val) return false;

            if (!CanAddWithTag(canRemoveDict, val)) {
                return false;
            }

            Dictionary<Type, InventoryItemData> removed = new Dictionary<Type, InventoryItemData>();
            other.RemoveWithTag<T>(canRemove, canRemoveDict, removed);
            foreach (var pair in removed) {
                Add(pair.Key, pair.Value.value);
            }
            return true;
        }

        /// <summary>
        /// 计算从另一个背包出来的物品, 能否被这个背包容纳
        /// </summary>
        /// <typeparam name="T">物品必须是T的子类型</typeparam>
        /// <param name="other">另一个背包</param>
        /// <param name="val">需要容纳的数量</param>
        /// <returns>是否能容纳</returns>
        public bool CanAddWithTag(Dictionary<Type, InventoryItemData> other, long val) {
            if (val > QuantityCapacity - Quantity) { return false; }
            long typeRoomRequired = other.Count;
            typeRoomRequired -= TypeCapacity - TypeCount;
            if (typeRoomRequired > 0) {
                foreach (var pair in other) {
                    if (Dict.ContainsKey(pair.Key)) {
                        typeRoomRequired--;
                        if (typeRoomRequired == 0) {
                            break;
                        }
                    }
                }
            }
            return typeRoomRequired <= 0;
        }

        public bool CanAddEverything(Dictionary<Type, InventoryItemData> other) {
            long capacityProvided = QuantityCapacity - Quantity;
            long capacityRequired = 0;
            foreach (var item in other) {
                capacityRequired += item.Value.value;
                if (capacityRequired > capacityProvided) {
                    return false;
                }
            }

            long typeRoomRequired = other.Count;
            typeRoomRequired -= TypeCapacity - TypeCount;
            if (typeRoomRequired > 0) {
                foreach (var pair in other) {
                    if (Dict.ContainsKey(pair.Key)) {
                        typeRoomRequired--;
                        if (typeRoomRequired == 0) {
                            break;
                        }
                    }
                }
            }
            return typeRoomRequired <= 0;
        }
        public void AddEverything(Dictionary<Type, InventoryItemData> everything) {
            if (!CanAddEverything(everything)) throw new Exception();
            foreach (var pair in everything) {
                Add(pair.Key, pair.Value.value);
            }
        }


        private static List<Type> allTypes = new List<Type>();
        public void AddEverythingFrom(IInventory other) {
            foreach (var pair in other) {
                allTypes.Add(pair.Key);
            }
            foreach (var type in allTypes) {
                AddFrom(type, other);
            }
            allTypes.Clear();
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
            result.inventory_dict = new Dictionary<string, InventoryItemData>();
            foreach (var pair in definition.Dict) {
                result.inventory_dict.Add(pair.Key.FullName, pair.Value);
            }
            return result;
        }

        public static IInventory FromData(DataPersistence.InventoryData data) {
            if (data == null) return null;
            if (data.inventory_dict == null) throw new Exception();

            IInventoryDefinition result = GetOne();

            long vertify = 0;
            foreach (var pair in data.inventory_dict) {
                vertify += pair.Value.value;
                result.Dict.Add(Type.GetType(pair.Key), pair.Value);
            }
            if (vertify != data.inventory_quantity) throw new Exception("存档背包物品数量错误");
            result.SetQuantity(data.inventory_quantity);
            result.QuantityCapacity = data.inventory_capacity;
            result.TypeCapacity = data.inventory_type_capacity;


            return result;
        }

        public static Inventory GetOne() {
            return new Inventory {
                Dict = new Dictionary<Type, InventoryItemData>(),
                Quantity = 0,
                QuantityCapacity = 0,
                TypeCapacity = 0,
            };
        }

        public IEnumerator<KeyValuePair<Type, InventoryItemData>> GetEnumerator() {
            return Dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Dict.GetEnumerator();
        }


    }
}

