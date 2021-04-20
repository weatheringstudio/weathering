

using System;

namespace Weathering
{
    public interface IStarType : ICelestialBodyType { }

    public class StarBlue : IStarType { }
    public class StarWhite : IStarType { }
    public class StarYellow : IStarType { }
    public class StarOrange : IStarType { }
    public class StarRed : IStarType { }


    public class MapOfGalaxyDefaultTile : StandardTile, IDontSave, ITileDescription, IHasFrameAnimationOnSpriteKey
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


        public string TileDescription => isStar ? "【恒星系】" : null;
        public bool DontSave => true;


        public const int starSystemDensity = 200;
        private bool isStar;
        public override string SpriteKey => isStar ? StarSpriteKey : null;
        private string StarSpriteKey => $"{StarTypeName}_{(inversedAnimation * MapView.Ins.AnimationIndex + TileHashCode) % 64}";

        public int HasFrameAnimation => isStar ? slowedAnimation : 0;

        private Type StarType;
        private string StarTypeName;
        private int inversedAnimation = 1;
        private int slowedAnimation = 1;

        public override void OnEnable() {
            isStar = TileHashCode % starSystemDensity == 0;
            if (isStar) {
                uint hashcode = GameEntry.ChildMapKeyHashCode(Map, Pos);
                StarType = CalculateStarType(hashcode);
                StarTypeName = StarType.Name;


                uint again = HashUtility.Hash(hashcode);
                inversedAnimation = again % 2 == 0 ? 1 : -1;
                again = HashUtility.Hash(again);
                slowedAnimation = 1 + ABS((int)again % 5);
            }
        }
        private int ABS(int x) => x >= 0 ? x : -x;

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            string title = $"{typeof(MapOfStarSystem).Name}#{Pos.x},{Pos.y}";
            if (isStar) {

                items.Add(UIItem.CreateButton($"进入{title}", () => {
                    Map.EnterChildMap(Pos);
                }));
            } else {

                items.Add(UIItem.CreateButton($"离开此银河系", () => {
                    Map.EnterParentMap();
                }));
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}
