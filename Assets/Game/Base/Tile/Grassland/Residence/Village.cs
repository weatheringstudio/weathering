
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
            Inventory.QuantityCapacity = long.MaxValue;
            Inventory.TypeCapacity = int.MaxValue;
        }

        public override void OnEnable() {
            base.OnEnable();

            level = Values.Get<Level>();
        }

        private const long foodSupplyCost = 2;
        private const long workerRevenue = 1;
        private const long levelMax = 1;

        public override void OnTap() {

            string title = null;

            InventoryQuery build = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = 10, Type = typeof(Wood), Source = Map.Inventory }
                );

            InventoryQuery query = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = foodSupplyCost, Type = typeof(FoodSupply), Source = Map.Inventory, Target = Inventory }
                , new InventoryQueryItem { Quantity = workerRevenue, Type = typeof(Worker), Target = Map.Inventory }
            );
            InventoryQuery queryInversed = query.CreateInversed();


            var items = new List<IUIItem>();

            if (level.Max == -1) {
                title = string.Format(Localization.Ins.Get<StateOfBuilding>(), Localization.Ins.Get<Village>());

                items.Add(UIItem.CreateDestructButton<Grassland>(this));

                // 还没建房子
                items.Add(UIItem.CreateText("村里只有一片地基，还没有房子"));
                items.Add(UIItem.CreateButton($"造房子{build.GetDescription()}", () => {
                    build.TryDo(() => {
                        level.Max = 0;
                    });
                }));
            }

            if (level.Max >= 0) {
                title = string.Format(Localization.Ins.Get<StateOfIdle>(), Localization.Ins.Get<Village>());

                items.Add(UIItem.CreateText($"村民数量 {level.Max}"));

                if (Inventory.Quantity > 0) {
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateText("村庄里的人的食物来源："));
                    UIItem.AddEntireInventoryContent(Inventory, items, OnTap);
                }
            }

            if (level.Max == 0) {
                items.Add(UIItem.CreateButton($"开始供应居民{query.GetDescription()}", () => {
                    query.TryDo(() => {
                        level.Max++;
                    });
                }, () => level.Max < levelMax));
            }

            if (level.Max == 1) {
                items.Add(UIItem.CreateButton($"停止供应居民{queryInversed.GetDescription() }", () => {
                    queryInversed.TryDo(() => {
                        level.Max--;
                    });
                }, () => level.Max > 0));
            }
            
            if (level.Max >= 1) {
                title = Localization.Ins.Get<Village>();
            }

            items.Add(UIItem.CreateDestructButton<Grassland>(this, () => level.Max == 0));
            UI.Ins.ShowItems(Localization.Ins.Get<Village>(), items);
        }
    }
}

