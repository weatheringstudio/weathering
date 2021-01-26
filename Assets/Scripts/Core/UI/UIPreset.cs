
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class UIPresetResourceInsufficient { }

    [Concept]
    public class UIPresetInventoryFull { }
    [Concept]
    public class UIPresetInventoryFullTitle { }

    [Concept]
    public class InsufficientResource { }

    [Concept]
    public class InsufficientResourceTitle { }

    public static class UIPreset
    {
        public static void ShowInventory(Action back, IInventory inventory) {
            List<IUIItem> items = new List<IUIItem>();
            UIItem.AddEntireInventory(inventory, items, () => ShowInventory(back, inventory));
            UI.Ins.ShowItems("背包", items);
        }

        public static void Notify(Action back, string content, string title = null) {
            UI.Ins.ShowItems(title == null ? "提示" : title
                , UIItem.CreateText(content)
                , UIItem.CreateReturnButton(back)
            );
        }

        public static void ResourceInsufficient<T>(Action back, long required, IValue value) {
            Type type = typeof(T);
            UI.Ins.ShowItems(Localization.Ins.Get<InsufficientResourceTitle>(),
                UIItem.CreateText(string.Format(Localization.Ins.Get<InsufficientResource>(), string.Format(Localization.Ins.Get<T>(), required))),
                UIItem.CreateValueProgress<T>(value),
                UIItem.CreateReturnButton(back)
            );
        }
        public static void ResourceInsufficient<T>(Action back, long required, IInventory inventory) {
            Type type = typeof(T);
            UI.Ins.ShowItems(Localization.Ins.Get<InsufficientResourceTitle>(),
                UIItem.CreateText(string.Format(Localization.Ins.Get<InsufficientResource>(), string.Format(Localization.Ins.Get<T>(), required))),
                UIItem.CreateReturnButton(back),

                UIItem.CreateSeparator(),
                UIItem.CreateInventoryTitle(),
                UIItem.CreateInventoryItem<T>(inventory, back)
            );
        }

        public static void InventoryFull(Action back, IInventory inventory) {
            var items = new List<IUIItem>() {
                UIItem.CreateText(Localization.Ins.Get<UIPresetInventoryFull>()),
                UIItem.CreateReturnButton(back),

                UIItem.CreateSeparator(),
                UIItem.CreateInventoryTitle(),
                UIItem.CreateInventoryCapacity(inventory),
                UIItem.CreateInventoryTypeCapacity(inventory)
            };

            UIItem.AddEntireInventory(inventory, items, () => InventoryFull(back, inventory));

            UI.Ins.ShowItems(Localization.Ins.Get<UIPresetInventoryFullTitle>(), items);
        }
    }
}

