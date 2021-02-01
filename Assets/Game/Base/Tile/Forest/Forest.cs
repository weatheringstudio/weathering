
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Forest : StandardTile
    {
        public override string SpriteKey => typeof(Forest).Name;

        public override void OnTap() {
            UI.Ins.ShowItems(Localization.Ins.Get<Forest>(),
                //UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}", PageOfFoodGathering),
                //UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Wood>()}", PageOfWoodGathering),
                UIItem.CreateConstructButton<HuntingGround>(this),
                //UIItem.CreateConstructButton<BerryBush>(this),
                UIItem.CreateConstructButton<ForestLoggingCamp>(this),
                UIItem.CreateConstructButton<ForestToGrassland>(this)
                // UIItem.CreateButton(Localization.Ins.Get<Terraform>(), () => { }, () => false)
            );
        }

        private const long gatherWoodSanityCost = 5;
        private const long gatherWoodWoodRevenue = 1;
        private void PageOfWoodGathering() {
            var items = new List<IUIItem> {
                UIItem.CreateReturnButton(OnTap),

                UIItem.CreateMultilineText($"捡起地上的小树枝{Localization.Ins.Val<Wood>(gatherWoodWoodRevenue)}{Localization.Ins.Val<Sanity>(-gatherWoodSanityCost)}"),
                UIItemOfWoodGathering(),

                UIItem.CreateSeparator(),
                UIItem.CreateValueProgress<Sanity>(Globals.Sanity)
            };
            items.Add(UIItem.CreateInventoryItem<Wood>(Map.Inventory, PageOfWoodGathering));
            UI.Ins.ShowItems(Localization.Ins.Get<Forest>(), items);
        }
        private UIItem UIItemOfWoodGathering() {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Wood>()}",
                OnTap = () => {
                    Map.Inventory.Add<Wood>(gatherWoodWoodRevenue);
                    Globals.Ins.Values.GetOrCreate<Sanity>().Val -= gatherWoodSanityCost;
                },
                CanTap = () => Map.Inventory.CanAdd<Wood>() > gatherWoodWoodRevenue
                && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= gatherWoodSanityCost,
            };
        }

        private void PageOfFoodGathering() {
            var items = new List<IUIItem> {
                UIItem.CreateReturnButton(OnTap),

                UIItem.CreateMultilineText($"找点能吃的东西{Localization.Ins.Val<Food>(gatherFoodFoodRevenue)}{Localization.Ins.Val<Sanity>(-gatherFoodSanityCost)}"),
                UIItem_CreateFoodGathering(),

                UIItem.CreateSeparator(),
                UIItem.CreateValueProgress<Sanity>(Globals.Sanity)
            };
            items.Add(UIItem.CreateInventoryItem<Food>(Map.Inventory, PageOfWoodGathering));
            UI.Ins.ShowItems(Localization.Ins.Get<Forest>(), items);
        }

        private const long gatherFoodSanityCost = 1;
        private const long gatherFoodFoodRevenue = 1;
        private UIItem UIItem_CreateFoodGathering() {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}",
                OnTap = () => {
                    Map.Inventory.Add<Food>(gatherFoodFoodRevenue);
                    Globals.Ins.Values.GetOrCreate<Sanity>().Val -= gatherFoodSanityCost;
                },
                CanTap = () => Map.Inventory.CanAdd<Food>() >= gatherFoodFoodRevenue
                && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= gatherFoodSanityCost,
            };
        }
    }
}

