

namespace Weathering
{
    public class MapOfGalaxyDefaultTile : StandardTile, IDontSave
    {
        public bool DontSave => true;

        public const int starSystemDensity = 50;
        private bool isStar => HashCode % starSystemDensity == 0;
        public override string SpriteKey => isStar ? "Star" : null;

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            string title = $"{typeof(MapOfStarSystem).Name}#{Pos.x},{Pos.y}";
            if (isStar) {

                items.Add(UIItem.CreateButton($"进入{title}", () => {
                    GameEntry.Ins.EnterChildMap(typeof(MapOfStarSystem), Map, Pos);
                }));
            } else {

                items.Add(UIItem.CreateButton("离开星系", () => {
                    GameEntry.Ins.EnterParentMap(typeof(MapOfUniverse), Map);
                }));
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}
