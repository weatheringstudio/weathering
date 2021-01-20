
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

        private static string playerAction;
        private static string construct;
        private static string management;

        private static string grassland;
        private static string gatherFood;

        private static string berryBush;

        public override void OnEnable() {
            base.OnEnable();

            playerAction = Concept.Ins.ColoredNameOf<PlayerAction>();
            construct = Concept.Ins.ColoredNameOf<Construct>();
            management = Concept.Ins.ColoredNameOf<Management>();

            grassland = Concept.Ins.ColoredNameOf<Grassland>();
            gatherFood = $"{Concept.Ins.ColoredNameOf<Gather>()}{Concept.Ins.ColoredNameOf<Food>()}";

            berryBush = Concept.Ins.ColoredNameOf<GrasslandBerryBush>();
        }

        public override void OnTap() {
            var items = new List<IUIItem>() {
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = playerAction,
                    OnTap = ActionPage,
                },
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = construct,
                    OnTap = ConstructionPage
                },
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = management,
                    // OnTap = BuildPage
                },
            };
            UI.Ins.ShowItems(grassland, items);
        }

        private void ActionPage() {
            var items = new List<IUIItem>();
            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = gatherFood,
                OnTap = FoodGatheringPage,
            });
            UI.Ins.ShowItems(playerAction, items);
        }
        private void ConstructionPage() {
            var items = new List<IUIItem>();
            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{berryBush}",
                OnTap = BuildBerryBushPage,
            });
            UI.Ins.ShowItems(construct, items);
        }

        private void FoodGatheringPage() {

            const long sanityCost = 1;
            const long foodRevenue = 1;
            var items = new List<IUIItem>() {
                UIItem.AddText($"在平原上搜集食材{Concept.Ins.Val<Food>(1)}{Concept.Ins.Val<Sanity>(-1)}"),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = gatherFood,
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

            UI.Ins.ShowItems(gatherFood, items);
        }



        private void BuildBerryBushPage() {
            var items = new List<IUIItem>();

            const long sanityCost = 10;
            const long foodCost = 10;

            items.Add(new UIItem {
                Type = IUIItemType.MultilineText,
                Content = $"建造浆果丛后，1点理智收集10食材，赚翻了",
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{berryBush}{Concept.Ins.Val<Sanity>(-sanityCost)}{Concept.Ins.Val<Food>(-10)}",
                OnTap = () => {
                    Map.Inventory.Remove<Food>(foodCost);
                    Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    Map.UpdateAt<GrasslandBerryBush>(Pos);
                    Map.Get(Pos).OnTap();
                },
                CanTap = () => Map.Inventory.CanRemove<Food>() >= foodCost
                && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
            });

            UIItem.AddSeparator(items);

            UIItem.AddInventory<Food>(Map.Inventory, items);

            UI.Ins.ShowItems($"{Concept.Ins.ColoredNameOf<Construct>()}{Concept.Ins.ColoredNameOf<GrasslandBerryBush>()}", items);
        }


    }
}

