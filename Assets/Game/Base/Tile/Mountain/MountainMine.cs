
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class MountainMine : StandardTile, IPassable
    {
        public override string SpriteKey {
            get {
                if (level.Max == 2) {
                    return producing;
                } else if (level.Max == 1) {
                    if (ore.Maxed) {
                        return TimeUtility.GetFrame(0.2f, 2) == 0 ? producing : notProducing;
                    } else {
                        return producing;
                    }
                }
                return producing;
            }
        }

        public bool Passable => true;

        private string producing = typeof(MountainMine).Name + "Producing";
        private string notProducing = typeof(MountainMine).Name;


        private const long workerDec = 1;
        private const long ironInc = 1;

        private void GotoLevel(long i) {
            switch (i) {
                case 0:
                    ore.Max = 10;
                    ore.Inc = ironInc;
                    ore.Del = 3 * Value.Minute;
                    break;
                case 1:
                    ore.Val = 0;
                    ore.Max = 100;
                    ore.Inc = ironInc;
                    ore.Del = 20 * Value.Second;
                    if (level.Max == 2) {
                        ore.Val = 0;
                    }
                    break;
                case 2:
                    ore.Max = long.MaxValue;
                    ore.Inc = ironInc;
                    ore.Del = 20 * Value.Second;
                    break;
                default:
                    throw new Exception();
            }
            level.Max = i;
        }

        private IValue ore;
        private IValue level;

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            level = Values.Create<Level>();
            ore = Values.Create<MetalOre>();
            GotoLevel(0);

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 5;
            Inventory.TypeCapacity = 5;
        }

        public override void OnEnable() {
            base.OnEnable();
            level = Values.Get<Level>();
            ore = Values.Get<MetalOre>();
        }


        public override void OnTap() {
            InventoryQuery quarryCost = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = workerDec, Type = typeof(Worker), Source = Map.Inventory, Target = Inventory }
                );
            InventoryQuery quarryRevenue = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = ironInc, Type = typeof(MetalOreSupply), Target = Map.Inventory }
                );

            InventoryQuery quarryCostInvsersed = quarryCost.CreateInversed();
            InventoryQuery quarryRevenueInversed = quarryRevenue.CreateInversed();


            var items = new List<IUIItem>();


            if (level.Max == 0) {
                items.Add(UIItem.CreateText("山里有很多矿石"));
                items.Add(UIItem.CreateText("【在目前版本中，金属矿石没有任何作用】"));
                items.Add(UIItem.CreateValueProgress<MetalOre>(Values));
                items.Add(UIItem.CreateTimeProgress<MetalOre>(Values));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<MetalOre>()}"
                    , GatherMetalOre, () => ore.Val > 0));
                items.Add(UIItem.CreateButton($"派遣居民采矿{quarryCost.GetDescription()}", () => {
                    quarryCost.TryDo(() => {
                        GotoLevel(1);
                    });
                }));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));
            }
            else if (level.Max == 1) {
                items.Add(UIItem.CreateText("居民在开采矿石"));
                items.Add(UIItem.CreateValueProgress<MetalOre>(Values));
                items.Add(UIItem.CreateTimeProgress<MetalOre>(Values));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<MetalOre>()}"
                    , GatherMetalOre, () => ore.Val > 0));
                items.Add(UIItem.CreateButton($"取消居民采矿{quarryCostInvsersed.GetDescription()}", () => {
                    quarryCostInvsersed.TryDo(() => {
                        GotoLevel(0);
                    });
                }));

                items.Add(UIItem.CreateButton($"开始自动供应采矿{quarryRevenue.GetDescription()}", () => {
                    quarryRevenue.TryDo(() => {
                        GotoLevel(2);
                    });
                }));
            } else if (level.Max == 2) {
                items.Add(UIItem.CreateText("一车车矿石从山里运了出来"));
                items.Add(UIItem.CreateInventoryItem<MetalOreSupply>(Map.Inventory, OnTap));
                items.Add(UIItem.CreateTimeProgress<MetalOre>(Values));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<MetalOre>()}", GatherMetalOre, () => false));
                items.Add(UIItem.CreateButton($"停止自动供应石材{quarryRevenueInversed.GetDescription()}", () => {
                    quarryRevenueInversed.TryDo(() => {
                        GotoLevel(1);
                    });
                }));
            }

            UI.Ins.ShowItems(Localization.Ins.Get<MountainQuarry>(), items);
        }

        private void GatherMetalOre() {
            Globals.SanityCheck();

            if (Map.Inventory.CanAdd<MetalOre>() <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }
            Map.Inventory.AddFrom<MetalOre>(ore);
        }
    }
}

