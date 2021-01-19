
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Concept("草地", "C8E78D")]
    public class Grassland : StandardTile
    {
        public override string SpriteKey => typeof(Grassland).Name;

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        private string GatherFoodColored() {
            return Concept.Ins.AddColor<Food>("采集食材");
        }
        public override void OnTap() {
            var items = new List<IUIItem>() {
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = GatherFoodColored(),
                    OnTap = GatherFoodPage,
                },
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Concept.Ins.ColoredNameOf<Construct>(),
                    OnTap = BuildPage
                },
            };
            UI.Ins.ShowItems(Concept.Ins.ColoredNameOf<Grassland>(), items);
        }

        private void GatherFoodPage() {
            string foodColoredName = Concept.Ins.ColoredNameOf<Food>();

            const long sanityCost = 1;
            const long foodRevenue = 1;
            var items = new List<IUIItem>() {
                UIItem.CreateText($"在草地上搜集食材{Concept.Ins.Val<Food>(1)}{Concept.Ins.Val<Sanity>(-1)}"),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = GatherFoodColored(),
                    OnTap = () => {
                        Map.Inventory.Add<Food>(1);
                        Globals.Ins.Values.GetOrCreate<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Food>() >= foodRevenue
                    && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= sanityCost,
                },
            };
            UIItem.AddSeparator(items);
            UIItem.AddInventory<Food>(Map.Inventory, items);
            items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));
            
            // UIItem.AddInventoryInfo(Map.Inventory, items);

            UI.Ins.ShowItems(GatherFoodColored(), items);
        }

        private void BuildPage() {
            var items = new List<IUIItem>();

            const long sanityCost = 10;
            const long foodCost = 10;
            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Concept.Ins.ColoredNameOf<Construct>()}{Concept.Ins.ColoredNameOf<BerryBush>()}{Concept.Ins.Val<Sanity>(-sanityCost)}{Concept.Ins.Val<Food>(-10)}",
                OnTap = () => {
                    Map.Inventory.Remove<Food>(foodCost);
                    Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    Map.UpdateAt<BerryBush>(Pos);
                    Map.Get(Pos).OnTap();
                },
                CanTap = () => Map.Inventory.CanRemove<Food>() >= foodCost
                && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
            });
            UI.Ins.ShowItems(Concept.Ins.ColoredNameOf<Grassland>(), items);
        }

    }
}

