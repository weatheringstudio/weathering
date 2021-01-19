﻿
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public interface IUI
    {
        bool Active { get; set; }

        void ShowItems(string title, List<IUIItem> IUIItems);

        void Error(Exception e);
    }


    public class UI : MonoBehaviour, IUI
    {
        public static IUI Ins { get; private set; }
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        public Sprite ColorSprite;
        public Sprite ButtonSprite;
        public Sprite InventoryItemSprite;


        [Space] // UI组件的预制体

        [SerializeField]
        private GameObject BarImage;
        [SerializeField]
        private GameObject ProgressBar;
        [SerializeField]
        private GameObject Text;
        [SerializeField]
        private GameObject InputField;


        [Space] // 特殊位置

        [SerializeField]
        private Canvas Canvas;
        [SerializeField]
        private UnityEngine.UI.GraphicRaycaster Raycaster;
        [SerializeField]
        private GameObject Content;
        [SerializeField]
        private GameObject Title;

        [SerializeField]
        private UnityEngine.UI.Text TitleText;

        private void DestroyChildren() {
            valueProgressBar.Clear();
            timeProgressBar.Clear();
            delProgressBar.Clear();
            dynamicImage.Clear();
            dynamicButtons.Clear();
            dynamicButtonContents.Clear();
            dynamicSliderContents.Clear();
            Transform trans = Content.transform;
            int length = trans.childCount;
            for (int i = 0; i < length; i++) {
                Destroy(trans.GetChild(i).gameObject);
            }
        }

        private BarImage CreateBarImage(string content = null, Func<string> dynamicContent = null, int scale = 1, int leftPadding = 64) {
            BarImage image = Instantiate(BarImage, Content.transform).GetComponent<BarImage>();
            image.RealImage.sprite = content == null ? null : Res.Ins.GetSprite(content);
            if (dynamicContent != null) image.RealImage.sprite = Res.Ins.GetSprite(dynamicContent());
            int rawWidth = image.RealImage.sprite.texture.width;
            int rawHeight = image.RealImage.sprite.texture.height;

            Vector2 size = new Vector2();
            size.x = 640 - leftPadding;
            size.y = rawHeight * scale;
            RectTransform trans = image.transform as RectTransform;
            trans.sizeDelta = size;

            Vector2 size2 = new Vector2();
            size2.x = rawWidth * scale;
            size2.y = rawHeight * scale;
            image.RealImage.rectTransform.sizeDelta = size2;

            if (dynamicContent != null) {
                dynamicImage.Add(image, dynamicContent);
            }
            return image;
        }

        private TextMultiLine CreateText(string content) {
            TextMultiLine result = Instantiate(Text, Content.transform).GetComponent<TextMultiLine>();
            // SetText(text, content);
            result.Content.text = content;
            result.name = "Multiline Text";
            return result;
        }
        //// 必须手动更新text的高度，才能正确适配
        //private void SetText(TextMultiLine text, string content) {
        //    text.Content.text = content;
        //    //(text.transform as RectTransform).SetSizeWithCurrentAnchors(
        //    //    RectTransform.Axis.Vertical, text.Content.preferredHeight + 128);
        //}





        private ProgressBar CreateButton(IUIBackgroundType background, string label = null, Action onTap = null,
            Func<bool> canTap = null, Func<string> dynamicContent = null, Func<float, string> dynamicSlider = null) {
            GameObject progressBarGameObject = Instantiate(ProgressBar, Content.transform);
            ProgressBar result = progressBarGameObject.GetComponent<ProgressBar>();

            if (label != null) result.Text.text = label;
            if (onTap != null) result.OnTap = onTap;
            if (canTap != null) {
                dynamicButtons.Add(result, canTap);
            }
            if (dynamicContent != null) {
                dynamicButtonContents.Add(result, dynamicContent);
            }
            switch (background) {
                case IUIBackgroundType.None:
                    throw new Exception();
                case IUIBackgroundType.Solid:
                    result.Background.color = new Color {
                        a = 4 / 5f, r = 2 / 5f, g = 2 / 5f, b = 2 / 5f
                    };
                    break;
                case IUIBackgroundType.SemiTranspanrent:
                    result.Background.color = new Color {
                        a = 2 / 5f, r = 2 / 5f, g = 2 / 5f, b = 2 / 5f
                    };
                    break;
                case IUIBackgroundType.Transparent:
                    result.Background.color = Color.clear;
                    break;
                case IUIBackgroundType.Button:
                    result.Background.sprite = ButtonSprite;
                    break;
                case IUIBackgroundType.InventoryItem:
                    result.Background.sprite = InventoryItemSprite;
                    result.Background.color = new Color {
                        a = 4 / 5f, r = 2 / 5f, g = 2 / 5f, b = 2 / 5f
                    };
                    break;
                default:
                    throw new Exception();
            }

            result.SliderRaycast.raycastTarget = false;
            result.Slider.interactable = false;
            result.Slider.enabled = true;
            return result;
        }

        private ProgressBar CreateOneLineDynamicText(Func<string> dynamicText) {
            ProgressBar result = CreateButton(IUIBackgroundType.Transparent, null, null, null, dynamicText);
            result.Slider.enabled = false;
            result.Foreground.color = new Color(0, 0, 0, 0);
            result.Background.raycastTarget = false;
            result.name = "Oneline Text";
            return result;
        }


        private ProgressBar CreateSlider(Func<float, string> dynamicSlider) {
            ProgressBar result = CreateProgressBar();
            result.Slider.enabled = true;
            result.Slider.interactable = true;
            result.SliderRaycast.raycastTarget = true;
            result.gameObject.name = "Slider";
            if (dynamicSlider != null) {
                dynamicSliderContents.Add(result, dynamicSlider);
            }
            return result;
        }



        private ProgressBar CreateProgressBar() {
            ProgressBar result = CreateButton(IUIBackgroundType.SemiTranspanrent);
            result.Background.sprite = ColorSprite;
            result.Foreground.gameObject.SetActive(true);
            result.Background.raycastTarget = false;
            return result;
        }

        private ProgressBar CreateValueProgress(IValue value, string barTitle = "") {
            ProgressBar result = CreateProgressBar();
            result.SetTo(CalcUpdateValueProgress(value));
            UpdateValueProgress(result, value, barTitle);
            valueProgressBar.Add(result, new Tuple<IValue, string>(value, barTitle));
            return null;
        }

        private ProgressBar CreateTimeProgress(IValue value, string barTitle = "") {
            ProgressBar result = CreateProgressBar();
            result.SetTo(CalcUpdateTimeProgress(value));
            UpdateTimeProgress(result, value, barTitle);
            timeProgressBar.Add(result, new Tuple<IValue, string>(value, barTitle));
            return null;
        }

        private ProgressBar CreateDelProgress(IValue value, string barTitle = "") {
            ProgressBar result = CreateProgressBar();
            UpdateDelProgress(result, value, barTitle);
            delProgressBar.Add(result, new Tuple<IValue, string>(value, barTitle));
            return null;
        }


        private bool activeLastLastTime;
        private bool activeLastTime;
        private bool active;
        public bool Active {
            get => active || activeLastTime || activeLastLastTime;
            set {
                if (!value) {
                    DestroyChildren();
                    GameEntry.Ins.TrySaveGame();
                }
                active = value;
                Canvas.enabled = value;
                GameMenu.Ins.gameObject.SetActive(!value);
            }
        }

        /// <summary>
        /// 滚动条每帧会根据绑定IValue进行更新。有两种方式显示IValue
        /// </summary>
        private readonly Dictionary<ProgressBar, Tuple<IValue, string>> valueProgressBar = new Dictionary<ProgressBar, Tuple<IValue, string>>();
        private readonly Dictionary<ProgressBar, Tuple<IValue, string>> timeProgressBar = new Dictionary<ProgressBar, Tuple<IValue, string>>();
        private readonly Dictionary<ProgressBar, Tuple<IValue, string>> delProgressBar = new Dictionary<ProgressBar, Tuple<IValue, string>>();
        private readonly Dictionary<BarImage, Func<string>> dynamicImage = new Dictionary<BarImage, Func<string>>();

        private readonly Dictionary<ProgressBar, Func<bool>> dynamicButtons = new Dictionary<ProgressBar, Func<bool>>();
        private readonly Dictionary<ProgressBar, Func<string>> dynamicButtonContents = new Dictionary<ProgressBar, Func<string>>();
        private readonly Dictionary<ProgressBar, Func<float, string>> dynamicSliderContents = new Dictionary<ProgressBar, Func<float, string>>();


        private void Update() {
            activeLastLastTime = activeLastTime;
            activeLastTime = active;
            if (!Active) return;

            foreach (var pair in valueProgressBar) {
                UpdateValueProgress(pair.Key, pair.Value.Item1, pair.Value.Item2);
            }
            foreach (var pair in timeProgressBar) {
                UpdateTimeProgress(pair.Key, pair.Value.Item1, pair.Value.Item2);
            }
            foreach (var pair in delProgressBar) {
                UpdateDelProgress(pair.Key, pair.Value.Item1, pair.Value.Item2);
            }
            foreach (var pair in dynamicImage) {
                pair.Key.RealImage.sprite = Res.Ins.GetSprite(pair.Value.Invoke());
            }
            foreach (var pair in dynamicButtons) {
                bool interactable = pair.Value();
                pair.Key.Button.interactable = interactable;
                pair.Key.Background.raycastTarget = interactable;
            }
            foreach (var pair in dynamicButtonContents) {
                pair.Key.Text.text = pair.Value();
            }
            foreach (var pair in dynamicSliderContents) {
                pair.Key.Text.text = pair.Value(pair.Key.Slider.value);
            }
        }

        private float CalcUpdateValueProgress(IValue value) {
            return value.Max == 0 ? 0 : (float)value.Val / value.Max;
        }
        private void UpdateValueProgress(ProgressBar key, IValue value, string title) {

            if (value.Max == 0) {
                key.SetTo(0); // key.Slider.value = 0;
                key.Text.text = $"{title} 无法储存";
                return;
            } else {
                float result = (float)value.Val / value.Max;
                //if (value.Del <= Value.Second) {
                //    key.SetTo(result);
                //} else {
                //    float dampping = 0.2f;
                //    key.Dampping = value.Del > 2 * Value.Second ? dampping : ((value.Del) / Value.Second - 1) * dampping;
                //    key.DampTo(result);
                //}
                key.Dampping = 0.2f;
                key.DampTo(result);
                key.Text.text = $"{title} { value.Val} / {value.Max}";
            }
        }
        private float CalcUpdateTimeProgress(IValue value) {
            return value.Val == value.Max ? 1 : (float)value.ProgressedTicks / value.Del;
        }
        private void UpdateTimeProgress(ProgressBar key, IValue value, string title) {
            float result = CalcUpdateTimeProgress(value);
            if (value.Del <= Value.Second) {
                key.SetTo(result);
            } else {
                float dampping = 0.2f;
                key.Dampping = value.Del > 2 * Value.Second ? dampping : ((value.Del) / Value.Second - 1) * dampping;
                key.DampTo(result);
            }

            if (value.Val >= value.Max) {
                if (value.Max != 0) {
                    key.Text.text = $"{title} 达到上限";
                } else {
                    key.Text.text = $"{title} 无法储存";
                }
            } else {
                if (value.Inc - value.Dec == 1) {
                    key.Text.text = $"{title} {value.RemainingTimeString}";
                } else if (value.Inc - value.Dec == 0) {
                    if (value.Inc == 0) {
                        key.Text.text = $"{title} 没有产量";
                    } else {
                        key.Text.text = $"{title} 供求平衡";
                    }
                } else {
                    key.Text.text = $"{title} 产量{value.Sur} 时间 {value.RemainingTimeString}";
                }
            }
        }

        private float CalcUpdateDelProgress(IValue value) {
            if (value.Inc == value.Dec) {
                return 1;
            }
            return (float)(value.Inc - value.Dec) / value.Inc;
        }
        private void UpdateDelProgress(ProgressBar key, IValue value, string title) {
            float result = CalcUpdateDelProgress(value);
            float dampping = 0.5f;
            key.Dampping = dampping;
            key.DampTo(result);
            if (value.Inc == value.Dec && value.Inc == 0) {
                key.Text.text = $"{title} 没有产量";
            } else {
                key.Text.text = $"{title} 生产{value.Inc} 消耗{value.Dec}";
            }
        }


        public void ShowItems(string title, List<IUIItem> IUIItems) {
            if (Active) Active = false;
            Active = true;
            TitleText.text = title;
            foreach (IUIItem item in IUIItems) {
                if (item == null) {
                    continue;
                }
                switch (item.Type) {
                    // None, Text, Button, ValueProgress, TimeProgress
                    case IUIItemType.Image:
                        CreateBarImage(item.Content, item.DynamicContent, item.Scale, item.LeftPadding);
                        break;
                    case IUIItemType.MultilineText:
                        CreateText(item.Content);
                        break;
                    case IUIItemType.OnelineDynamicText:
                        CreateOneLineDynamicText(item.DynamicContent);
                        break;
                    case IUIItemType.Button:
                        CreateButton(item.BackgroundType, item.Content, item.OnTap, item.CanTap, item.DynamicContent);
                        break;
                    case IUIItemType.ValueProgress: // val-max
                        if (item.Value == null) throw new Exception();
                        CreateValueProgress(item.Value, item.Content);
                        break;
                    case IUIItemType.TimeProgress: // time-del
                        if (item.Value == null) throw new Exception();
                        CreateTimeProgress(item.Value, item.Content);
                        break;
                    case IUIItemType.DelProgress: // inc dec
                        if (item.Value == null) throw new Exception();
                        CreateDelProgress(item.Value, item.Content);
                        break;
                    case IUIItemType.Slider:
                        if (item.DynamicSliderContent == null) throw new Exception();
                        CreateSlider(item.DynamicSliderContent);
                        break;
                    case IUIItemType.Separator:
                        CreateBarImage("Separator", null, 1, 0);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        public void Error(Exception e) {
            Debug.LogError(e.StackTrace);
            UI.Ins.ShowItems($"<color=red>error</color>: {e.GetType().Name}", new List<IUIItem> {
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "发生错误，可能存档损坏或版本过期，是否要清除存档？"
                },
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = "<color=red>清除存档</color>",
                    OnTap = GameEntry.Ins.DeleteGameSave
                },
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = e.Message
                }
            });
        }
    }
}

