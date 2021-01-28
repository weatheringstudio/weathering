
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
                UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Berry>()}", PageOfFoodGathering),
                UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Wood>()}", PageOfWoodGathering),
                UIItem.CreateConstructButton<HuntingGround>(this),
                UIItem.CreateConstructButton<BerryBush>(this),
                UIItem.CreateButton(Localization.Ins.Get<Terraform>(), () => { }, () => false)
            );
        }

        private const long gatherWoodSanityCost = 5;
        private const long gatherWoodWoodRevenue = 1;
        private void PageOfWoodGathering() {

            UI.Ins.ShowItems(Localization.Ins.Get<Forest>(),
                UIItem.CreateReturnButton(OnTap),

                UIItem.CreateMultilineText($"捡起地上的小树枝{Localization.Ins.Val<Wood>(gatherWoodWoodRevenue)}{Localization.Ins.Val<Sanity>(-gatherWoodSanityCost)}"),
                UIItemOfWoodGathering(),
                UIItem.CreateValueProgress<Sanity>(Globals.Sanity)

                , UIItem.CreateSeparator()
                , UIItem.CreateInventoryTitle()
                , UIItem.CreateInventoryItem<Wood>(Map.Inventory, OnTap)
                , UIItem.CreateInventoryCapacity(Map.Inventory)
                , UIItem.CreateInventoryTypeCapacity(Map.Inventory)
            );
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
            UI.Ins.ShowItems(Localization.Ins.Get<Forest>(),
                UIItem.CreateReturnButton(OnTap),

                UIItem.CreateMultilineText($"找点能吃的东西{Localization.Ins.Val<Berry>(gatherFoodFoodRevenue)}{Localization.Ins.Val<Sanity>(-gatherFoodSanityCost)}"),
                UIItem_CreateFoodGathering(),
                UIItem.CreateValueProgress<Sanity>(Globals.Sanity)

                , UIItem.CreateSeparator()
                , UIItem.CreateInventoryTitle()
                , UIItem.CreateInventoryItem<Berry>(Map.Inventory, OnTap)
                , UIItem.CreateInventoryCapacity(Map.Inventory)
                , UIItem.CreateInventoryTypeCapacity(Map.Inventory)
            );
        }

        private const long gatherFoodSanityCost = 1;
        private const long gatherFoodFoodRevenue = 1;
        private UIItem UIItem_CreateFoodGathering() {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Berry>()}",
                OnTap = () => {
                    Map.Inventory.Add<Berry>(gatherFoodFoodRevenue);
                    Globals.Ins.Values.GetOrCreate<Sanity>().Val -= gatherFoodSanityCost;
                },
                CanTap = () => Map.Inventory.CanAdd<Berry>() >= gatherFoodFoodRevenue
                && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= gatherFoodSanityCost,
            };
        }
    }
}

