
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Concept]
    public class Grassland : StandardTile
    {
        public override string SpriteKey => typeof(Grassland).Name;

        private static bool initialized = false;
        private static string gatherFood;

        public override void OnEnable() {
            base.OnEnable();
            if (!initialized) {
                initialized = true;
                gatherFood = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.Get<Food>()}";
            }
        }

        public override void OnTap() {
            UI.Ins.ShowItems(Localization.Ins.Get<Grassland>(),
                UIItem.CreateButton(gatherFood, FoodGatheringPage),
                UIItem.CreateConstructButton<Farm>(this),
                UIItem.CreateButton(Localization.Ins.Get<Terraform>(), () => { }, () => false)
            );
        }

        private void FoodGatheringPage() {
            UI.Ins.ShowItems(gatherFood,
                UIItem.CreateMultilineText($"在平原上搜集食材{Localization.Ins.Val<Food>(foodRevenue)}{Localization.Ins.Val<Sanity>(-sanityCost)}"),
                UIItemOfFoodGathering(),

                UIItem.CreateSeparator(),

                UIItem.CreateInventoryTitle(),
                UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values),
                UIItem.CreateInventoryItem<Food>(Map.Inventory),
                UIItem.CreateInventoryCapacity(Map.Inventory),
                UIItem.CreateInventoryTypeCapacity(Map.Inventory)
            );
        }

        private const long sanityCost = 1;
        private const long foodRevenue = 1;
        private UIItem UIItemOfFoodGathering() {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = gatherFood,
                OnTap = () => {
                    Map.Inventory.Add<Food>(foodRevenue);
                    Globals.Ins.Values.GetOrCreate<Sanity>().Val -= sanityCost;
                },
                CanTap = () => Map.Inventory.CanAdd<Food>() >= foodRevenue
                && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= sanityCost,
            };
        }
    }
}

