
using System;
using System.Collections.Generic;

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


        private const long workerDec = 1;
        private const long woodInc = 1;

        private void GotoLevel(long i) {
            switch (i) {
                case 0:
                    wood.Max = 10;
                    wood.Inc = woodInc;
                    wood.Del = 1 * Value.Minute;
                    break;
                case 1:
                    wood.Max = 100;
                    wood.Inc = woodInc;
                    wood.Del = 12 * Value.Second;
                    if (level.Max == 2) {
                        wood.Val = 0;
                    }
                    break;
                case 2:
                    wood.Max = long.MaxValue;
                    wood.Inc = woodInc;
                    wood.Del = 12 * Value.Second;
                    break;
                default:
                    throw new Exception();
            }
            level.Max = i;
        }

        private IValue wood;
        private IValue level;

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            level = Values.Create<Level>();
            wood = Values.Create<Wood>();
            GotoLevel(0);

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

            InventoryQuery logCost = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = workerDec, Type = typeof(Worker), Source = Map.Inventory, Target = Inventory }
                );
            InventoryQuery logRevenue = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = woodInc, Type = typeof(WoodSupply), Target = Map.Inventory }
                );

            InventoryQuery logCostInvsersed = logCost.CreateInversed();
            InventoryQuery logRevenueInversed = logRevenue.CreateInversed();


            var items = new List<IUIItem>() { };


            if (level.Max == 0) {
                items.Add(UIItem.CreateText("森林里地上有些小树枝"));
                items.Add(UIItem.CreateValueProgress<Wood>(Values));
                items.Add(UIItem.CreateTimeProgress<Wood>(Values));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Wood>()}"
                    , GatherWood, () => wood.Val > 0));
                items.Add(UIItem.CreateButton($"派遣居民伐木{logCost.GetDescription()}", () => {
                    logCost.TryDo(() => {
                        GotoLevel(1);
                    });
                }));

            } else if (level.Max == 1) {
                items.Add(UIItem.CreateText("居民在拿石头砸树干"));
                items.Add(UIItem.CreateValueProgress<Wood>(Values));
                items.Add(UIItem.CreateTimeProgress<Wood>(Values));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Wood>()}"
                    , GatherWood, () => wood.Val > 0));
                items.Add(UIItem.CreateButton($"取消居民伐木{logCostInvsersed.GetDescription()}", () => {
                    logCostInvsersed.TryDo(() => {
                        GotoLevel(0);
                    });
                }));

                items.Add(UIItem.CreateButton($"开始自动供应木材{logRevenue.GetDescription()}", () => {
                    logRevenue.TryDo(() => {
                        GotoLevel(2);
                    });
                }));
            } else if (level.Max == 2) {
                items.Add(UIItem.CreateText("一车车木材丛森林里运了出来"));
                items.Add(UIItem.CreateInventoryItem<Wood>(Map.Inventory, OnTap));
                items.Add(UIItem.CreateTimeProgress<Wood>(Values));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Wood>()}", GatherWood, () => false));
                items.Add(UIItem.CreateButton($"停止自动供应木材{logRevenueInversed.GetDescription()}", () => {
                    logRevenueInversed.TryDo(() => {
                        GotoLevel(1);
                    });
                }));
            }

            items.Add(UIItem.CreateDestructButton<Forest>(this, () => level.Max <= 0));

            UI.Ins.ShowItems(Localization.Ins.Get<ForestLoggingCamp>(), items);
        }

        private void GatherWood() {
            Globals.SanityCheck();

            if (Map.Inventory.CanAdd<Wood>() <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }
            Map.Inventory.AddFrom<Wood>(wood);
        }
    }
}

