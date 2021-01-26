
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Village : StandardTile
    {
        public override string SpriteKey => level.Max < 0 ? "ResidenceOnBuilding" : "Residence";


        IValue level;

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            level = Values.Create<Level>();
            level.Max = -1;

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 6;
            Inventory.TypeCapacity = 6;
        }

        public override void OnEnable() {
            base.OnEnable();

            level = Values.Get<Level>();
        }

        private const long foodSupplyCost = 2;
        private const long workerRevenue = 1;
        private const long levelMax = 3;

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

            if (level.Max >= 0) {
                items.Add(UIItem.CreateText($"村庄人数：{level.Max} / {levelMax}"));

                items.Add(UIItem.CreateButton($"为村民提供{Localization.Ins.Get<FoodSupply>()}", () => {

                    // 有工人。凭空造

                    // 有空间放工人
                    if (Map.Inventory.CanAdd<Worker>() <= 0) {
                        UIPreset.InventoryFull(OnTap, Map.Inventory);
                        return;
                    }

                    // 有食物供给
                    Dictionary<Type, InventoryItemData> foodSupply = new Dictionary<Type, InventoryItemData>();
                    if (Map.Inventory.CanRemoveWithTag<FoodSupply>(foodSupply, foodSupplyCost) < foodSupplyCost) {
                        UIPreset.ResourceInsufficientWithTag<FoodSupply>(OnTap, foodSupplyCost, Map.Inventory);
                        return;
                    }

                    // 有空间放这些食物
                    if (!Inventory.CanAddWithTag<FoodSupply>(foodSupply, foodSupplyCost)) {
                        UIPreset.InventoryFull(OnTap, Inventory);
                        return;
                    }

                    // 改变工人
                    Map.Inventory.Add<Worker>(workerRevenue);
                    // 改变食物供应
                    Inventory.AddFromWithTag<FoodSupply>(Map.Inventory, foodSupplyCost);

                    level.Max++;
                    OnTap();
                }, () => level.Max < levelMax));


                items.Add(UIItem.CreateButton("停止供应食物", () => {
                    // 有空间放工人。凭空消失

                    // 有工人
                    Dictionary<Type, InventoryItemData> workerSupply = new Dictionary<Type, InventoryItemData>();
                    if (Map.Inventory.CanRemoveWithTag<Worker>(workerSupply, workerRevenue) < workerRevenue) {
                        UIPreset.ResourceInsufficient<Worker>(OnTap, workerRevenue, Map.Inventory);
                    }

                    // 有食物供给
                    Dictionary<Type, InventoryItemData> foodSupply = new Dictionary<Type, InventoryItemData>();
                    if (Inventory.CanRemoveWithTag<FoodSupply>(foodSupply, foodSupplyCost) < foodSupplyCost) {
                        UIPreset.ResourceInsufficient<FoodSupply>(OnTap, workerRevenue, Inventory);
                    }

                    // 能够存放这些食物
                    if (!Map.Inventory.CanAddWithTag<FoodSupply>(foodSupply, foodSupplyCost)) {
                        UIPreset.InventoryFull(OnTap, Inventory);
                    }

                    Map.Inventory.RemoveWithTag<Worker>(workerRevenue, workerSupply, null);
                    Map.Inventory.AddFromWithTag<FoodSupply>(Inventory, foodSupplyCost);

                    level.Max--;
                    OnTap();
                }, () => level.Max > 0));
            }



            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateDestructButton<Grassland>(this, () => level.Max <= 0));

            UIItem.AddEntireInventory(Inventory, items, OnTap);
            UI.Ins.ShowItems(title, items);
        }
    }
}

