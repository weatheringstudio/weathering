

using System;

namespace Weathering
{
    public class MapOfGalaxyDefaultTile : StandardTile, IDontSave, ITileDescription, IHasFrameAnimationOnSpriteKey
    {
        public string TileDescription => isStar ? "【恒星系】" : null;
        public bool DontSave => true;


        public const int starSystemDensity = 100;
        private bool isStar;
        public override string SpriteKey => isStar ? StarSpriteKey : null;
        private string StarSpriteKey => $"{StarTypeName}_{(inversedAnimation * MapView.Ins.AnimationIndex / slowedAnimation + HashCode) % 64}";

        public bool HasFrameAnimation => isStar;

        private Type StarType;
        private string StarTypeName;
        private int inversedAnimation = 1;
        private int slowedAnimation = 1;

        public override void OnEnable() {
            isStar = HashCode % starSystemDensity == 0;
            if (isStar) {
                uint hashcode = GameEntry.ChildMapKeyHashCode(Map, Pos);
                StarType = MapOfStarSystemDefaultTile.CalculateStarType(hashcode);
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
                    GameEntry.Ins.EnterChildMap(typeof(MapOfStarSystem), Map, Pos);
                }));
            } else {

                items.Add(UIItem.CreateButton($"离开星系 {(Map as IMapDefinition).MapKey}", () => {
                    GameEntry.Ins.EnterParentMap(typeof(MapOfUniverse), Map);
                }));
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}
