
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Forest : StandardTile
    {
        public override string SpriteKey => typeof(Forest).Name;


        private static bool initialized = false;
        private static string gatherWood;
        private static string gatherFood;

        public override void OnEnable() {
            base.OnEnable();

            if (!initialized) {
                initialized = true;
                gatherWood = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Wood>()}";
                gatherFood = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}";
            }
        }

        public override void OnTap() {
            UI.Ins.ShowItems(TileName,
                UIItem.CreateButton(gatherFood, PageOfFoodGathering),
                UIItem.CreateButton(gatherWood, PageOfWoodGathering),
                UIItem.CreateConstructButton<HuntingGround>(this),
                UIItem.CreateConstructButton<BerryBush>(this),
                UIItem.CreateButton(Localization.Ins.Get<Deforestation>(), () => { }, () => false)
            );
        }

        private const long gatherWoodSanityCost = 1;
        private const long gatherWoodWoodRevenue = 1;
        private void PageOfWoodGathering() {

            UI.Ins.ShowItems(gatherWood,
                UIItem.CreateMultilineText($"捡起地上的小树枝{Localization.Ins.Val<Wood>(gatherWoodWoodRevenue)}{Localization.Ins.Val<Sanity>(-gatherWoodSanityCost)}"),
                UIItemOfWoodGathering(),
                UIItem.CreateValueProgress<Sanity>(Globals.Sanity),

                UIItem.CreateSeparator(),

                UIItem.CreateInventoryTitle(),
                UIItem.CreateInventoryItem<Wood>(Map.Inventory, OnTap),
                UIItem.CreateInventoryCapacity(Map.Inventory),
                UIItem.CreateInventoryTypeCapacity(Map.Inventory)
            );
        }
        private UIItem UIItemOfWoodGathering() {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = gatherWood,
                OnTap = () => {
                    Map.Inventory.Add<Wood>(gatherWoodWoodRevenue);
                    Globals.Ins.Values.GetOrCreate<Sanity>().Val -= gatherWoodSanityCost;
                },
                CanTap = () => Map.Inventory.CanAdd<Wood>() > gatherWoodWoodRevenue
                && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= gatherWoodSanityCost,
            };
        }

        private void PageOfFoodGathering() {
            UI.Ins.ShowItems(gatherFood,
                UIItem.CreateMultilineText($"找点能吃的东西{Localization.Ins.Val<Food>(gatherFoodFoodRevenue)}{Localization.Ins.Val<Sanity>(-gatherWoodSanityCost)}"),
                UIItemOfFoodGathering(),
                UIItem.CreateValueProgress<Sanity>(Globals.Sanity),

                UIItem.CreateSeparator(),

                UIItem.CreateInventoryTitle(),
                UIItem.CreateInventoryItem<Food>(Map.Inventory, OnTap),
                UIItem.CreateInventoryCapacity(Map.Inventory),
                UIItem.CreateInventoryTypeCapacity(Map.Inventory)
            );
        }

        private const long gatherFoodFoodRevenue = 1;
        private UIItem UIItemOfFoodGathering() {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = gatherFood,
                OnTap = () => {
                    Map.Inventory.Add<Food>(gatherFoodFoodRevenue);
                    Globals.Ins.Values.GetOrCreate<Sanity>().Val -= gatherWoodSanityCost;
                },
                CanTap = () => Map.Inventory.CanAdd<Food>() >= gatherFoodFoodRevenue
                && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= gatherWoodSanityCost,
            };
        }
    }
}

