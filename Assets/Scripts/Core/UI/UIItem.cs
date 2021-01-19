
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public enum IUIItemType
    {
        None, OnelineDynamicText, MultilineText,
        Separator, Image,
        Button, ValueProgress, TimeProgress, DelProgress, Slider
    }

    public interface IUIItem
    {
        IUIItemType Type { get; }
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
        public int Scale { get; set; } = 1;
        public int LeftPadding { get; set; } = 64;
        public string Content { get; set; }
        public Func<string> DynamicContent { get; set; }
        public Func<float, string> DynamicSliderContent { get; set; }
        public IValue Value { get; set; }
        public Action OnTap { get; set; }
        public Func<bool> CanTap { get; set; }


        public static void AddInventoryInfo(IInventory inventory, List<IUIItem> items) {
            items.Add(new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => $"背包物品容量 {inventory.Quantity} / {inventory.QuantityCapacity}",
            });
            items.Add(new UIItem() {
                Type = IUIItemType.OnelineDynamicText,
                DynamicContent = () => $"背包物品种类 {inventory.TypeCount} / {inventory.TypeCapacity}",
            });
        }

        public static void AddEntireInventory(IInventory inventory, List<IUIItem> items) {
            IInventoryDefinition definition = inventory as IInventoryDefinition;
            if (definition == null) throw new Exception();
            AddInventoryInfo(inventory, items);
            foreach (var pair in definition.Dict) {
                AddInventory(pair.Key, inventory, items);
            }
        }

        public static void AddInventory<T>(IInventory inventory, List<IUIItem> items) {
            AddInventory(typeof(T), inventory, items);
        }

        private static float SliderValue = 0;
        private static long SliderValueRounded = 0;
        private static void AddInventory(Type type, IInventory inventory, List<IUIItem> items) {
            items.Add(new UIItem() {
                Type = IUIItemType.Button,
                DynamicContent = () => $"{Concept.Ins.ColoredNameOf(type)} {inventory.Get(type)}",
                OnTap = () => {

                    UI.Ins.ShowItems(Concept.Ins.ColoredNameOf(type), new List<IUIItem> {
                        new UIItem {
                            Type = IUIItemType.OnelineDynamicText,
                            DynamicContent = () => $"当前数量 {inventory.Get(type)}"
                        },
                        new UIItem {
                            Type = IUIItemType.Button,
                            Content = "丢弃全部",
                            OnTap = () => {
                                inventory.Take(type, inventory.Get(type));
                                UI.Ins.Active = false;
                            },
                        },
                        new UIItem {
                            Type = IUIItemType.Slider,
                            DynamicSliderContent = (float x) => {
                                SliderValue = 1-x;
                                SliderValueRounded = (long)Mathf.Round(SliderValue*inventory.Get(type));
                                return $"丢弃{SliderValueRounded}";
                            }
                        },
                        new UIItem {
                            Type = IUIItemType.Button,
                            DynamicContent = () => $"丢弃{SliderValueRounded}",
                            OnTap = () => {
                                if (SliderValueRounded == inventory.Get(type)) {
                                    UI.Ins.Active = false;
                                }
                                inventory.Take(type, SliderValueRounded);
                                
                            },
                        },
                    });
                }
            });
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
                Value = values.GetOrCreate<T>()
            };
        }

        public static UIItem CreateGlobalValueProgress<T>() {
            return new UIItem() {
                Content = Concept.Ins.ColoredNameOf<T>(),
                Type = IUIItemType.ValueProgress,
                Value = Globals.Ins.Values.GetOrCreate<T>()
            };
        }
    }
}

