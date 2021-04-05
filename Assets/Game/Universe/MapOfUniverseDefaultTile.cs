

namespace Weathering
{
    public class MapOfUniverseDefaultTile : StandardTile, IDontSave
    {
        public bool DontSave => true;

        public const int galaxyDensity = 50;
        private bool isGalaxy => HashCode % galaxyDensity == 0;
        public override string SpriteKey => isGalaxy ? "Galaxy" : null;


        public override void OnTap() {
            var items = UI.Ins.GetItems();

            string title = $"{typeof(MapOfGalaxy).Name}#{Pos.x},{Pos.y}";
            if (isGalaxy) {

                items.Add(UIItem.CreateButton($"进入{title}", () => {
                    GameEntry.Ins.EnterChildMap(typeof(MapOfGalaxy), Map, Pos);
                }));
            }
            else {

                items.Add(UIItem.CreateText("无法离开宇宙"));
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}
