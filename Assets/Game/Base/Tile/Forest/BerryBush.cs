


using System;
using System.Collections.Generic;

namespace Weathering
{
    public class BerryBush : StandardTile, IDefaultDestruction, ILookLikeRoad
    {
        public override string SpriteKey {
            get {
                if (level.Max == 2) {
                    return producing;
                } else if (level.Max == 1) {
                    if (berry.Maxed) {
                        return TimeUtility.GetFrame(0.2f, 2) == 0 ? producing : notProducing;
                    } else {
                        return producing;
                    }
                }
                return producing;
            }
        }

        public Type DefaultDestruction => typeof(Forest);

        public bool LookLikeRoad => level.Max == 2;

        private string producing = typeof(BerryBush).Name + "Producing";
        private string notProducing = typeof(BerryBush).Name;

        IValue berry;
        IValue level;

        private const long foodInc = 10;
        private const long foodMax = 30;
        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            berry = Values.Create<Berry>();
            berry.Max = foodMax;
            berry.Inc = foodInc;
            berry.Del = 100 * Value.Second;

            level = Values.Create<Level>();
            level.Max = 1;

            Refs = Weathering.Refs.GetOne();
        }

        public override void OnEnable() {
            base.OnEnable();
            berry = Values.Get<Berry>();
            level = Values.Get<Level>();
        }

        public override void OnTap() {
            var inventoryQuery = InventoryQuery.Create(OnTap, Map.Inventory, new List<InventoryQueryItem> {
                    new InventoryQueryItem { Target = Map.Inventory, Quantity = 1, Type = typeof(BerrySupply) }
                });
            var inventoryQueryInversed = inventoryQuery.CreateInversed();

            //if (level.Max == 0) {
            //    var build = InventoryQuery.Create(OnTap, Map.Inventory,
            //        new InventoryQueryItem { Source = Map.Inventory, Quantity = 10, Type = typeof(Food) }
            //    );

            //    var items = new List<IUIItem> {
            //        UIItem.CreateDestructButton<Forest>(this, null, () => Map.Get(Pos).OnTap())
            //        , UIItem.CreateButton($"播种浆果{build.GetDescription()}", () => {
            //            build.TryDo(() => {
            //                level.Max = 1;
            //            });
            //        })
            //    };
            //    UIItem.AddEntireInventoryWithTag<Berry>(Map.Inventory, items, OnTap);
            //    UI.Ins.ShowItems(string.Format(Localization.Ins.Get<StateOfBuilding>(), Localization.Ins.Get<BerryBush>()), items);

            //} else 
            if (level.Max == 1) {
                UI.Ins.ShowItems(string.Format(Localization.Ins.Get<StateOfProducing>(), Localization.Ins.Get<BerryBush>()),
                    UIItem.CreateText("正在等待浆果成熟"),
                    UIItem.CreateValueProgress<Berry>(Values),
                    UIItem.CreateTimeProgress<Berry>(Values),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Berry>()}", GatherFood, () => berry.Val > 0),
                    UIItem.CreateButton($"按时采集浆果{inventoryQuery.GetDescription()}", () => {
                        inventoryQuery.TryDo(() => {
                            berry.Max = long.MaxValue;
                            level.Max = 2;
                        });
                    }),

                    UIItem.CreateSeparator(),
                    UIItem.CreateDestructButton<Forest>(this)
                );

            } else if (level.Max == 2) {
                UI.Ins.ShowItems(string.Format(Localization.Ins.Get<StateOfAutomated>(), Localization.Ins.Get<BerryBush>())
                    , UIItem.CreateText("森林里每天都有浆果成熟，提供了稳定的食物供给")
                    , UIItem.CreateInventoryItem<BerrySupply>(Map.Inventory, OnTap)
                    , UIItem.CreateTimeProgress<Berry>(Values)

                    , UIItem.CreateSeparator()
                    , UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Berry>()}", GatherFood, () => false)
                    , UIItem.CreateButton($"不再按时采集浆果{inventoryQueryInversed.GetDescription()}", () => {
                        inventoryQueryInversed.TryDo(() => {
                            berry.Max = foodMax;
                            berry.Val = 0;
                            level.Max = 1;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateDestructButton<Forest>(this, () => false)
                );
            } else {
                throw new System.Exception();
            }
        }

        private const long gatherFoodSanityCost = 1;
        private void GatherFood() {
            if (Globals.Sanity.Val < gatherFoodSanityCost) {
                UIPreset.ResourceInsufficient<Sanity>(OnTap, gatherFoodSanityCost, Globals.Sanity);
                return;
            }
            if (Map.Inventory.CanAdd<Berry>() <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }

            Globals.Sanity.Val -= gatherFoodSanityCost;
            Map.Inventory.AddFrom<Berry>(berry);
        }
    }
}

