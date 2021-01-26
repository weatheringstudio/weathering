
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
        Button, ValueProgress, TimeProgress, SurProgress, Slider
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

        /// <summary>
        /// 横条分隔
        /// </summary>
        /// <returns></returns>
        public static UIItem CreateSeparator() {
            return new UIItem {
                Type = IUIItemType.Separator,
            };
        }

        /// <summary>
        /// 透明分割 (默认256像素高)
        /// </summary>
        public static UIItem CreateTransparency(int scale = 256) {
            return new UIItem {
                Type = IUIItemType.Transparency,
                Scale = scale,
            };
        }


        /// <summary>
        /// 背包两个字
        /// </summary>
        public static IUIItem CreateInventoryTitle() {
            return new UIItem() {
                Type = IUIItemType.OnelineStaticText,
                Content = Localization.Ins.Get<PlayerInventory>(),
            };
        }

        /// <summary>
        /// 背包容量动态文本
        /// </summary>
        private static string uiitemInventoryQuantityCapacity;
        public static IUIItem CreateInventoryCapacity(IInventory inventory) {
            InitializeLocalizationText();
            return new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => string.Format(uiitemInventoryQuantityCapacity, inventory.Quantity, inventory.QuantityCapacity),
            };
        }

        /// <summary>
        /// 背包类型容量动态文本
        /// </summary>
        private static string uiitemInventoryTypeCapacity;
        public static IUIItem CreateInventoryTypeCapacity(IInventory inventory) {
            InitializeLocalizationText();
            return new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => string.Format(uiitemInventoryTypeCapacity, inventory.TypeCount, inventory.TypeCapacity),
            };
        }

        /// <summary>
        /// 背包内容
        /// </summary>
        public static void AddEntireInventoryContent(IInventory inventory, List<IUIItem> items, Action back) {
            IInventoryDefinition definition = inventory as IInventoryDefinition;
            if (definition == null) throw new Exception();
            foreach (var pair in definition.Dict) {
                items.Add(CreateInventoryItem(pair.Key, inventory, back));
            }
        }

        /// <summary>
        /// 背包头和背包内容
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="items"></param>
        /// <param name="back"></param>
        public static void AddEntireInventory(IInventory inventory, List<IUIItem> items, Action back) {
            IInventoryDefinition definition = inventory as IInventoryDefinition;
            if (definition == null) throw new Exception();
            items.Add(CreateReturnButton(back));
            items.Add(CreateInventoryCapacity(inventory));
            items.Add(CreateInventoryTypeCapacity(inventory));
            AddEntireInventoryContent(inventory, items, back);
        }

        private static float SliderValue = 0;
        private static long SliderValueRounded = 0;

        /// <summary>
        /// 一项内容
        /// </summary>
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

        /// <summary>
        /// 背包项目被按时，会发生什么？在这里写了
        /// </summary>
        private static void OnTapInventoryItem(IInventory inventory, Type type, Action back) {
            if (back == null) throw new Exception();

            var items = new List<IUIItem> {
            };

            // 返回按钮
            items.Add(CreateReturnButton(back));

            // 此内容数量
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

            // 所有的tag（todo）

            UI.Ins.ShowItems(Localization.Ins.NoVal(type), items);

        }

        /// <summary>
        /// 多行文本
        /// </summary>
        public static UIItem CreateMultilineText(string text) {
            return new UIItem() {
                Type = IUIItemType.MultilineText,
                Content = text,
            };
        }
        /// <summary>
        /// 单行文本
        /// </summary>
        public static UIItem CreateText(string text) {
            return new UIItem() {
                Type = IUIItemType.OnelineStaticText,
                Content = text,
            };
        }
        /// <summary>
        /// 动态单行文本
        /// </summary>
        public static UIItem CreateDynamicText(Func<string> dynamicText) {
            return new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = dynamicText,
            };
        }

        /// <summary>
        /// 已弃用
        /// </summary>
        private static string uiitemDecIncMaxText;
        public static UIItem CreateDecIncMaxText<T>(IValue value) {
            InitializeLocalizationText();
            return new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => $"{Localization.Ins.ValUnit<T>()}{string.Format(uiitemDecIncMaxText, value.Dec, value.Inc, value.Max)}"
            };
        }



        /// <summary>
        /// val max 进度条
        /// </summary>
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

        /// <summary>
        /// time del 进度条
        /// </summary>
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

        /// <summary>
        /// dec inc sur 进度条
        /// </summary>
        public static UIItem CreateSurProgress<T>(IValues values) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit<T>(),
                Type = IUIItemType.SurProgress,
                Value = values.Get<T>()
            };
        }
        public static UIItem CreateSurProgress<T>(IValue value) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit<T>(),
                Type = IUIItemType.SurProgress,
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

        public static UIItem CreateDestructButton<T>(ITile tile, Func<bool> canTap = null) where T : ITile {
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
                CanTap = canTap,
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

