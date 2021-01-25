
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

    [Concept]
    public class UIItemInventoryQuantityCapacity { }

    [Concept]
    public class UIItemInventoryTypeCapacity { }

    [Concept]
    public class UIItemDecIncMaxText { }

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


        private static bool initialized = false;
        private static void InitializeLocalizationText() {
            if (initialized) return;
            initialized = true;

            uiitemInventoryQuantityCapacity = Localization.Ins.Get<UIItemInventoryQuantityCapacity>();
            uiitemInventoryTypeCapacity = Localization.Ins.Get<UIItemInventoryTypeCapacity>();
            uiitemDecIncMaxText = Localization.Ins.Get<UIItemDecIncMaxText>();
        }

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
                Content = Localization.Ins.Get<PlayerInventory>(),
            };
        }

        private static string uiitemInventoryQuantityCapacity;
        public static IUIItem CreateInventoryCapacity(IInventory inventory) {
            InitializeLocalizationText();
            return new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => string.Format(uiitemInventoryQuantityCapacity, inventory.Quantity, inventory.QuantityCapacity),
            };
        }
        private static string uiitemInventoryTypeCapacity;
        public static IUIItem CreateInventoryTypeCapacity(IInventory inventory) {
            InitializeLocalizationText();
            return new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => string.Format(uiitemInventoryTypeCapacity, inventory.TypeCount, inventory.TypeCapacity),
            };
        }

        public static void AddEntireInventory(IInventory inventory, List<IUIItem> items, Action back) {
            IInventoryDefinition definition = inventory as IInventoryDefinition;
            if (definition == null) throw new Exception();
            CreateInventoryCapacity(inventory);
            CreateInventoryTypeCapacity(inventory);
            foreach (var pair in definition.Dict) {
                items.Add(CreateInventoryItem(pair.Key, inventory, back));
            }
        }

        private static float SliderValue = 0;
        private static long SliderValueRounded = 0;

        public static UIItem CreateInventoryItem<T>(IInventory inventory, Action back) {
            return CreateInventoryItem(typeof(T), inventory, back);
        }
        public static UIItem CreateInventoryItem(Type type, IInventory inventory, Action back) {
            return new UIItem() {
                Type = IUIItemType.Button,
                BackgroundType = IUIBackgroundType.InventoryItem,
                DynamicContent = () => $"{Localization.Ins.NoVal(type)} {inventory.Get(type)}",
                OnTap = () => {
                    OnTapInventoryItem(inventory, type, back);
                }
            };
        }

        private static void OnTapInventoryItem(IInventory inventory, Type type, Action back) {
            if (back == null) throw new Exception();

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
                        back?.Invoke();
                    },
                });
            }
            //else {
            //    items.Add(CreateText("无法丢弃此类物品"));
            //}

            UI.Ins.ShowItems(Localization.Ins.NoVal(type), items);

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
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = dynamicText,
            };
        }

        private static string uiitemDecIncMaxText;
        public static UIItem CreateDecIncMaxText<T>(IValue value) {
            InitializeLocalizationText();
            return new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => $"{Localization.Ins.ValUnit<T>()}{string.Format(uiitemDecIncMaxText, value.Dec, value.Inc, value.Max)}"
            };
        }

        public static UIItem CreateValueProgress<T>(IValues values) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit<T>(),
                Type = IUIItemType.ValueProgress,
                Value = values.Get<T>()
            };
        }
        public static UIItem CreateValueProgress<T>(IValue value) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit<T>(),
                Type = IUIItemType.ValueProgress,
                Value = value
            };
        }
        public static UIItem CreateTimeProgress<T>(IValues values) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit<T>(),
                Type = IUIItemType.TimeProgress,
                Value = values.Get<T>()
            };
        }
        public static UIItem CreateTimeProgress<T>(IValue value) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit<T>(),
                Type = IUIItemType.TimeProgress,
                Value = value
            };
        }

        public static UIItem Shortcut;


        public static UIItem ShowInventoryButton(Action back, IInventory inventory) {
            return CreateButton("查看背包", () => {
                UIPreset.ShowInventory(back, inventory);
            });
        }

        public static UIItem CreateButton(string label, Action onTap, Func<bool> canTap = null) {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = label,
                OnTap = onTap,
                CanTap = canTap,
            };
        }

        public static UIItem CreateReturnButton(Action back) {
            return CreateButton(Localization.Ins.Get<ReturnMenu>(), back);
        }

        public static UIItem CreateDestructButton<T>(ITile tile) where T : ITile {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Destruct>()}",
                OnTap =
                    () => {
                        IMap map = tile.GetMap();
                        Vector2Int pos = tile.GetPos();
                        map.UpdateAt<T>(pos);
                        UI.Ins.Active = false;
                    }
                ,
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
            UIItem.AddEntireInventory(inventory, items, back);
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
                UIItem.CreateInventoryItem<T>(inventory, back),
                UIItem.CreateReturnButton(back)
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

