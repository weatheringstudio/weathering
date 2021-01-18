
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public enum IUIItemType
    {
        None, OnelineDynamicText, MultilineText,
        Separator, Image,
        Button, ValueProgress, TimeProgress, DelProgress
    }

    public interface IUIItem
    {
        IUIItemType Type { get; }
        string Content { get; }
        int Scale { get; set; }
        int LeftPadding { get; set; }
        Func<string> DynamicContent { get; set; }
        IValue Value { get; set; }
        Action OnTap { get; set; }
        Func<bool> CanTap { get; set; }
    }


    public class UIItem : IUIItem
    {
        public IUIItemType Type { get; set; } = IUIItemType.None;
        public int Scale { get; set; } = 1;
        public int LeftPadding { get; set; } = 64;
        public string Content { get; set; }
        public Func<string> DynamicContent { get; set; }
        public IValue Value { get; set; }
        public Action OnTap { get; set; }
        public Func<bool> CanTap { get; set; }

        public static void AddInventory(IInventory inventory, List<IUIItem> items) {
            items.Add(new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => $"背包容量 {inventory.Quantity} / {inventory.QuantityCapacity}",
            });
            foreach (var pair in inventory.Dict) {
                items.Add(new UIItem() {
                    Type = IUIItemType.OnelineDynamicText,
                    DynamicContent = () => $"{Concept.Ins.ColoredNameOf(pair.Key)} {inventory.Dict[pair.Key]}",
                });
            }
        }

        public static UIItem CreateText(string text) {
            return new UIItem() {
                Type = IUIItemType.MultilineText,
                Content = text,
            };
        }

        public static UIItem CreateValueProgress<T>(IValues values) {
            return new UIItem() {
                Content = Concept.Ins.ColoredNameOf<T>(),
                Type = IUIItemType.ValueProgress,
                Value = values.Get<T>()
            };
        }
    }
}

