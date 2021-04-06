

namespace Weathering
{
    public class MapOfUniverseDefaultTile : StandardTile, IDontSave, ITileDescription
    {
        public string TileDescription => isGalaxy ? "【星系】" : "【虚空】";
        public bool DontSave => true;

        public const int galaxyDensity = 50;
        private bool isGalaxy => HashCode % galaxyDensity == 0;
        public override string SpriteKey => isGalaxy ? "Galaxy" : null;


        public override void OnTap() {
            var items = UI.Ins.GetItems();

            if (isGalaxy) {
                items.Add(UIItem.CreateButton($"进入{typeof(MapOfGalaxy).Name}#{Pos.x},{Pos.y}", () => {
                    Map.EnterChildMap(Pos);
                }));
            }
            else {
                items.Add(UIItem.CreateText("无法离开宇宙"));
            }
            UI.Ins.ShowItems(TileDescription, items);
        }
    }
}
