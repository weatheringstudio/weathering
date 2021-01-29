
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    class ForestLoggingCamp : StandardTile
    {
        public override string SpriteKey {
            get {
                if (level.Max == 2) {
                    return producing;
                } else if (level.Max == 1) {
                    if (wood.Maxed) {
                        return TimeUtility.GetFrame(0.2f, 2) == 0 ? producing : notProducing;
                    } else {
                        return producing;
                    }
                }
                return producing;
            }
        }
        private string producing = typeof(ForestLoggingCamp).Name + "Producing";
        private string notProducing = typeof(ForestLoggingCamp).Name;

        private const long woodInc = 1;
        private IValue wood;

        private IValue level;


        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            level = Values.Create<Level>();
            level.Max = -1;

            wood = Values.Create<Wood>();
            wood.Max = 300;
            wood.Del = 12 * Value.Second;
            wood.Inc = 0;

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 5;
            Inventory.TypeCapacity = 5;
        }
        public override void OnEnable() {
            base.OnEnable();
            level = Values.Get<Level>();
            wood = Values.Get<Wood>();

        }


        public override void OnTap() {
            string title = "";

            InventoryQuery logCost = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = 1, Type = typeof(Worker), Source = Map.Inventory, Target = Inventory }
                );
            InventoryQuery logRevenue = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = woodInc, Type = typeof(WoodSupply), Target = Map.Inventory }
                );

            InventoryQuery logCostInvsersed = logCost.CreateInversed();
            InventoryQuery logRevenueInversed = logRevenue.CreateInversed();


            var items = new List<IUIItem>() { };

            if (level.Max == -1) {
                title = string.Format(Localization.Ins.Get<StateOfBuilding>(), string.Format(Localization.Ins.Get<ForestLoggingCamp>()));

                InventoryQuery build = InventoryQuery.Create(OnTap, Map.Inventory
                    , new InventoryQueryItem { Quantity = 10, Type = typeof(Wood), Source = Map.Inventory });

                items.Add(UIItem.CreateText("伐木场还没造好"));
                items.Add(UIItem.CreateButton($"造好{build.GetDescription()}", () => {
                    build.TryDo(() => {
                        level.Max = 0;
                    });
                }));

            } else if (level.Max == 0) {
                items.Add(UIItem.CreateButton($"派遣居民伐木{logCost.GetDescription()}", () => {
                    logCost.TryDo(() => {
                        level.Max = 1;
                        wood.Inc = woodInc;
                    });
                }));

            } else if (level.Max == 1) {
                items.Add(UIItem.CreateText("森林里有各种木材"));

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Wood>()}", GatherWood, () => wood.Val > 0));
                items.Add(UIItem.CreateValueProgress<Wood>(Values));
                items.Add(UIItem.CreateTimeProgress<Wood>(Values));

                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"取消居民种田{logCostInvsersed.GetDescription()}", () => {
                    logCostInvsersed.TryDo(() => {
                        level.Max = 0;
                    });
                }));

                items.Add(UIItem.CreateButton($"开始自动供应木材{logRevenue.GetDescription()}", () => {
                    logRevenue.TryDo(() => {
                        wood.Inc = 0;
                        wood.Val = 0;
                        level.Max = 2;
                    });
                }));
            } else if (level.Max == 2) {
                items.Add(UIItem.CreateText("一车车木材丛森林里运了出来"));
                items.Add(UIItem.CreateTimeProgress<Wood>(wood));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateButton($"停止自动供应木材{logRevenueInversed.GetDescription()}", () => {
                    logRevenueInversed.TryDo(() => {
                        wood.Inc = woodInc;
                        level.Max = 1;
                    });
                }));
            }

            items.Add(UIItem.CreateDestructButton<Grassland>(this, () => level.Max <= 0));

            UI.Ins.ShowItems(Localization.Ins.Get<Farm>(), items);
        }

        private const long gatherSanityCost = 1;
        private void GatherWood() {
            if (Globals.Sanity.Val < gatherSanityCost) {
                UIPreset.ResourceInsufficient<Sanity>(OnTap, gatherSanityCost, Globals.Sanity);
                return;
            }
            if (Map.Inventory.CanAdd<Wood>() <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }
            Globals.Sanity.Val -= gatherSanityCost;
            Map.Inventory.AddFrom<Wood>(wood);
        }
    }
}

