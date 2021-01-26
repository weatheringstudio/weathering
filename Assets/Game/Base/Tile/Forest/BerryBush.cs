


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

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            berry = Values.Create<Berry>();
            berry.Max = foodMax;
            berry.Inc = foodInc;
            berry.Del = 10 * Value.Second;
        }

        private const long foodInc = 1;
        private const long foodMax = 10;

        public override void OnEnable() {
            base.OnEnable();
            berry = Values.Get<Berry>();
        }

        private const long foodSupply = 1;

        public override void OnTap() {
            if (berry.Inc != 0) {
                UI.Ins.ShowItems(TileName,
                    UIItem.CreateText("正在等待浆果成熟"),
                    UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Berry>()}", GatherFood, () => berry.Val > 0),
                    UIItem.CreateValueProgress<Berry>(Values),
                    UIItem.CreateTimeProgress<Berry>(Values),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"按时采集浆果", () => {
                        if (Map.Inventory.CanAdd<BerrySupply>() < foodSupply) {
                            UIPreset.InventoryFull(OnTap, Map.Inventory);
                            return;
                        }
                        Map.Inventory.Add<BerrySupply>(foodSupply);
                        berry.Max = 0;
                        berry.Inc = 0;
                        OnTap();
                    }),

                    UIItem.CreateSeparator(),
                    UIItem.CreateDestructButton<Forest>(this)
                );
            } else {
                UI.Ins.ShowItems(TileName,
                    UIItem.CreateText("森林里每天都有浆果成熟，提供了稳定的食物供给"),
                    UIItem.CreateText("获得了1食物供给"),
                    UIItem.CreateInventoryItem<BerrySupply>(Map.Inventory, OnTap),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"不再按时采集浆果", () => {
                        if (Map.Inventory.Get<BerrySupply>() < foodSupply) {
                            UIPreset.ResourceInsufficient<BerrySupply>(OnTap, foodSupply, Map.Inventory);
                            return;
                        }
                        Map.Inventory.Remove<BerrySupply>(foodSupply);
                        berry.Max = foodMax;
                        berry.Inc = foodInc;
                        OnTap();
                    }),

                    UIItem.CreateSeparator(),
                    UIItem.CreateDestructButton<Forest>(this)
                );
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

