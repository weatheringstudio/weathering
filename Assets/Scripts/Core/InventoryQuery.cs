
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    /// <summary>
    /// 一个涉及背包查询的命令
    /// </summary>
    public struct InventoryQueryItem
    {
        /// <summary>
        /// 从哪个背包拿物品。如果为空则凭空创建。Source和Target不能同时为空
        /// </summary>
        public IInventory Source;
        /// <summary>
        /// 物品会进入哪个背包。如果为空则凭空消失。Source和Target不能同时为空
        /// </summary>
        public IInventory Target;

        /// <summary>
        /// 物品的类型
        /// </summary>
        public Type Type;
        /// <summary>
        /// 物品的数量
        /// </summary>
        public long Quantity;
        /// <summary>
        /// 从源头拿物品时，考不考虑物品的子类。默认考虑。如果没源头则没影响
        /// </summary>
        public bool SourceIgnoreSubtype;

        /// <summary>
        /// 内部使用。用于记录从源头拿物品时，具体拿的子类物品的类型和数量
        /// </summary>
        public Dictionary<Type, InventoryItemData> _Val;
    }

    public class InventoryQueryInformationOfRevenueDisabled { }
    public class InventoryQueryInformationOfCostDisabled { }


    public class InventoryQuery
    {

        private InventoryQuery() { }
        /// <summary>
        /// 背包查询组合
        /// </summary>
        private List<InventoryQueryItem> inventoryQueryItems;
        /// <summary>
        /// 返回界面
        /// </summary>
        private Action back;
        /// <summary>
        /// 如果物品转移涉及informedInventory，那么会给玩家提示
        /// </summary>
        private IInventory informedInventory;


        /// <summary>
        /// 一个未解决的问题：举例：物品ABC，物品A是B也是C，如果需要3个B和3个C，那么可能只消耗3个A而不是6个A
        /// </summary>
        public static InventoryQuery Create(Action back, IInventory informedInventory, params InventoryQueryItem[] inventoryQueryItems) {
            return Create(back, informedInventory, new List<InventoryQueryItem>(inventoryQueryItems));
        }

        /// <summary>
        /// 创建一个查询组合
        /// </summary>
        public static InventoryQuery Create(Action back, IInventory informedInventory, List<InventoryQueryItem> inventoryQueryItems) {
            if (back == null) throw new Exception();
            if (inventoryQueryItems == null) throw new Exception();
            if (inventoryQueryItems.Count == 0) throw new Exception();
            if (informedInventory == null) throw new Exception();
            InventoryQuery query = new InventoryQuery();
            query.back = back;
            query.inventoryQueryItems = inventoryQueryItems;
            query.informedInventory = informedInventory;
            // Debug.LogWarning(inventoryQueryItems.Count);
            foreach (var item in inventoryQueryItems) {
                // Debug.Log($"{item.Source==informedInventory}  {item.Target== informedInventory}  {item.Type.Name}  {item.Quantity}");
                if (item.Source == null && item.Target == null) throw new Exception();
                if (item.Type == null) throw new Exception();
                if (item.Quantity <= 0) throw new Exception();
                if (item.Source == null && item.SourceIgnoreSubtype) throw new Exception();
                if (item._Val != null) throw new Exception();
            }
            return query;
        }
        public InventoryQuery CreateInversed() {
            List<InventoryQueryItem> resultItems = new List<InventoryQueryItem>();
            for (int i = 0; i < inventoryQueryItems.Count; i++) {
                InventoryQueryItem originalInventoryQueryItem = inventoryQueryItems[i];
                InventoryQueryItem inventoryQueryItem = originalInventoryQueryItem;
                inventoryQueryItem.Source = originalInventoryQueryItem.Target;
                inventoryQueryItem.Target = originalInventoryQueryItem.Source;
                inventoryQueryItem._Val = null;
                resultItems.Add(inventoryQueryItem);
            }
            InventoryQuery result = Create(back, informedInventory, resultItems);
            return result;
        }

        private bool targetCombinedCreated = false;
        private Dictionary<IInventory, Dictionary<Type, InventoryItemData>> allToEveryTargetInventory = new Dictionary<IInventory, Dictionary<Type, InventoryItemData>>();

        private bool CanExecuteQuery() {
            // can do 后，下一个必须是do？
            if (targetCombinedCreated) throw new Exception();
            targetCombinedCreated = true;

            // 对于每一个查询项
            for (int i = 0; i < inventoryQueryItems.Count; i++) {
                var item = inventoryQueryItems[i];
                // 如果这个查询项有目标背包，那么创建一个背包，记录所有将转移到这个背包的物品，方便以后判断能否装得下这些东西
                Dictionary<Type, InventoryItemData> allToOneTargetInventory = null;
                if (item.Target != null) {
                    if (!allToEveryTargetInventory.ContainsKey(item.Target)) {
                        allToOneTargetInventory = new Dictionary<Type, InventoryItemData>();
                        allToEveryTargetInventory.Add(item.Target, allToOneTargetInventory);
                    } else {
                        allToOneTargetInventory = allToEveryTargetInventory[item.Target];
                    }
                }
                // 如果这个查询项有来源背包
                if (item.Source != null) {
                    if (item.SourceIgnoreSubtype) {
                        // 从来源背包拿物品时，不考虑子类
                        if (item.Source.CanRemove(item.Type) < item.Quantity) {
                            UIPreset.ResourceInsufficient(item.Type, back, item.Quantity, item.Source);
                            return false;
                        }
                        if (item.Target != null) {
                            // 又是重复开发，哎
                            if (allToOneTargetInventory.ContainsKey(item.Type)) {
                                allToOneTargetInventory[item.Type] = new InventoryItemData { value = allToOneTargetInventory[item.Type].value + item.Quantity };
                            } else {
                                allToOneTargetInventory.Add(item.Type, new InventoryItemData { value = item.Quantity });
                            }
                        }

                    } else {
                        // 从来源背包拿物品时，要考虑子类
                        Dictionary<Type, InventoryItemData> val = new Dictionary<Type, InventoryItemData>();
                        if (item.Source.CanRemoveWithTag(item.Type, val, item.Quantity) < item.Quantity) {
                            UIPreset.ResourceInsufficientWithTag(item.Type, back, item.Quantity, item.Source);
                            return false;
                        }
                        if (item.Target != null) {
                            // 把这一项合并于allToTarget，以判断目标背包能否塞得下这些东西
                            Add(allToOneTargetInventory, val);
                        }
                        // 把这一项加入_Val，以判断应该转移哪个子类多少个
                        // inventoryQueryItems[i]._Val = val;
                        InventoryQueryItem inventoryQueryItem = inventoryQueryItems[i];
                        inventoryQueryItem._Val = val;
                        inventoryQueryItems[i] = inventoryQueryItem;
                    }
                }
            }
            // 对于每个目标背包，判断能否塞下对应东西
            foreach (var pair in allToEveryTargetInventory) {
                if (!pair.Key.CanAddEverything(pair.Value)) {
                    UIPreset.InventoryFull(back, pair.Key);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 其实Inventory和Dictionary<Type, InventoryItemData>差不多，这些东西实现了好多份。开发得不好，将就用
        /// </summary>
        private void Add(Dictionary<Type, InventoryItemData> source, Dictionary<Type, InventoryItemData> val) {
            foreach (var item in val) {
                if (source.ContainsKey(item.Key)) {
                    source[item.Key] = new InventoryItemData { value = source[item.Key].value + item.Value.value };
                } else {
                    source.Add(item.Key, new InventoryItemData { value = item.Value.value });
                }
            }
        }

        public void ExecuteQuery() {
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
                // 目标背包凭空出现资源，
                if (item.Target != null && (item.Source == null || item.SourceIgnoreSubtype)) {
                    item.Target.Add(item.Type, item.Quantity);
                }
            }
            // 目标背包获得其他背包的资源
            foreach (var pair in allToEveryTargetInventory) {
                pair.Key.AddEverything(pair.Value);
            }
            allToEveryTargetInventory.Clear();
        }

        private void AskUserToExecuteQuery(Action after = null) {
            Action confirm = () => {
                ExecuteQuery();
                after?.Invoke();
                if (Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>()) {
                    back();
                }
                List<IUIItem> items = null;
                bool foundRevenue = false;
                foreach (var pair in allToEveryTargetInventory) {
                    if (pair.Key == informedInventory) {
                        foundRevenue = true;
                        if (items == null) items = new List<IUIItem>();
                        foreach (var item in pair.Value) {
                            items.Add(UIItem.CreateDynamicText(() => $"获得{Localization.Ins.Val(item.Key, item.Value.value)}"));
                        }
                        break;
                    }
                }
                foreach (var item in inventoryQueryItems) {
                    if (item.Target == informedInventory && (item.Source == null || item.SourceIgnoreSubtype)) {
                        foundRevenue = true;
                        if (items == null) items = new List<IUIItem>();
                        items.Add(UIItem.CreateDynamicText(() => $"获得{Localization.Ins.Val(item.Type, item.Quantity)}"));
                    }
                }
                if (foundRevenue && !Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>()) {
                    items.Add(UIItem.CreateReturnButton(back));
                    UI.Ins.ShowItems("获得资源", items);
                } else {
                    // 没有资源获得就不用确认
                    back();
                }
            };

            if (Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>()) {
                // 设置里跳过确认
                confirm();
                return;
            }

            var uiItem = new List<IUIItem>();
            bool found = false;
            uiItem.Add(UIItem.CreateText("需要以下资源，是否审批通过？"));
            foreach (var queryItem in inventoryQueryItems) {
                if (queryItem.Source == informedInventory) {
                    if (queryItem.SourceIgnoreSubtype) {
                        uiItem.Add(UIItem.CreateText(string.Format(Localization.Ins.Get(queryItem.Type), queryItem.Quantity)));
                    } else {
                        foreach (var pair in queryItem._Val) {
                            uiItem.Add(UIItem.CreateText(string.Format(Localization.Ins.Get(pair.Key), pair.Value.value)));
                        }
                    }
                    found = true;
                }
            }

            // 没有资源需求就不用确认
            if (!found) {
                confirm();
                return;
            }
            uiItem.Add(UIItem.CreateButton("通过", confirm));
            uiItem.Add(UIItem.CreateButton("取消", back));

            UI.Ins.ShowItems("资源需求订单", uiItem);
        }

        public void TryDo(Action after) {
            if (!CanExecuteQuery()) {
                return;
            }
            AskUserToExecuteQuery(after);
        }

        public string GetDescription() {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var item in inventoryQueryItems) {
                if (item.Source == informedInventory) {
                    sb.Append(Localization.Ins.ValPlus(item.Type, -item.Quantity));
                }
            }
            foreach (var item in inventoryQueryItems) {
                if (item.Target == informedInventory) {
                    sb.Append(Localization.Ins.ValPlus(item.Type, item.Quantity));
                }
            }
            return sb.ToString();
        }
    }
}

