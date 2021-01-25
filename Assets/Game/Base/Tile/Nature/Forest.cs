
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Forest : StandardTile
    {
        public override string SpriteKey => typeof(Forest).Name;


        private static bool initialized = false;
        private static string forest;
        private static string gatherWood;
        private static string huntingGround;

        public override void OnEnable() {
            base.OnEnable();

            if (!initialized) {
                initialized = true;
                forest = Localization.Ins.Get<Forest>();
                gatherWood = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.Get<Wood>()}";
                huntingGround = Localization.Ins.Get<HuntingGround>();
            }
        }

        public override void OnTapPlaySound() {
            Sound.Ins.PlayWoodSound();
        }

        public override void OnTap() {
            UI.Ins.ShowItems(forest,
                UIItem.CreateButton(gatherWood, PageOfWoodGathering),
                UIItem.CreateConstructButton<HuntingGround>(this),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Get<Deforestation>(),
                    CanTap = () => false,
                }
            );
        }


        private void PageOfWoodGathering() {

            const long sanityCost = 1;
            const long woodRevenue = 1;
            var items = new List<IUIItem>() {
                UIItem.CreateMultilineText($"森林里面有木材{Localization.Ins.Val<Wood>(woodRevenue)}{Localization.Ins.Val<Sanity>(-sanityCost)}"),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = gatherWood,
                    OnTap = () => {
                        Map.Inventory.Add<Wood>(woodRevenue);
                        Globals.Ins.Values.GetOrCreate<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Wood>() > woodRevenue
                    && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= sanityCost,
                },
            };

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateInventoryTitle());
            items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));
            items.Add(UIItem.CreateInventoryItem<Wood>(Map.Inventory));
            items.Add(UIItem.CreateInventoryCapacity(Map.Inventory));
            items.Add(UIItem.CreateInventoryTypeCapacity(Map.Inventory));

            UI.Ins.ShowItems(gatherWood, items);
        }

        //private void ConstructionPageOfForestLoggingCamp() {
        //    var items = new List<IUIItem>();

        //    const long sanityCost = 10;
        //    const long woodCost = 10;

        //    items.Add(new UIItem {
        //        Type = IUIItemType.MultilineText,
        //        Content = $"收集木材，1点理智获得10木材，赚翻了",
        //    });

        //    items.Add(new UIItem {
        //        Type = IUIItemType.Button,
        //        Content = $"{construct}{forestLoggingCamp}{Localization.Ins.Val<Sanity>(-sanityCost)}{Localization.Ins.Val<Wood>(-woodCost)}",
        //        OnTap = () => {
        //            Map.Inventory.Remove<Wood>(woodCost);
        //            Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
        //            Map.UpdateAt<ForestLoggingCamp>(Pos);
        //            Map.Get(Pos).OnTap();
        //        },
        //        CanTap = () => Map.Inventory.CanRemove<Wood>() >= woodCost
        //        && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
        //    });

        //    items.Add(UIItem.CreateSeparator());
        //    UIItem.AddInventoryItem<Wood>(Map.Inventory, items);

        //    UI.Ins.ShowItems($"{construct}{forestLoggingCamp}", items);
        //}
    }
}

