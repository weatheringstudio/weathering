
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public enum IUIBackgroundType
    {
        None, Transparent, Solid, SemiTranspanrent, Button, InventoryItem
    }
    public enum IUIItemType
    {
        None, OnelineDynamicText, OnelineStaticText, MultilineText,
        Separator, Image, Transparency,
        Button, ValueProgress, TimeProgress, DelProgress, Slider
    }

    public interface IUIItem
    {
        IUIItemType Type { get; }
        IUIBackgroundType BackgroundType { get; }
        string Content { get; }
        int Scale { get; set; }
        int LeftPadding { get; set; }
        Func<string> DynamicContent { get; set; }
        Func<float, string> DynamicSliderContent { get; set; }
        IValue Value { get; set; }
        Action OnTap { get; set; }
        Func<bool> CanTap { get; set; }
    }

    public class UIItem : IUIItem
    {
        public IUIItemType Type { get; set; } = IUIItemType.None;
        public IUIBackgroundType BackgroundType { get; set; } = IUIBackgroundType.Solid;
        public int Scale { get; set; } = 1;
        public int LeftPadding { get; set; } = 64;
        public string Content { get; set; }
        public Func<string> DynamicContent { get; set; }
        public Func<float, string> DynamicSliderContent { get; set; }
        public IValue Value { get; set; }
        public Action OnTap { get; set; }
        public Func<bool> CanTap { get; set; }


        public static UIItem CreateSeparator() {
            return new UIItem {
                Type = IUIItemType.Separator,
            };
        }

        public static UIItem CreateTransparency(int scale = 256) {
            return new UIItem {
                Type = IUIItemType.Transparency,
                Scale = scale,
            };
        }

        public static IUIItem CreateInventoryTitle() {
            return new UIItem() {
                Type = IUIItemType.OnelineStaticText,
                DynamicContent = () => $"{Localization.Ins.Get<PlayerInventory>()}",
            };
        }

        public static IUIItem CreateInventoryCapacity(IInventory inventory) {
            return new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => $"容量 {inventory.Quantity} / {inventory.QuantityCapacity}",
            };
        }
        public static IUIItem CreateInventoryTypeCapacity(IInventory inventory) {
            return new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => $"种类 {inventory.TypeCount} / {inventory.TypeCapacity}",
            };
        }

        public static void AddEntireInventory(IInventory inventory, List<IUIItem> items) {
            IInventoryDefinition definition = inventory as IInventoryDefinition;
            if (definition == null) throw new Exception();
            CreateInventoryCapacity(inventory);
            CreateInventoryTypeCapacity(inventory);
            foreach (var pair in definition.Dict) {
                items.Add(CreateInventoryItem(pair.Key, inventory));
            }
        }

        private static float SliderValue = 0;
        private static long SliderValueRounded = 0;

        public static UIItem CreateInventoryItem<T>(IInventory inventory) {
            return CreateInventoryItem(typeof(T), inventory);
        }
        public static UIItem CreateInventoryItem(Type type, IInventory inventory) {
            return new UIItem() {
                Type = IUIItemType.Button,
                BackgroundType = IUIBackgroundType.InventoryItem,
                DynamicContent = () => $"{Localization.Ins.Get(type)} {inventory.Get(type)}",
                OnTap = () => {
                    OnTapInventoryItem(inventory, type);
                }
            };
        }

        private static void OnTapInventoryItem(IInventory inventory, Type type) {
            var items = new List<IUIItem> {
            };

            items.Add(new UIItem {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => $"当前数量 {inventory.Get(type)}"
            });


            if (true) {
                items.Add(new UIItem {
                    Type = IUIItemType.Slider,
                    DynamicSliderContent = (float x) => {
                        SliderValue = 1 - x;
                        SliderValueRounded = (long)Mathf.Round(SliderValue * inventory.Get(type));
                        return $"丢弃 {SliderValueRounded}";
                    }
                });
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => $"丢弃 {SliderValueRounded}",
                    OnTap = () => {
                        if (SliderValueRounded == inventory.Get(type)) {
                            UI.Ins.Active = false;
                        }
                        inventory.Remove(type, SliderValueRounded);
                    },
                });
            }
            //else {
            //    items.Add(CreateText("无法丢弃此类物品"));
            //}

            UI.Ins.ShowItems(Localization.Ins.Get(type), items);

        }

        public static UIItem CreateMultilineText(string text) {
            return new UIItem() {
                Type = IUIItemType.MultilineText,
                Content = text,
            };
        }
        public static UIItem CreateText(string text) {
            return new UIItem() {
                Type = IUIItemType.OnelineStaticText,
                Content = text,
            };
        }
        public static UIItem CreateDynamicText(Func<string> dynamicText) {
            return new UIItem() {
                Type = IUIItemType.OnelineStaticText,
                DynamicContent = dynamicText,
            };
        }

        public static UIItem CreateValueProgress<T>(IValues values) {
            return new UIItem() {
                Content = Localization.Ins.Get<T>(),
                Type = IUIItemType.ValueProgress,
                Value = values.Get<T>()
            };
        }
        public static UIItem CreateTimeProgress<T>(IValues values) {
            return new UIItem() {
                Content = Localization.Ins.Get<T>(),
                Type = IUIItemType.TimeProgress,
                Value = values.Get<T>()
            };
        }

        public static UIItem CreateGlobalValueProgress<T>() {
            return new UIItem() {
                Content = Localization.Ins.Get<T>(),
                Type = IUIItemType.ValueProgress,
                Value = Globals.Ins.Values.GetOrCreate<T>()
            };
        }

        public static UIItem Shortcut;

        public static UIItem CreateButton(string label, Action onTap, Func<bool> canTap=null) {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = label,
                OnTap = onTap,
                CanTap = canTap,
            };
        }

        public static UIItem CreateDestructButton<T>(ITile tile) where T : ITile {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Destruct>()}",
                OnTap = UIDecorator.ConfirmBefore(
                    () => {
                        IMap map = tile.GetMap();
                        Vector2Int pos = tile.GetPos();
                        map.UpdateAt<T>(pos);
                        UI.Ins.Active = false;
                    }
                ),
            };
        }

        public static UIItem CreateConstructButton<T>(ITile tile) where T : ITile {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Construct>()}{Localization.Ins.Get<T>()}",
                OnTap =
                    () => {
                        IMap map = tile.GetMap();
                        Vector2Int pos = tile.GetPos();
                        map.UpdateAt<T>(pos);
                        map.Get(pos).OnTap();
                    }
                ,
            };
        }
    }
}

