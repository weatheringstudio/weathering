
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

            UI.Ins.ShowItems($"{Localization.Ins.Get<Mountain>()}", items);
        }

        private const long gatherStoneSanityCost = 20;
        private const long gatherStoneStoneRenenue = 1;

        private void PageOfStoneGathering() {
            var items = new List<IUIItem> {
                UIItem.CreateReturnButton(OnTap),

                UIItem.CreateMultilineText($"山里的石头亮闪闪{Localization.Ins.Val<Stone>(gatherStoneStoneRenenue)}{Localization.Ins.Val<Sanity>(-gatherStoneSanityCost)}"),
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
    }
}

