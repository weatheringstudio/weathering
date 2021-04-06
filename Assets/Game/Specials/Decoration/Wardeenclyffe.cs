

namespace Weathering
{
    [ConstructionCostBase(typeof(MachinePrimitive), 1000, 0)]
    public class Wardeenclyffe : AbstractDecoration, IHasFrameAnimationOnSpriteKey
    {
        public override void OnTap() {

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateMultilineText("想当法师？下个版本吧！"));

            items.Add(UIItem.CreateStaticDestructButton<MapOfPlanetDefaultTile>(this, CanDestruct()));

            UI.Ins.ShowItems(Localization.Ins.Get<MagicSchool>(), items);
        }

        public override string SpriteKey => $"{typeof(Wardeenclyffe).Name}{MapView.Ins.AnimationIndex % 6}";

        public bool HasFrameAnimation => true;
    }
}
