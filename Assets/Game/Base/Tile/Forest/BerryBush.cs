


namespace Weathering
{
    public class BerryBush : StandardTile
    {
        public override string SpriteKey {
            get {
                if (!food.Maxed) {
                    return typeof(GrasslandBerryBush).Name + "Growing";
                }
                return typeof(GrasslandBerryBush).Name;
            }
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            food = Values.Create<Food>();
            food.Max = 10;
            food.Inc = 1;
            food.Del = 10 * Value.Second;
        }

        IValue food;

        public override void OnEnable() {
            base.OnEnable();
            food = Values.Get<Food>();
        }

        public override void OnTap() {
            UI.Ins.ShowItems(TileName,
                UIItem.CreateText("森林里有不少浆果"),
                UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}", GatherFood),
                UIItem.CreateValueProgress<Food>(Values),
                UIItem.CreateTimeProgress<Food>(Values),

                UIItem.CreateSeparator(),
                UIItem.CreateDestructButton<Forest>(this)
            );
        }

        private const long gatherFoodSanityCost = 1;
        private void GatherFood() {
            if (Globals.Sanity.Val < gatherFoodSanityCost) {
                UIPreset.ResourceInsufficient<Sanity>(OnTap, gatherFoodSanityCost, Globals.Sanity);
            }
            long canAdd = Map.Inventory.CanAdd<Food>();
            if (canAdd <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
            }

            Globals.Sanity.Val -= gatherFoodSanityCost;
            Map.Inventory.AddAsManyAsPossible<Food>(food);
        }
    }
}

