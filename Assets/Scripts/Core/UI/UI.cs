
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Weathering
{
    public enum IUIItemType
    {
        OnelineDynamicText,
        None, MultilineText, Button, ValueProgress, TimeProgress
    }

    public interface IUIItem
    {
        IUIItemType Type { get; }
        string Content { get; }
        Func<string> DynamicContent { get; set; }
        IValue Value { get; set; }
        Action OnTap { get; set; }
        Func<bool> CanTap { get; set; }
    }


    public class UIItem : IUIItem
    {
        public IUIItemType Type { get; set; }
        public string Content { get; set; }
        public Func<string> DynamicContent { get; set; }
        public IValue Value { get; set; }
        public Action OnTap { get; set; }
        public Func<bool> CanTap { get; set; }
    }


    public interface IUI
    {
        bool Active { get; set; }

        //void Notify(string title, string content);
        //void Confirm(string title, string content, string label, Action onTap = null);
        //void Choose(string title, string content, string yes, string no);

        //void SimpleValue(string title, string content, IValue value, string barTitle = "");

        void UIItems(string title, List<IUIItem> IUIItems);

        void Error(Exception e);
    }

    //public static class UIDecorator
    //{
    //    public static Action Ensure(Action callback, string content=null) {
    //        UI.Ins.UIItems("是否确认", new List<IUIItem> {
    //            new UIItem {
    //                Type = IUIItemType.MultilineText,
    //                Content = "确认" + (content == null ? "要这么做" : content)
    //            },
    //            new UIItem {
    //                Type = IUIItemType.Button,
    //                Content = "确定",
    //                OnTap = callback
    //            },
    //            new UIItem {
    //                Type = IUIItemType.Button,
    //                Content = "取消",
    //                OnTap = () => {
    //                    UI.Ins.Active = false;
    //                }
    //            }
    //        });
    //        return null;
    //    }
    //}

    public class UI : MonoBehaviour, IUI
    {
        public static IUI Ins { get; private set; }
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        public Sprite ColorSprite;
        public Sprite ButtonSprite;


        [Space] // UI组件的预制体

        [SerializeField]
        private GameObject SquareImage;
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
            resourceValues.Clear();
            progressValues.Clear();
            dynamicButtons.Clear();
            dynamicButtonContents.Clear();
            Transform trans = Content.transform;
            int length = trans.childCount;
            for (int i = 0; i < length; i++) {
                Destroy(trans.GetChild(i).gameObject);
            }
        }

        private TextMultiLine CreateText(string content) {
            TextMultiLine text = Instantiate(Text, Content.transform).GetComponent<TextMultiLine>();
            SetText(text, content);
            // text.Content.text = content;
            return text;
        }

        private ProgressBar CreateOneLineDynamicText(Func<string> dynamicText) {
            ProgressBar result = CreateButton(null, null, null, dynamicText);
            result.Background.color = new Color(0, 0, 0, 0);
            result.Foreground.color = new Color(0, 0, 0, 0);
            // result.Background.raycastTarget = false;
            return result;
        }

        // 必须手动更新text的高度，才能正确适配
        private void SetText(TextMultiLine text, string content) {
            text.Content.text = content;
            //(text.transform as RectTransform).SetSizeWithCurrentAnchors(
            //    RectTransform.Axis.Vertical, text.Content.preferredHeight + 128);
        }

        private ProgressBar CreateButton(string label = null, Action onTap = null,
            Func<bool> canTap = null, Func<string> dynamicContent = null) {
            GameObject progressBarGameObject = Instantiate(ProgressBar, Content.transform);
            ProgressBar result = progressBarGameObject.GetComponent<ProgressBar>();
            result.Text.gameObject.SetActive(true);
            result.Button.gameObject.SetActive(true);
            result.Background.gameObject.SetActive(true);
            result.Foreground.gameObject.SetActive(true);
            if (label != null) result.Text.text = label;
            if (onTap != null) result.OnTap = onTap;
            if (canTap != null) {
                dynamicButtons.Add(result, canTap);
                updating = true;
            }
            if (dynamicContent != null) {
                dynamicButtonContents.Add(result, dynamicContent);
                updating = true;
            }
            result.Slider.interactable = false;
            result.Slider.enabled = true;
            result.Background.color = new Color {
                a = 4 / 5f, r = 2 / 5f, g = 2 / 5f, b = 2 / 5f
            };
            result.Background.sprite = ButtonSprite;

            return result;
        }

        private ProgressBar CreateProgressBar() {
            ProgressBar result = CreateButton();
            result.Background.sprite = ColorSprite;
            result.Foreground.gameObject.SetActive(true);
            result.Slider.enabled = true;
            result.Background.raycastTarget = false;
            result.Background.color = new Color {
                a = 2 / 5f, r = 2 / 5f, g = 2 / 5f, b = 2 / 5f
            };
            return result;
        }

        private ProgressBar CreateResourceValue(IValue value, string barTitle = "") {
            ProgressBar result = CreateProgressBar();
            result.SetTo(CalcUpdateResourceValues(value));
            UpdateResourceValues(result, value, barTitle);
            resourceValues.Add(result, new Tuple<IValue, string>(value, barTitle));
            return null;
        }

        private ProgressBar CreateProgressValue(IValue value, string barTitle = "") {
            ProgressBar result = CreateProgressBar();
            result.SetTo(CalcUpdateProgresValues(value));
            UpdateProgressValues(result, value, barTitle);
            progressValues.Add(result, new Tuple<IValue, string>(value, barTitle));
            return null;
        }


        //public void Choose(string title, string content, string yes, string no) {
        //    ActiveNow = true;
        //    TitleText.text = title;
        //    CreateText(content);
        //    CreateButton(yes);
        //    CreateButton(no);
        //}

        //public void Confirm(string title, string content, string label, Action onTap = null) {
        //    ActiveNow = true;
        //    TitleText.text = title;
        //    CreateText(content);
        //    CreateButton(label, onTap);
        //}

        //public void Notify(string title, string content) {
        //    ActiveNow = true;
        //    TitleText.text = title;
        //    CreateText(content);
        //}

        //public void SimpleValue(string title, string content, IValue value, string barTitle = "资源") {
        //    ActiveNow = true;
        //    TitleText.text = title;
        //    CreateText(content);
        //    CreateResourceValue(value, barTitle);
        //    CreateProgressValue(value, barTitle);
        //}

        private bool activeLastTime;
        private bool active;
        public bool Active {
            get => active || activeLastTime;
            set {
                if (!value) {
                    DestroyChildren();
                    GameEntry.Ins.TrySave();
                    updating = value;
                }
                active = value;
                Canvas.enabled = value;
            }
        }

        /// <summary>
        /// 滚动条每帧会根据绑定IValue进行更新。有两种方式显示IValue
        /// </summary>
        private readonly Dictionary<ProgressBar, Tuple<IValue, string>> resourceValues = new Dictionary<ProgressBar, Tuple<IValue, string>>();
        private readonly Dictionary<ProgressBar, Tuple<IValue, string>> progressValues = new Dictionary<ProgressBar, Tuple<IValue, string>>();
        private bool updating = false;
        private readonly Dictionary<ProgressBar, Func<bool>> dynamicButtons = new Dictionary<ProgressBar, Func<bool>>();
        private readonly Dictionary<ProgressBar, Func<string>> dynamicButtonContents = new Dictionary<ProgressBar, Func<string>>();


        private void Update() {
            activeLastTime = active;
            if (!Active) return;

            if (updating) {
                foreach (var pair in resourceValues) {
                    UpdateResourceValues(pair.Key, pair.Value.Item1, pair.Value.Item2);
                }
                foreach (var pair in progressValues) {
                    UpdateProgressValues(pair.Key, pair.Value.Item1, pair.Value.Item2);
                }
                foreach (var pair in dynamicButtons) {
                    bool interactable = pair.Value();
                    pair.Key.Button.interactable = interactable;
                    pair.Key.Background.raycastTarget = interactable;
                }
                foreach (var pair in dynamicButtonContents) {
                    pair.Key.Text.text = pair.Value();
                }
            }
        }
        private float CalcUpdateResourceValues(IValue value) {
            return value.Max == 0 ? 0 : (float)value.Val / value.Max;
        }
        private void UpdateResourceValues(ProgressBar key, IValue value, string title) {

            if (value.Max == 0) {
                key.SetTo(0); // key.Slider.value = 0;
                key.Text.text = title + " - 无法储存";
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
                key.Text.text = title + " - " + value.Val.ToString() + " - " + value.Max.ToString();
            }
        }
        private float CalcUpdateProgresValues(IValue value) {
            return (value.Del == 0 || (value.Inc - value.Dec) == 0 || (value.Val == value.Max)) ? 1 : (float)value.ProgressedTicks / value.Del;
        }
        private void UpdateProgressValues(ProgressBar key, IValue value, string title) {
            float result = CalcUpdateProgresValues(value);
            if (value.Del <= Value.Second) {
                key.SetTo(result);
            } else {
                float dampping = 0.2f;
                key.Dampping = value.Del > 2 * Value.Second ? dampping : ((value.Del) / Value.Second - 1) * dampping;
                key.DampTo(result);
            }

            if (value.Val >= value.Max) {
                if (value.Max != 0) {
                    key.Text.text = title + " - 已满";
                } else {
                    key.Text.text = title + " - 无法储存";
                }
            } else {
                if (value.Inc - value.Dec == 1) {
                    key.Text.text = title + " - " + value.RemainingTimeString;
                } else {
                    key.Text.text = title + " - " + (value.Inc - value.Dec) + " - " + value.RemainingTimeString;
                }
            }
        }


        public void UIItems(string title, List<IUIItem> IUIItems) {
            Active = true;
            TitleText.text = title;
            foreach (IUIItem item in IUIItems) {
                if (item == null) {
                    continue;
                }
                switch (item.Type) {
                    // None, Text, Button, ValueProgress, TimeProgress
                    case IUIItemType.MultilineText:
                        CreateText(item.Content);
                        break;
                    case IUIItemType.OnelineDynamicText:
                        CreateOneLineDynamicText(item.DynamicContent);
                        break;
                    case IUIItemType.Button:
                        CreateButton(item.Content, item.OnTap, item.CanTap, item.DynamicContent);
                        break;
                    case IUIItemType.ValueProgress:
                        CreateResourceValue(item.Value, item.Content);
                        break;
                    case IUIItemType.TimeProgress:
                        CreateProgressValue(item.Value, item.Content);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        public void Error(Exception e) {
            //UI.Ins.UIItems("<color=red>error</color>: " + e.GetType().Name, new List<IUIItem> {
            //    new UIItem {
            //        Type = IUIItemType.MultilineText,
            //        Content = e.Message
            //    }
            //});
        }
    }
}

