

using System;

namespace Weathering
{
    public interface IStarType { }

    public class StarBlue : IStarType { }
    public class StarWhite : IStarType { }
    public class StarYellow : IStarType { }
    public class StarOrange : IStarType { }
    public class StarRed : IStarType { }

    public class MapOfStarSystemDefaultTile : StandardTile, IDontSave, ITileDescription, IHasFrameAnimationOnSpriteKey
    {
        public static Type CalculateStarType(uint hashcode) {
            Type result;
            switch (hashcode % 5) {
                case 0:
                    result = typeof(StarBlue);
                    break;
                case 1:
                    result = typeof(StarWhite);
                    break;
                case 2:
                    result = typeof(StarYellow);
                    break;
                case 3:
                    result = typeof(StarOrange);
                    break;
                case 4:
                    result = typeof(StarRed);
                    break;
                default:
                    throw new Exception();
            }
            return result;
        }


        public bool DontSave => true;
        public string TileDescription => isStar ? "【恒星】" : null;


        public const int planetDensity = 50;
        public const int starDensity = 200;

        private bool isPlanet;
        private bool isStar;
        public override string SpriteKey => isPlanet ? PlanetSpriteKey : (isStar ? StarSpriteKey : null);
        private string StarSpriteKey {
            get {
                return $"{StarTypeName}_{(inversedAnimation * MapView.Ins.AnimationIndex / slowedAnimation + HashCode) % 64}";
            }
        }
        private string PlanetSpriteKey {
            get {
                return $"PlanetWet_{(inversedAnimation * MapView.Ins.AnimationIndex / slowedAnimation + HashCode) % 64}";
            }
        }
        public bool HasFrameAnimation => isPlanet || isStar;

        private int inversedAnimation = 1;
        private int slowedAnimation = 1;

        public Type StarType { get; private set; }
        public string StarTypeName { get; private set; }
        public override void OnEnable() {
            isPlanet = HashCode % planetDensity == 0;

            uint starHashcode = 0;
            if (Map is IStarPosition pos) {
                isStar = (pos.X == Pos.x && pos.Y == Pos.y) || (pos.HasSecondStar && pos.SecondStarX == Pos.x && pos.SecondStarY == Pos.y);
                if (isStar) {
                    starHashcode = GameEntry.SelfMapKeyHashCode(Map);
                    StarType = CalculateStarType(starHashcode);
                    StarTypeName = StarType.Name;
                }
            } else {
                throw new Exception();
            }
            if (HasFrameAnimation) {
                uint again = HashUtility.Hash(isStar ? starHashcode : HashCode);
                inversedAnimation = again % 2 == 0 ? 1 : -1;
                again = HashUtility.Hash(again);
                slowedAnimation = 1 + ABS((int)again % 5);
            }
        }
        private int ABS(int x) => x >= 0 ? x : -x;

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
