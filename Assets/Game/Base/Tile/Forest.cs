
using System;
using System.Collections.Generic;

namespace Weathering
{
    public class Forest : StandardTile
    {
        public override string SpriteKey => typeof(Forest).Name;

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            string foodColoredName = Concept.Ins.ColoredNameOf<Food>();
            var items = new List<IUIItem>() {
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Concept.Ins.AddColor<Wood>($"采集木材"),
                    OnTap = GatherWoodPage,
                },
            };
            UI.Ins.ShowItems(Concept.Ins.ColoredNameOf<Grassland>(), items);
        }

        private void GatherWoodPage() {
            string foodColoredName = Concept.Ins.ColoredNameOf<Wood>();
            string actionName = Concept.Ins.AddColor<Wood>("采集木材");
            var items = new List<IUIItem>() {
                UIItem.CreateText($"在草地上搜集木材{Concept.Ins.Val<Wood>(1)}{Concept.Ins.Val<Sanity>(-1)}"),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = actionName,
                    OnTap = () => {
                        Map.Inventory.Add<Wood>(1);
                        Globals.Ins.Values.GetOrCreate<Sanity>().Val -= 1;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Wood>(1)
                    && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= 1,
                },
            };
            items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));
            UIItem.AddInventory<Wood>(Map.Inventory, items);
            UIItem.AddInventoryInfo(Map.Inventory, items);
            UI.Ins.ShowItems(actionName, items);
        }
    }
}

