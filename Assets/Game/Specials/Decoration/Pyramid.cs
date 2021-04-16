

namespace Weathering
{
    [ConstructionCostBase(typeof(StoneBrick), 10000, 0)]
    public class Pyramid : AbstractDecoration
    {
        public override void OnTap() {

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateText("海猫之家"));

            items.Add(UIItem.CreateStaticDestructButton(this));

            UI.Ins.ShowItems(Localization.Ins.Get<Pyramid>(), items);
        }

        public override string SpriteKey => typeof(Pyramid).Name;

    }
}
