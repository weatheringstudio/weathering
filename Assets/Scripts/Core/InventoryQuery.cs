
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public struct InventoryQueryItem
    {
        public IInventory Source;
        public IInventory Target;
        public Type Type;
        public long Quantity;
        public bool WithoutTag;
    }

    public class InventoryQuery
    {
        private InventoryQuery() { }
        private InventoryQueryItem[] inventoryQueryItems;
        private Action back;
        public static void Create(Action back, params InventoryQueryItem[] inventoryQueryItems) {
            if (back == null) throw new Exception();
            if (inventoryQueryItems == null) throw new Exception();
            InventoryQuery query = new InventoryQuery();
            query.back = back;
            query.inventoryQueryItems = inventoryQueryItems;
            foreach (var item in inventoryQueryItems) {
                if (item.Source == null && item.Target != null && !item.WithoutTag) throw new Exception(); // 没有源头，不能带tag
                if (item.Source == null && item.Target == null) throw new Exception();
                if (item.Type == null) throw new Exception();
                if (item.Quantity <= 0) throw new Exception();
            }
        }

        private Dictionary<IInventory, Dictionary<Type, InventoryItemData>> targetCombined = new Dictionary<IInventory, Dictionary<Type, InventoryItemData>>();
        public bool CanDo() {
            foreach (var item in inventoryQueryItems) {
                /// 创造一个字典，记录往某个背包里塞的所有内容
                Dictionary<Type, InventoryItemData> transfer = null;
                if (item.Target != null && !item.WithoutTag) {
                    if (!targetCombined.ContainsKey(item.Target)) {
                        transfer = new Dictionary<Type, InventoryItemData>();
                        targetCombined.Add(item.Target, transfer);
                    }
                    else {
                        transfer = targetCombined[item.Target];
                    }
                }
                if (item.Source != null) {
                    if (item.WithoutTag) {
                        if (item.Source.CanRemove(item.Type) < item.Quantity) {
                            UIPreset.ResourceInsufficient(item.Type, back, item.Quantity, item.Source);
                            return false;
                        }
                        if (transfer.ContainsKey(item.Type)) {
                            transfer[item.Type] = new InventoryItemData { value = transfer[item.Type].value + item.Quantity };
                        }
                        else {
                            transfer.Add(item.Type, new InventoryItemData { value = item.Quantity });
                        }
                    } else {
                        if (item.Source.CanRemoveWithTag(item.Type, transfer, item.Quantity) < item.Quantity) {
                            UIPreset.ResourceInsufficientWithTag(item.Type, back, item.Quantity, item.Source);
                            return false;
                        }
                    }
                }
            }
            foreach (var pair in targetCombined) {
                if (!pair.Key.CanAddEverything(pair.Value)) {
                    UIPreset.InventoryFull(back, pair.Key);
                    return false;
                }
            }
            targetCombined.Clear();
            return true;
        }
    }
}

