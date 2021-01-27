
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
        public bool SourceIgnoreSubtype;
        public Dictionary<Type, InventoryItemData> _Val;
    }

    public class InventoryQuery
    {
        private InventoryQuery() { }
        private InventoryQueryItem[] inventoryQueryItems;
        private Action back;
        private IInventory notifyInventory;
        public static InventoryQuery Create(Action back, IInventory notifyInventory, params InventoryQueryItem[] inventoryQueryItems) {
            if (back == null) throw new Exception();
            if (inventoryQueryItems == null) throw new Exception();
            if (notifyInventory == null) throw new Exception();
            InventoryQuery query = new InventoryQuery();
            query.back = back;
            query.inventoryQueryItems = inventoryQueryItems;
            query.notifyInventory = notifyInventory;
            foreach (var item in inventoryQueryItems) {
                if (item.Source == null && item.Target == null) throw new Exception();
                if (item.Type == null) throw new Exception();
                if (item.Quantity <= 0) throw new Exception();
            }
            return query;
        }

        private bool targetCombinedCreated = false;
        private Dictionary<IInventory, Dictionary<Type, InventoryItemData>> targetCombined = new Dictionary<IInventory, Dictionary<Type, InventoryItemData>>();
        public bool CanDo() {
            // can do 后，下一个必须是do？
            if (targetCombinedCreated) throw new Exception();
            targetCombinedCreated = true;

            for (int i = 0; i < inventoryQueryItems.Length; i++) {
                var item = inventoryQueryItems[i];
                Dictionary<Type, InventoryItemData> allToTarget = null;
                if (item.Target != null && !item.SourceIgnoreSubtype) {
                    if (!targetCombined.ContainsKey(item.Target)) {
                        allToTarget = new Dictionary<Type, InventoryItemData>();
                        targetCombined.Add(item.Target, allToTarget);
                    } else {
                        allToTarget = targetCombined[item.Target];
                    }
                }
                if (item.Source != null) {
                    if (item.SourceIgnoreSubtype) {
                        if (item.Source.CanRemove(item.Type) < item.Quantity) {
                            UIPreset.ResourceInsufficient(item.Type, back, item.Quantity, item.Source);
                            return false;
                        }
                        if (allToTarget.ContainsKey(item.Type)) {
                            allToTarget[item.Type] = new InventoryItemData { value = allToTarget[item.Type].value + item.Quantity };
                        } else {
                            allToTarget.Add(item.Type, new InventoryItemData { value = item.Quantity });
                        }
                    } else {
                        Dictionary<Type, InventoryItemData> val = new Dictionary<Type, InventoryItemData>();
                        if (item.Source.CanRemoveWithTag(item.Type, val, item.Quantity) < item.Quantity) {
                            UIPreset.ResourceInsufficientWithTag(item.Type, back, item.Quantity, item.Source);
                            return false;
                        }
                        Add(allToTarget, val);
                        inventoryQueryItems[i]._Val = val;
                    }
                }
            }
            foreach (var pair in targetCombined) {
                if (!pair.Key.CanAddEverything(pair.Value)) {
                    UIPreset.InventoryFull(back, pair.Key);
                    return false;
                }
            }
            return true;
        }

        private void Add(Dictionary<Type, InventoryItemData> source, Dictionary<Type, InventoryItemData> val) {
            foreach (var item in val) {
                if (source.ContainsKey(item.Key)) {
                    source[item.Key] = new InventoryItemData { value = source[item.Key].value + item.Value.value };
                } else {
                    source.Add(item.Key, new InventoryItemData { value = item.Value.value });
                }
            }
        }

        public void Do() {
            // 先用 cando
            if (!targetCombinedCreated) throw new Exception();
            targetCombinedCreated = false;

            foreach (var item in inventoryQueryItems) {
                if (item.Source != null) {
                    if (item.SourceIgnoreSubtype) {
                        item.Source.Remove(item.Type, item.Quantity);
                    } else {
                        item.Source.RemoveWithTag(item.Type, item.Quantity, item._Val, null);
                    }
                }
                if (item.Target != null && (item.Source == null || item.SourceIgnoreSubtype)) {
                    item.Target.Add(item.Type, item.Quantity);
                }
            }
            foreach (var pair in targetCombined) {
                pair.Key.AddEverything(pair.Value);
            }
            targetCombined.Clear();
        }

        public void Ask(Action after = null) {
            var uiItem = new List<IUIItem>();
            foreach (var queryItem in inventoryQueryItems) {
                if (queryItem.Source == notifyInventory) {
                    if (queryItem.SourceIgnoreSubtype) {
                        uiItem.Add(UIItem.CreateText(string.Format(Localization.Ins.Get(queryItem.Type), queryItem.Quantity)));
                    } else {
                        foreach (var pair in queryItem._Val) {
                            uiItem.Add(UIItem.CreateText(string.Format(Localization.Ins.Get(pair.Key), pair.Value.value)));
                        }
                    }
                }
            }
            uiItem.Add(UIItem.CreateButton("确认", () => {
                Do();
                after?.Invoke();
                List<IUIItem> items = null;
                bool notifyFound = false;
                foreach (var pair in targetCombined) {
                    if (pair.Key == notifyInventory) {
                        notifyFound = true;
                        if (items == null) items = new List<IUIItem>();
                        foreach (var item in pair.Value) {
                            items.Add(UIItem.CreateDynamicText(() => $"获得{Localization.Ins.Val(item.Key, item.Value.value)}"));
                        }
                        break;
                    }
                }
                foreach (var item in inventoryQueryItems) {
                    if (item.Target == notifyInventory && (item.Source == null || item.SourceIgnoreSubtype)) {
                        notifyFound = true;
                        if (items == null) items = new List<IUIItem>();
                        items.Add(UIItem.CreateDynamicText(() => $"获得{Localization.Ins.Val(item.Type, item.Quantity)}"));
                    }
                }
                if (notifyFound) {
                    items.Add(UIItem.CreateReturnButton(back));
                    UI.Ins.ShowItems("获得资源", items);
                } else {
                    back();
                }
            }));
            uiItem.Add(UIItem.CreateButton("取消", back));

            UI.Ins.ShowItems("是否提供以下资源？", uiItem);
        }
    }
}

