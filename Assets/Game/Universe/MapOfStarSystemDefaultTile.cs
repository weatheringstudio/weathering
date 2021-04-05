

namespace Weathering
{
    public class MapOfStarSystemDefaultTile : StandardTile, IDontSave
    {
        public bool DontSave => true;

        public const int planetDensity = 50;
        public const int starDensity = 200;
        private bool isPlanet => HashCode % planetDensity == 0;
        private bool isStar => HashCode % starDensity == 0;
        public override string SpriteKey => isStar ? "Star" : (isPlanet ? "Planet" : null);

        public override void OnTap() {
            var items = UI.Ins.GetItems();
            string title = $"{typeof(MapOfPlanet).Name}#{Pos.x},{Pos.y}";

            if (isStar) {
                items.Add(UIItem.CreateText("好大一颗恒星"));
            }
            else if (isPlanet) {

                items.Add(UIItem.CreateButton($"进入{title}", () => {
                    GameEntry.Ins.EnterChildMap(typeof(MapOfPlanet), Map, Pos);
                }));
            }
            else {

                items.Add(UIItem.CreateButton("离开恒星系", () => {
                    GameEntry.Ins.EnterParentMap(typeof(MapOfGalaxy), Map);
                }));
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}
