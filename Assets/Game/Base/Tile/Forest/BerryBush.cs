


using System.Collections.Generic;

namespace Weathering
{
    public class BerryBush : StandardTile
    {
        public override string SpriteKey {
            get {
                if (!berry.Maxed || berry.Max == 0) {
                    return typeof(BerryBush).Name + "Producing";
                }
                return typeof(BerryBush).Name;
            }
        }

        IValue berry;
        IValue level;

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            berry = Values.Create<Berry>();
            berry.Max = foodMax;
            berry.Inc = foodInc;
            berry.Del = 10 * Value.Second;

            level = Values.Create<Level>();
        }

        private const long foodInc = 1;
        private const long foodMax = 10;

        public override void OnEnable() {
            base.OnEnable();
            berry = Values.Get<Berry>();
            level = Values.Get<Level>();
        }

        public override void OnTap() {
            var inventoryQuery = InventoryQuery.Create(OnTap, Map.Inventory, new List<InventoryQueryItem> {
                    new InventoryQueryItem { Source = Map.Inventory, Quantity = 1, Type = typeof(BerrySupply) }
                });
            var inventoryQueryInversed = inventoryQuery.CreateInversed();

            if (level.Max == 0) {
                var build = InventoryQuery.Create(OnTap, Map.Inventory,
                    new InventoryQueryItem { Target = Map.Inventory, Quantity = 10, Type = typeof(Berry) }
                );

                UI.Ins.ShowItems(Localization.Ins.Get<BerryBush>()
                    , UIItem.CreateButton($"播种浆果{build.GetDescription()}", () => {
                        build.TryDo(() => {
                            level.Max = 1;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateDestructButton<Forest>(this)
                );
            } else if (level.Max == 1) {
                UI.Ins.ShowItems(Localization.Ins.Get<BerryBush>(),
                    UIItem.CreateText("正在等待浆果成熟"),
                    UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Berry>()}", GatherFood, () => berry.Val > 0),
                    UIItem.CreateValueProgress<Berry>(Values),
                    UIItem.CreateTimeProgress<Berry>(Values),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"按时采集浆果{inventoryQuery.GetDescription()}", () => {
                        inventoryQuery.TryDo(() => {
                            berry.Max = 0;
                            berry.Inc = 0;
                            level.Max = 2;
                        });
                    }),

                    UIItem.CreateSeparator(),
                    UIItem.CreateDestructButton<Forest>(this)
                );
            } else if (level.Max == 2) {
                UI.Ins.ShowItems(Localization.Ins.Get<BerryBush>(),
                    UIItem.CreateText("森林里每天都有浆果成熟，提供了稳定的食物供给"),
                    UIItem.CreateButton($"不再按时采集浆果{inventoryQueryInversed.GetDescription()}", () => {
                        inventoryQueryInversed.TryDo(() => {
                            berry.Max = foodMax;
                            berry.Inc = foodInc;
                            level.Max = 1;
                        });
                    }),

                    UIItem.CreateSeparator(),
                    UIItem.CreateDestructButton<Forest>(this)
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
            if (Map.Inventory.CanAdd<Food>() <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }

            Globals.Sanity.Val -= gatherFoodSanityCost;
            Map.Inventory.AddFrom<Food>(berry);
        }
    }
}

