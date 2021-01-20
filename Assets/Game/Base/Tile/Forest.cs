
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept("森林", "D2A064")]
    public class Forest : StandardTile
    {
        public override string SpriteKey => typeof(Forest).Name;

        private static string playerAction;
        private static string construct;
        private static string management;

        private static string forest;
        private static string gatherWood;

        private static string forestLoggingCamp;
        private static string wood;

        public override void OnEnable() {
            base.OnEnable();

            playerAction = Concept.Ins.ColoredNameOf<PlayerAction>();
            construct = Concept.Ins.ColoredNameOf<Construct>();
            management = Concept.Ins.ColoredNameOf<Management>();

            forest = Concept.Ins.ColoredNameOf<Forest>();
            gatherWood = $"{Concept.Ins.ColoredNameOf<Gather>()}{Concept.Ins.ColoredNameOf<Wood>()}";

            forestLoggingCamp = Concept.Ins.ColoredNameOf<ForestLoggingCamp>();
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
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
                },
            };
            UI.Ins.ShowItems(forest, items);
        }

        private void ActionPage() {
            var items = new List<IUIItem>();
            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = gatherWood,
                OnTap = WoodGatheringPage,
            });
            UI.Ins.ShowItems(playerAction, items);
        }

        private void ConstructionPage() {
            var items = new List<IUIItem>();
            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{forestLoggingCamp}",
                OnTap = ForestLoggingCampConstructionPage,
            });
            UI.Ins.ShowItems(construct, items);
        }

        private void WoodGatheringPage() {
            var items = new List<IUIItem>() {
                UIItem.AddText($"森林里面有木材{Concept.Ins.Val<Wood>(1)}{Concept.Ins.Val<Sanity>(-1)}"),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = gatherWood,
                    OnTap = () => {
                        Map.Inventory.Add<Wood>(1);
                        Globals.Ins.Values.GetOrCreate<Sanity>().Val -= 1;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Wood>() >= 1
                    && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= 1,
                },
            };

            UIItem.AddSeparator(items);

            items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));
            UIItem.AddInventory<Wood>(Map.Inventory, items);
            UIItem.AddInventoryInfo(Map.Inventory, items);
            UI.Ins.ShowItems(gatherWood, items);
        }

        private void ForestLoggingCampConstructionPage() {
            var items = new List<IUIItem>();

            const long sanityCost = 10;
            const long woodCost = 10;
            const long foodCost = 10;

            items.Add(new UIItem {
                Type = IUIItemType.MultilineText,
                Content = $"收集木材，1点理智获得10木材，赚翻了",
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{forestLoggingCamp}{Concept.Ins.Val<Sanity>(-sanityCost)}{Concept.Ins.Val<Food>(-10)}{Concept.Ins.Val<Wood>(-10)}",
                OnTap = () => {
                    Map.Inventory.Remove<Food>(foodCost);
                    Map.Inventory.Remove<Wood>(woodCost);
                    Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    Map.UpdateAt<ForestLoggingCamp>(Pos);
                    Map.Get(Pos).OnTap();
                },
                CanTap = () => Map.Inventory.CanRemove<Food>() >= foodCost
                && Map.Inventory.CanRemove<Wood>() >= woodCost
                && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
            });

            UIItem.AddSeparator(items);

            UIItem.AddInventory<Wood>(Map.Inventory, items);

            UI.Ins.ShowItems($"{construct}{forestLoggingCamp}", items);
        }
    }
}

