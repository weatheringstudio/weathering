
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class Mountain : StandardTile
    {
        public override string SpriteKey => typeof(Mountain).Name;

        public override void OnTap() {
            var items = new List<IUIItem>();

            items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Stone>()}"
                , PageOfStoneGathering));


            items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<MetalOre>()}"
                , PageOfMetalOreGathering));

            items.Add(UIItem.CreateConstructionButton<MountainQuarry>(this));
            items.Add(UIItem.CreateConstructionButton<MountainMine>(this));

            items.Add(new UIItem {
                Type = IUIItemType.Image,
                LeftPadding = 0,
                Content = "MountainBanner"
            });

            UI.Ins.ShowItems($"{Localization.Ins.Get<Mountain>()}", items);
        }

        private const long gatherStoneSanityCost = 20;
        private const long gatherStoneStoneRenenue = 1;

        private void PageOfStoneGathering() {
            var items = new List<IUIItem> {
                UIItem.CreateReturnButton(OnTap),

                UIItem.CreateMultilineText($"岩石非常坚硬，很难开采和搬运{Localization.Ins.Val<Stone>(gatherStoneStoneRenenue)}{Localization.Ins.Val<Sanity>(-gatherStoneSanityCost)}"),
                UIItem_StoneGathering(),

                UIItem.CreateSeparator(),
                UIItem.CreateValueProgress<Sanity>(Globals.Sanity)
            };
            items.Add(UIItem.CreateInventoryItem<Stone>(Map.Inventory, PageOfStoneGathering));
            UI.Ins.ShowItems(Localization.Ins.Get<Mountain>(), items);
        }
        private UIItem UIItem_StoneGathering() {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Stone>()}",
                OnTap = () => {
                    Map.Inventory.Add<Stone>(gatherStoneStoneRenenue);
                    Globals.Ins.Values.GetOrCreate<Sanity>().Val -= gatherStoneSanityCost;
                },
                CanTap = () => Map.Inventory.CanAdd<Stone>() > gatherStoneStoneRenenue
                && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= gatherStoneSanityCost,
            };
        }


        private const long gatherMetalOreSanityCost = 50;
        private const long gatherMetalOreMetalOreRenenue = 1;

        private void PageOfMetalOreGathering() {
            var items = new List<IUIItem> {
                UIItem.CreateReturnButton(OnTap),

                UIItem.CreateMultilineText($"金属矿埋在地下，很难开采{Localization.Ins.Val<MetalOre>(gatherMetalOreMetalOreRenenue)}{Localization.Ins.Val<Sanity>(-gatherMetalOreSanityCost)}"),
                UIItem_MetalOreGathering(),

                UIItem.CreateSeparator(),
                UIItem.CreateValueProgress<Sanity>(Globals.Sanity)
            };
            items.Add(UIItem.CreateInventoryItem<MetalOre>(Map.Inventory, PageOfMetalOreGathering));
            UI.Ins.ShowItems(Localization.Ins.Get<Mountain>(), items);
        }
        private UIItem UIItem_MetalOreGathering() {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<MetalOre>()}",
                OnTap = () => {
                    Map.Inventory.Add<MetalOre>(gatherMetalOreMetalOreRenenue);
                    Globals.Ins.Values.GetOrCreate<Sanity>().Val -= gatherMetalOreSanityCost;
                },
                CanTap = () => Map.Inventory.CanAdd<MetalOre>() > gatherMetalOreMetalOreRenenue
                && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= gatherMetalOreSanityCost,
            };
        }
    }
}

