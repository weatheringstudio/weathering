

namespace Weathering
{
    [ConstructionCostBase(typeof(StoneBrick), 100, 0)]
    public class WallOfStoneBrick : StandardTile
    {
        public override void OnTap() {

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateText("墙有什么用呢？答案：把南北分开、把东西分开、把内外分开"));

            items.Add(UIItem.CreateStaticDestructButton<MapOfPlanetDefaultTile>(this, CanDestruct()));

            UI.Ins.ShowItems("墙", items);
        }

        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(this, (tile, direction) => tile is WallOfStoneBrick);
                return $"Wall_{index}";
            }
        }

        public override bool CanDestruct() {
            return true;
        }
    }
}
