
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class MountainQuarry : StandardTile
    {
        public override string SpriteKey {
            get {
                if (level.Max == 2) {
                    return producing;
                } else if (level.Max == 1) {
                    if (stone.Maxed) {
                        return TimeUtility.GetFrame(0.2f, 2) == 0 ? producing : notProducing;
                    } else {
                        return producing;
                    }
                }
                return producing;
            }
        }
        private string producing = typeof(MountainQuarry).Name + "Producing";
        private string notProducing = typeof(MountainQuarry).Name;


        private const long workerDec = 1;
        private const long stoneInc = 1;

        private void GotoLevel(long i) {
            switch (i) {
                case 0:
                    stone.Max = 10;
                    stone.Inc = stoneInc;
                    stone.Del = 2 * Value.Minute;
                    break;
                case 1:
                    stone.Val = 0;
                    stone.Max = 100;
                    stone.Inc = stoneInc;
                    stone.Del = 15 * Value.Second;
                    if (level.Max == 2) {
                        stone.Val = 0;
                    }
                    break;
                case 2:
                    stone.Max = long.MaxValue;
                    stone.Inc = stoneInc;
                    stone.Del = 15 * Value.Second;
                    break;
                default:
                    throw new Exception();
            }
            level.Max = i;
        }

        private IValue stone;
        private IValue level;

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            level = Values.Create<Level>();
            stone = Values.Create<Stone>();
            GotoLevel(0);

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 5;
            Inventory.TypeCapacity = 5;
        }

        public override void OnEnable() {
            base.OnEnable();
            level = Values.Get<Level>();
            stone = Values.Get<Stone>();
        }


        public override void OnTap() {
            InventoryQuery quarryCost = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = workerDec, Type = typeof(Worker), Source = Map.Inventory, Target = Inventory }
                );
            InventoryQuery quarryRevenue = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = stoneInc, Type = typeof(StoneSupply), Target = Map.Inventory }
                );

            InventoryQuery quarryCostInvsersed = quarryCost.CreateInversed();
            InventoryQuery quarryRevenueInversed = quarryRevenue.CreateInversed();


            var items = new List<IUIItem>();


            if (level.Max == 0) {
                items.Add(UIItem.CreateText("山里有很多石材"));
                items.Add(UIItem.CreateText("【在目前版本中，石材没有任何作用】"));
                items.Add(UIItem.CreateValueProgress<Stone>(Values));
                items.Add(UIItem.CreateTimeProgress<Stone>(Values));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Stone>()}"
                    , GatherStone, () => stone.Val > 0));
                items.Add(UIItem.CreateButton($"派遣居民采石{quarryCost.GetDescription()}", () => {
                    quarryCost.TryDo(() => {
                        GotoLevel(1);
                    });
                }));
            }
            else if (level.Max == 1) {
                items.Add(UIItem.CreateText("居民在开采石材"));
                items.Add(UIItem.CreateValueProgress<Stone>(Values));
                items.Add(UIItem.CreateTimeProgress<Stone>(Values));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Stone>()}"
                    , GatherStone, () => stone.Val > 0));
                items.Add(UIItem.CreateButton($"取消居民采石{quarryCostInvsersed.GetDescription()}", () => {
                    quarryCostInvsersed.TryDo(() => {
                        GotoLevel(0);
                    });
                }));

                items.Add(UIItem.CreateButton($"开始自动供应采石{quarryRevenue.GetDescription()}", () => {
                    quarryRevenue.TryDo(() => {
                        GotoLevel(2);
                    });
                }));
            } else if (level.Max == 2) {
                items.Add(UIItem.CreateText("一车车石材从山里运了出来"));
                items.Add(UIItem.CreateInventoryItem<StoneSupply>(Map.Inventory, OnTap));
                items.Add(UIItem.CreateTimeProgress<Stone>(Values));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Stone>()}", GatherStone, () => false));
                items.Add(UIItem.CreateButton($"停止自动供应木材{quarryRevenueInversed.GetDescription()}", () => {
                    quarryRevenueInversed.TryDo(() => {
                        GotoLevel(1);
                    });
                }));
            }

            UI.Ins.ShowItems(Localization.Ins.Get<MountainQuarry>(), items);
        }

        private void GatherStone() {
            Globals.SanityCheck();

            if (Map.Inventory.CanAdd<Stone>() <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }
            Map.Inventory.AddFrom<Stone>(stone);
        }
    }
}

