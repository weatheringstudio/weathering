
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

            InventoryQuery query = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity=foodSupplyCost, Type=typeof(FoodSupply), Source=Map.Inventory, Target=Inventory}
                , new InventoryQueryItem { Quantity=workerRevenue, Type=typeof(Worker), Target = Map.Inventory});

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

                items.Add(UIItem.CreateButton($"开始供应居民{Localization.Ins.Val<FoodSupply>(-foodSupplyCost)}{Localization.Ins.Val<Worker>(workerRevenue)}", () => {

                    if (!query.CanDo()) {
                        return;
                    }

                    query.Ask(() => {
                        level.Max++;
                    });

                    //var items2 = new List<IUIItem>();
                    //foreach (var pair in foodSupply) {
                    //    items2.Add(UIItem.CreateText(string.Format(Localization.Ins.Get(pair.Key), pair.Value.value)));
                    //}
                    //items2.Add(UIItem.CreateButton("取消", OnTap));
                    //items2.Add(UIItem.CreateButton("确认", () => {
                    //    // 改变工人
                    //    Map.Inventory.Add<Worker>(workerRevenue);
                    //    // 改变食物供应
                    //    Inventory.AddFromWithTag<FoodSupply>(Map.Inventory, foodSupplyCost);

                    //    level.Max++;

                    //    UIPreset.Notify(OnTap, $"获得{Localization.Ins.Val<Worker>(1)}");
                    //}));

                    //UI.Ins.ShowItems("是否提供以下资源？", items2);


                }, () => level.Max < levelMax));


                items.Add(UIItem.CreateButton($"停止供应居民{Localization.Ins.Val<FoodSupply>(foodSupplyCost)}{Localization.Ins.Val<Worker>(-workerRevenue)}", () => {

                    // 有空间放工人。凭空消失

                    // 有工人。工人子类也行！这种转换
                    Dictionary<Type, InventoryItemData> workerSupply = new Dictionary<Type, InventoryItemData>();
                    if (Map.Inventory.CanRemoveWithTag<Worker>(workerSupply, workerRevenue) < workerRevenue) {
                        UIPreset.ResourceInsufficient<Worker>(OnTap, workerRevenue, Map.Inventory);
                    }

                    // 有食物供给。其实不用检验，肯定有
                    Dictionary<Type, InventoryItemData> foodSupply = new Dictionary<Type, InventoryItemData>();
                    if (Inventory.CanRemoveWithTag<FoodSupply>(foodSupply, foodSupplyCost) < foodSupplyCost) {
                        UIPreset.ResourceInsufficient<FoodSupply>(OnTap, workerRevenue, Inventory);
                    }

                    // 能够存放这些食物
                    if (!Map.Inventory.CanAddWithTag(foodSupply, foodSupplyCost)) {
                        UIPreset.InventoryFull(OnTap, Inventory);
                    }

                    Map.Inventory.RemoveWithTag<Worker>(workerRevenue, workerSupply, null);
                    Map.Inventory.AddFromWithTag<FoodSupply>(Inventory, foodSupplyCost);

                    level.Max--;
                    OnTap();

                }, () => level.Max > 0));
            }

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateText(Localization.Ins.Get<Village>()));

            UIItem.AddEntireInventory(Inventory, items, OnTap);

            items.Add(UIItem.CreateDestructButton<Grassland>(this, () => level.Max <= 0));

            UI.Ins.ShowItems(Localization.Ins.Get<Village>(), items);
        }
    }
}

