
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public enum IUIBackgroundType
    {
        None, Transparent, Solid, SemiTranspanrent, Button, ButtonBack, InventoryItem
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
        bool Interactable { get; set; }
        Func<string> DynamicContent { get; set; }
        Func<float, string> DynamicSliderContent { get; set; }
        IValue Value { get; set; }
        Action OnTap { get; set; }
        Func<bool> CanTap { get; set; }

        float InitialSliderValue { get; set; }
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
        public bool Interactable { get; set; } = true;
        public string Content { get; set; }
        public Func<string> DynamicContent { get; set; }
        public Func<float, string> DynamicSliderContent { get; set; }
        public IValue Value { get; set; }
        public Action OnTap { get; set; }
        public Func<bool> CanTap { get; set; }
        public float InitialSliderValue { get; set; }


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


        ///// <summary>
        ///// 背包两个字
        ///// </summary>
        //public static IUIItem CreateInventoryTitle() {
        //    return new UIItem() {
        //        Type = IUIItemType.OnelineStaticText,
        //        Content = Localization.Ins.Get<PlayerInventory>(),
        //    };
        //}

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

        private static void AddEntireInventoryHead(IInventory inventory, List<IUIItem> items) {
            // items.Add(CreateInventoryTitle());
            items.Add(CreateInventoryCapacity(inventory));
            items.Add(CreateInventoryTypeCapacity(inventory));
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
            AddEntireInventoryHead(inventory, items);
            AddEntireInventoryContent(inventory, items, back);
        }


        public static long AddEntireInventoryContentWithTag<T>(IInventory inventory, List<IUIItem> items, Action back) {
            return AddEntireInventoryContentWithTag(typeof(T), inventory, items, back);
        }

        public static long AddEntireInventoryContentWithTag(Type type, IInventory inventory, List<IUIItem> items, Action back) {
            IInventoryDefinition definition = inventory as IInventoryDefinition;
            if (definition == null) throw new Exception();
            long count = 0;
            foreach (var pair in definition.Dict) {
                if (Tag.HasTag(pair.Key, type)) {
                    items.Add(CreateInventoryItem(pair.Key, inventory, back));
                    count++;
                }
            }
            return count;
        }

        public static void AddEntireInventoryWithTag<T>(IInventory inventory, List<IUIItem> items, Action back) {
            IInventoryDefinition definition = inventory as IInventoryDefinition;
            if (definition == null) throw new Exception();
            AddEntireInventoryHead(inventory, items);
            AddEntireInventoryContentWithTag<T>(inventory, items, back);
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
                DynamicContent = () => $"{Localization.Ins.Val(type, inventory.CanRemove(type))}",
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

            var items = UI.Ins.GetItems();

            // 返回按钮
            items.Add(CreateReturnButton(back));

            // 此内容数量
            items.Add(new UIItem {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => $"数量 {inventory.CanRemove(type)}"
            });

            AddItemDescription(items, type);

            if (Tag.HasTag(type, typeof(Discardable))) {
                items.Add(new UIItem {
                    Type = IUIItemType.Slider,
                    DynamicSliderContent = (float x) => {
                        SliderValue = x;
                        SliderValueRounded = (long)Mathf.Round(SliderValue * inventory.CanRemove(type));
                        return $"选择丢弃数量 {SliderValueRounded}";
                    }
                });
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => $"确认丢弃 {SliderValueRounded}",
                    OnTap = () => {
                        if (SliderValueRounded == inventory.CanRemove(type)) {
                            UI.Ins.Active = false;
                        }
                        inventory.Remove(type, SliderValueRounded);
                        back?.Invoke();
                    },
                });
            }

            items.Add(CreateTransparency(128));

            UI.Ins.ShowItems(Localization.Ins.ValUnit(type), items);

        }
        public static void AddItemDescription(List<IUIItem> items, Type type) {

            // 资源特性
            List<Type> allTags = Tag.AllTagOf(type);
            if (allTags != null && allTags.Count > 0) {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("资源特性：");
                foreach (var tag in allTags) {
                    if (Tag.HasTag(type, typeof(InventoryItemResource))) {
                        sb.Append(Localization.Ins.ValUnit(tag));
                    } else {
                        sb.Append(Localization.Ins.Get(tag));
                    }
                }
                items.Add(CreateMultilineText(sb.ToString()));
                items.Add(UIItem.CreateSeparator());
            }


            // 子类物品
            List<Type> allsubtag = Tag.AllSubTagOf(type);
            if (allsubtag != null && allsubtag.Count > 0) {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("子类资源：");
                foreach (var tag in allsubtag) {
                    if (Tag.HasTag(type, typeof(InventoryItemResource))) {
                        sb.Append(Localization.Ins.ValUnit(tag));
                    } else {
                        sb.Append(Localization.Ins.Get(tag));
                    }
                }
                items.Add(CreateMultilineText(sb.ToString()));
                items.Add(CreateSeparator());
            }

            // 物品描述
            var inventoryItemDescription = Attribute.GetCustomAttribute(type, typeof(ConceptDescription)) as ConceptDescription;
            if (inventoryItemDescription != null) {
                items.Add(CreateMultilineText(Localization.Ins.Get(inventoryItemDescription.DescriptionKey)));
            } else {
                items.Add(CreateText("【此资源描述文案有待完善】"));
            }

            // 物品图片

            items.Add(CreateTileImage(type));
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
            return CreateValueProgress(typeof(T), values);
        }
        public static UIItem CreateValueProgress(Type type, IValues values) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit(type),
                Type = IUIItemType.ValueProgress,
                Value = values.Get(type)
            };
        }
        public static UIItem CreateValueProgress<T>(IValue value) {
            return CreateValueProgress(typeof(T), value);
        }
        public static UIItem CreateValueProgress(Type type, IValue value) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit(type),
                Type = IUIItemType.ValueProgress,
                Value = value
            };
        }

        /// <summary>
        /// time del 进度条
        /// </summary>
        public static UIItem CreateTimeProgress<T>(IValues values) {
            return CreateTimeProgress(typeof(T), values);
        }
        public static UIItem CreateTimeProgress(Type type, IValues values) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit(type),
                Type = IUIItemType.TimeProgress,
                Value = values.Get(type)
            };
        }

        public static UIItem CreateTimeProgress<T>(IValue value) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit<T>(),
                Type = IUIItemType.TimeProgress,
                Value = value
            };
        }
        public static UIItem CreateTimeProgress(Type type, IValue value) {
            return new UIItem() {
                Content = Localization.Ins.ValUnit(type),
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

        //public static UIItem ShowInventoryButton(Action back, IInventory inventory) {
        //    return CreateButton("查看背包", () => {
        //        UIPreset.ShowInventory(back, inventory);
        //    });
        //}

        public static UIItem CreateBanner(string name) {
            return new UIItem {
                Type = IUIItemType.Image,
                LeftPadding = 0,
                Content = name
            };
        }

        public static UIItem CreateButton(string label, Action onTap) {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = label,
                OnTap = onTap,
                Interactable = true,
            };
        }

        public static UIItem CreateStaticButton(string label, Action onTap, bool interactable) {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = label,
                OnTap = onTap,
                Interactable = interactable,
            };
        }

        public static UIItem CreateDynamicButton(string label, Action onTap, Func<bool> canTap) {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = label,
                OnTap = onTap,
                CanTap = canTap,
            };
        }

        public static UIItem CreateDynamicContentButton(Func<string> label, Action onTap, Func<bool> canTap = null) {
            return new UIItem {
                Type = IUIItemType.Button,
                DynamicContent = label,
                OnTap = onTap,
                CanTap = canTap,
            };
        }

        public static UIItem CreateReturnButton(Action back) {
            UIItem result = null;
            string title = string.Empty; // Localization.Ins.Get<ReturnMenu>();
            if (back == null) result =  CreateButton(title, () => UI.Ins.Active = false);
            else result =  CreateButton(title, back);
            result.BackgroundType = IUIBackgroundType.ButtonBack;
            return result;
        }

        public static UIItem CreateDestructButton<T>(ITile tile, Func<bool> canTap = null, Action back = null) where T : class, ITile {
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Destruct>()}",
                OnTap =
                    () => {
                        IMap map = tile.GetMap();
                        Vector2Int pos = tile.GetPos();
                        map.UpdateAt<T>(pos);
                        if (back == null) {
                            UI.Ins.Active = false;
                        } else {
                            back.Invoke();
                        }
                    }
                ,
                CanTap = canTap,
            };
        }

        public static IMap ShortcutMap { get; private set; }
        public static Type ShortcutType { get; set; }
        private static UIItem CreateComplexConstructionButton(Type type, ITile tile) {
            CostInfo cost = ConstructionCostBaseAttribute.GetCost(type, tile.GetMap(), true);
            string title = cost.CostType == null ? string.Empty : Localization.Ins.ValPlus(cost.CostType, -cost.RealCostQuantity);
            return new UIItem {
                Interactable = true,
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Construct>()}{Localization.Ins.Get(type)} {title}",
                OnTap =
                    () => {
                        if (!Globals.SanityCheck()) {
                            return;
                        }

                        IMap map = tile.GetMap();
                        Vector2Int pos = tile.GetPos();
                        ITile newTile = map.UpdateAt(type, pos);
                        if (newTile != null) {
                            ShortcutMap = map;
                            ShortcutType = type;

                            UI.Ins.Active = false;
                        }

                        //Action action = () => {
                        //    //shortcutSource = shortcutSourceTileType;
                        //    //shortcutTarget = type;

                        //    IMap map = tile.GetMap();
                        //    Vector2Int pos = tile.GetPos();
                        //    ITile newTile = map.UpdateAt(type, pos);
                        //    if (newTile != null) {
                        //        ShortcutMap = map;
                        //        ShortcutType = type;

                        //        UI.Ins.Active = false;
                        //    }
                        //};
                        //action.Invoke();
                    }
                ,
            };
        }

        public static UIItem CreateConstructionButton<T>(ITile tile) {
            return CreateConstructionButton(typeof(T), tile);
        }
        public static UIItem CreateConstructionButton(Type type, ITile tile) {
            return CreateComplexConstructionButton(type, tile);
        }

        public static UIItem CreateTileImage(Type tileType) {
            return new UIItem {
                Type = IUIItemType.Image,
                Content = tileType.Name,
                Scale = 4
            };
        }
    }
}

