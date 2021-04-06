

namespace Weathering
{
    [ConstructionCostBase(typeof(GoldCoin), 100, 0)]
    public class MagicSchool : AbstractDecoration
    {
        public override void OnTap() {

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateMultilineText("想当法师？下个版本吧！"));

            items.Add(UIItem.CreateStaticDestructButton<MapOfPlanetDefaultTile>(this, CanDestruct()));

            UI.Ins.ShowItems(Localization.Ins.Get<MagicSchool>(), items);
        }

        public override string SpriteKey => typeof(MagicSchool).Name;

    }
}
