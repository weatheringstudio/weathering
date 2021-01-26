
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Village : StandardTile
    {
        public override string SpriteKey => "Residence";


        IValue level;

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            level = Values.Create<Level>();
            level.Max = -1;

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 10;
            Inventory.TypeCapacity = 5;
        }

        public override void OnEnable() {
            base.OnEnable();

            level = Values.Get<Level>();
        }

        private const long foodSupplyCost = 2;
        private const long workerRevenue = 1;
        private const long maxLevel = 3;

        public override void OnTap() {

            string title = "村庄";
            var items = new List<IUIItem>();

            if (level.Max == -1) {
                // 还没建房子
                items.Add(UIItem.CreateText("村里还没有房子"));
                items.Add(UIItem.CreateButton("造房子", () => {
                    level.Max = 0;
                    OnTap();
                }));
            }

            if (level.Max >= 0 && level.Max < maxLevel) {
                items.Add(UIItem.CreateText($"村庄人数：{level.Max}"));

                items.Add(UIItem.CreateButton($"为村民提供{Localization.Ins.Get<FoodSupply>()}", () => {
                    // 有空间放工人
                    if (Map.Inventory.CanAdd<Worker>() <= 0) {
                        UIPreset.InventoryFull(OnTap, Map.Inventory);
                        return;
                    }

                    // 有食物供给
                    Dictionary<Type, InventoryItemData> data = new Dictionary<Type, InventoryItemData>();
                    if (Map.Inventory.CanRemoveWithTag<FoodSupply>(ref data, foodSupplyCost) < foodSupplyCost) {
                        UIPreset.ResourceInsufficient<FoodSupply>(OnTap, foodSupplyCost, Map.Inventory);
                        return;
                    }

                    // 有空间放食物
                    if (!Inventory.CanAddWithTag<FoodSupply>(foodSupplyCost, data)) {
                        UIPreset.InventoryFull(OnTap, Inventory);
                        // 背包装满了
                        return;
                    }

                    Map.Inventory.Add<Worker>(workerRevenue);
                    Inventory.AddFromWithTag<FoodSupply>(Map.Inventory, foodSupplyCost);
                    level.Max++;
                    OnTap();
                }));
            }


            items.Add(UIItem.CreateSeparator());
            UIItem.AddEntireInventory(Inventory, items, OnTap);

            UI.Ins.ShowItems(title, items);
        }
        private bool CanFindFoodOn(Vector2Int pos) {
            ITile tile = Map.Get(Pos + Vector2Int.up);
            return tile is Forest || tile is Grassland;
        }
    }
}

