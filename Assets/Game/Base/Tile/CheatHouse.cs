

using System.Collections.Generic;

namespace Weathering
{
    public class CheatHouse : StandardTile
    {
        public override string SpriteKey => "Pitaya";
        public override void OnTap() {
            var items = UI.Ins.GetItems();
            items_ = items;

            AddButton<ElectricitySupply>(100);
            AddButton<GoldCoin>(10);
            AddButton<Worker>(10);
            AddButton<WoodPlank>(100);
            AddButton<StoneBrick>(100);
            AddButton<Brick>(100);

            items_ = null;

            if (CanDestruct()) items.Add(UIItem.CreateDynamicDestructButton<TerrainDefault>(this));

            UI.Ins.ShowItems("作弊点", items);
        }
        private List<IUIItem> items_;

        private void AddButton<T>(long quantity) {
            items_.Add(UIItem.CreateButton($"增加{Localization.Ins.Val<T>(quantity)}", () => Map.Inventory.Add<T>(quantity)));
        }
        public override bool CanDestruct() => true;
    }
}
