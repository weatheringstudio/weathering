

namespace Weathering
{
    [ConstructionCostBase(typeof(WoodPlank), 100, 0)]
    public class Torii : AbstractDecoration
    {
        public override void OnTap() {

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateMultilineText("踏入鸟居即意味着进入神域，之后所有的行为举止都应特别注意。"));

            items.Add(UIItem.CreateButton("神隐", () => {
                PlanetLander.Ins.LeavePlanet();
            }));

            items.Add(UIItem.CreateStaticDestructButton(this));

            UI.Ins.ShowItems(Localization.Ins.Get<Torii>(), items);
        }

        public override string SpriteKey => typeof(Torii).Name;

    }
}
