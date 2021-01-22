
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Forest : StandardTile
    {
        public override string SpriteKey => typeof(Forest).Name;

        private static bool initialized = false;

        //private static string playerAction;
        private static string construct;
        //private static string management;
        private static string terraform;

        private static string forest;
        private static string gatherWood;

        private static string forestLoggingCamp;

        public override void OnEnable() {
            base.OnEnable();

            if (!initialized) {
                initialized = true;

                //playerAction = Localization.Ins.Get<PlayerAction>();
                construct = Localization.Ins.Get<Construct>();
                //management = Localization.Ins.Get<Management>();
                terraform = Localization.Ins.Get<Terraform>();

                forest = Localization.Ins.Get<Forest>();
                gatherWood = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.Get<Wood>()}";

                forestLoggingCamp = Localization.Ins.Get<ForestLoggingCamp>();
            }
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            var items = new List<IUIItem>() {
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = gatherWood,
                    OnTap = WoodGatheringPage,
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"{construct}{forestLoggingCamp}",
                    OnTap = ForestLoggingCampConstructionPage
                },
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = terraform,
                    CanTap = () => false,
                },

                //new UIItem {
                //    Type = IUIItemType.Button,
                //    Content = playerAction,
                //    OnTap = ActionPage,
                //},
                //new UIItem {
                //    Type = IUIItemType.Button,
                //    Content = construct,
                //    OnTap = ConstructionPage
                //},
                //new UIItem {
                //    Type = IUIItemType.Button,
                //    Content = management,
                //},
            };
            UI.Ins.ShowItems(forest, items);
        }

        //private void ActionPage() {
        //    var items = new List<IUIItem>();
        //    items.Add(new UIItem {
        //        Type = IUIItemType.Button,
        //        Content = gatherWood,
        //        OnTap = WoodGatheringPage,
        //    });
        //    UI.Ins.ShowItems(playerAction, items);
        //}

        //private void ConstructionPage() {
        //    var items = new List<IUIItem>();
        //    items.Add(new UIItem {
        //        Type = IUIItemType.Button,
        //        Content = $"{construct}{forestLoggingCamp}",
        //        OnTap = ForestLoggingCampConstructionPage,
        //    });
        //    UI.Ins.ShowItems(construct, items);
        //}

        private void WoodGatheringPage() {
            var items = new List<IUIItem>() {
                UIItem.CreateMultilineText($"森林里面有木材{Localization.Ins.Val<Wood>(1)}{Localization.Ins.Val<Sanity>(-1)}"),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = gatherWood,
                    OnTap = () => {
                        Map.Inventory.Add<Wood>(1);
                        Globals.Ins.Values.GetOrCreate<Sanity>().Val -= 1;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Wood>() > 0
                    && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= 1,
                },
            };

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));
            UIItem.AddInventoryItem<Wood>(Map.Inventory, items);
            UIItem.AddInventoryInfo(Map.Inventory, items);
            UI.Ins.ShowItems(gatherWood, items);
        }

        private void ForestLoggingCampConstructionPage() {
            var items = new List<IUIItem>();

            const long sanityCost = 10;
            const long woodCost = 10;

            items.Add(new UIItem {
                Type = IUIItemType.MultilineText,
                Content = $"收集木材，1点理智获得10木材，赚翻了",
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{forestLoggingCamp}{Localization.Ins.Val<Sanity>(-sanityCost)}{Localization.Ins.Val<Wood>(-woodCost)}",
                OnTap = () => {
                    Map.Inventory.Remove<Wood>(woodCost);
                    Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    Map.UpdateAt<ForestLoggingCamp>(Pos);
                    Map.Get(Pos).OnTap();
                },
                CanTap = () => Map.Inventory.CanRemove<Wood>() >= woodCost
                && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
            });

            items.Add(UIItem.CreateSeparator());
            UIItem.AddInventoryItem<Wood>(Map.Inventory, items);

            UI.Ins.ShowItems($"{construct}{forestLoggingCamp}", items);
        }
    }
}

