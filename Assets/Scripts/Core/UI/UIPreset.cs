
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
            items.Add(UIItem.CreateReturnButton(back));
            UIItem.AddEntireInventory(inventory, items, back);
            UI.Ins.ShowItems(Localization.Ins.Get<PlayerInventory>(), items);
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
                UIItem.CreateText(string.Format(Localization.Ins.Get<InsufficientResource>(), Localization.Ins.Val<T>(required))),
                UIItem.CreateValueProgress<T>(value),
                UIItem.CreateReturnButton(back)
            );
        }
        public static void ResourceInsufficient<T>(Action back, long required, IInventory inventory) {
            ResourceInsufficient(typeof(T), back, required, inventory);
        }
        public static void ResourceInsufficient(Type type, Action back, long required, IInventory inventory) {
            UI.Ins.ShowItems(Localization.Ins.Get<InsufficientResourceTitle>(),
                UIItem.CreateText(string.Format(Localization.Ins.Get<InsufficientResource>(), Localization.Ins.Val(type, required))),
                UIItem.CreateReturnButton(back),

                UIItem.CreateSeparator(),
                UIItem.CreateInventoryTitle(),
                UIItem.CreateInventoryItem(type, inventory, back)
            );
        }

        public static void ResourceInsufficientWithTag<T>(Action back, long required, IInventory inventory) {
            ResourceInsufficientWithTag(typeof(T), back, required, inventory);
        }
        public static void ResourceInsufficientWithTag(Type type, Action back, long required, IInventory inventory) {
            var items = new List<IUIItem>() {
                UIItem.CreateText(string.Format(Localization.Ins.Get<InsufficientResource>(), string.Format(Localization.Ins.Get(type), required))),
                UIItem.CreateReturnButton(back),

                UIItem.CreateSeparator(),
                UIItem.CreateInventoryTitle(),
            };

            foreach (var pair in inventory) {
                UIItem.CreateInventoryItem(pair.Key, inventory, () => {
                    ResourceInsufficient(type, back, required, inventory);
                });
            }
            UI.Ins.ShowItems(Localization.Ins.Get<InsufficientResourceTitle>(), items);
        }


        public static void InventoryFull(Action back, IInventory inventory, string extraContent=null) {
            var items = new List<IUIItem>() {
            };

            items.Add(UIItem.CreateText(Localization.Ins.Get<UIPresetInventoryFull>()));

            if (extraContent != null) {
                items.Add(UIItem.CreateMultilineText(extraContent));
            }

            UIItem.AddEntireInventory(inventory, items, () => InventoryFull(back, inventory));
            UI.Ins.ShowItems(Localization.Ins.Get<UIPresetInventoryFullTitle>(), items);
        }
    }
}

