


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
                UIItem.CreateDestructButton<Forest>(this)
            );
        }
    }
}

