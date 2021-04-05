

namespace Weathering
{
    public class MapOfStarSystemDefaultTile : StandardTile, IDontSave, ITileDescription, IHasFrameAnimationOnSpriteKey
    {
        public bool DontSave => true;
        public string TileDescription => isStar ? "【恒星】" : null;


        public const int planetDensity = 50;
        public const int starDensity = 200;

        private bool isPlanet => HashCode % planetDensity == 0;
        private bool isStar {
            get {
                if (Map is IStarPosition pos) {
                    return (pos.X == Pos.x && pos.Y == Pos.y) || (pos.HasSecondStar && pos.SecondStarX == Pos.x && pos.SecondStarY == Pos.y);
                } else {
                    throw new System.Exception();
                }
            }
        }
        public override string SpriteKey => isPlanet ? PlanetSpriteKey : (isStar ? StarSpriteKey : null);
        private string StarSpriteKey { 
            get {
                return "Star";
            } 
        }
        private string PlanetSpriteKey {
            get {
                return "Planet";
            }
        }
        public bool HasFrameAnimation => isPlanet || isStar;


        public override void OnTap() {
            var items = UI.Ins.GetItems();
            string title = $"{typeof(MapOfPlanet).Name}#{Pos.x},{Pos.y}";

            if (isStar) {
                items.Add(UIItem.CreateText("好大一颗恒星"));
            } else if (isPlanet) {

                items.Add(UIItem.CreateButton($"进入{title}", () => {
                    GameEntry.Ins.EnterChildMap(typeof(MapOfPlanet), Map, Pos);
                }));
            } else {

                items.Add(UIItem.CreateButton($"离开恒星系 {(Map as IMapDefinition).MapKey}", () => {
                    GameEntry.Ins.EnterParentMap(typeof(MapOfGalaxy), Map);
                }));
            }
            UI.Ins.ShowItems(title, items);
        }

    }
}
