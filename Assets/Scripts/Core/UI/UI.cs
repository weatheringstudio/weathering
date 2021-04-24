
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Weathering
{

    public interface IUI {
        bool Active { get; set; }

        List<IUIItem> GetItems();
        void ShowItems(string title, List<IUIItem> uiitems);
        void ShowItems(string title, params IUIItem[] uiitems);
        void Error(Exception e);

        void SetBackgroundTransparency(float a);
    }


    public class UI : MonoBehaviour, IUI {
        public static IUI Ins { get; private set; }
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;

            rawImage.color = Color.white;
        }

        public Sprite ColorSprite;
        public Sprite ButtonSprite;
        public Sprite ButtonSpriteBack;
        public Sprite InventoryItemSprite;
        public Sprite Separator;
        public Sprite ProgressBarBackground;
        public Sprite ProgressBarForeground;

        [Space] // UI组件的预制体

        [SerializeField]
        private GameObject BarImage;
        [SerializeField]
        private GameObject ProgressBar;
        [SerializeField]
        private GameObject Text;
        //[SerializeField]
        //private GameObject InputField; // 应该已经不用了


        [Space] // 特殊位置
        [SerializeField]
        private RawImage rawImage;
        public RenderTexture Raw {
            set {
                rawImage.texture = value;
            }
        }

        [SerializeField]
        private Image MainBackground;
        [SerializeField]
        private Image TitleBackground;
        public void SetBackgroundTransparency(float a) {
            MainBackground.color = SetAlpha(MainBackground.color, a);
            // TitleBackground.color = SetAlpha(MainBackground.color, a);
        }
        private Color SetAlpha(Color c, float a) {
            c.a = a; return c;
        }

        [Space] // 特殊位置

        [SerializeField]
        private Canvas Canvas;
        [SerializeField]
        private UnityEngine.UI.GraphicRaycaster Raycaster;
        [SerializeField]
        private GameObject Content;
        [SerializeField]
        public GameObject Title;
        public static bool DontCloseOnIntroduction = false;
        public void OnTapTitle() {
            if (DontCloseOnIntroduction) return;
            Active = false;
            PlaySound();
        }

        [SerializeField]
        private UnityEngine.UI.Text InputFieldTextComponent;
        [SerializeField]
        private UnityEngine.UI.InputField InputFieldComponent;

        [SerializeField]
        private UnityEngine.UI.Text TitleText;

        [SerializeField]
        private GameObject ExitIndicator;
        public void SetExitIndicatorVisible(bool val) {
            ExitIndicator.SetActive(val);
        }


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

        private BarImage CreateTransparency(int scale) {
            BarImage image = Instantiate(BarImage, Content.transform).GetComponent<BarImage>();
            image.RealImage.sprite = null;
            image.RealImage.color = Color.clear;

            RectTransform trans = image.transform as RectTransform;
            if (trans == null) throw new Exception();

            Vector2 size = trans.sizeDelta;
            size.y = scale;
            trans.sizeDelta = size;

            return image;
        }

        public const int DefaultHeight = 180;
        public const int DefaultWidth = 320;

        public const int DefaultUIHeight = 360;
        public const int DefaultUIWidth = 640;


        public const int DefaultPPU = 32;
        private BarImage CreateBarImage(string content = null, Func<string> dynamicContent = null,
            Action onTap=null,
            int scale = 1, int leftPadding = 64,
            bool useSeparator = false) {

            Sprite sprite;
            if (useSeparator) {
                sprite = Separator;
            } else {
                sprite = content == null ? null : Res.Ins.TryGetSprite(content);
            }
            if (sprite == null) return null;

            BarImage image = Instantiate(BarImage, Content.transform).GetComponent<BarImage>();
            image.RealImage.sprite = sprite;

            if (onTap != null) {
                image.Button.enabled = true;
                image.OnButtonTapped = onTap;
            }

            if (image.RealImage.sprite != null) {
                if (dynamicContent != null) image.RealImage.sprite = Res.Ins.TryGetSprite(dynamicContent());
                int rawWidth = image.RealImage.sprite.texture.width;
                int rawHeight = image.RealImage.sprite.texture.height;

                Vector2 size = new Vector2();
                size.x = DefaultUIWidth - leftPadding;
                size.y = rawHeight * scale;
                RectTransform trans = image.transform as RectTransform;
                trans.sizeDelta = size;

                image.RealImage.rectTransform.sizeDelta = new Vector2(rawWidth * scale, rawHeight * scale);
            }

            if (dynamicContent != null) {
                dynamicImage.Add(image, dynamicContent);
            }
            return image;
        }

        private TextMultiLine CreateText(string content) {
            TextMultiLine result = Instantiate(Text, Content.transform).GetComponent<TextMultiLine>();
            // result.GetComponent<UnityEngine.UI.ContentSizeFitter>().enabled = true;
            // SetText(text, content);
            result.Content.text = content;
            result.name = "Multiline Text";
            return result;
        }
        //// 必须手动更新text的高度, 才能正确适配
        //private void SetText(TextMultiLine text, string content) {
        //    text.Content.text = content;
        //    //(text.transform as RectTransform).SetSizeWithCurrentAnchors(
        //    //    RectTransform.Axis.Vertical, text.Content.preferredHeight + 128);
        //}

        private static Color solidColor = new Color {
            a = 1f, r = 2 / 5f, g = 2 / 5f, b = 2 / 5f
        };

        private static Color semiTransparentColor = new Color {
            a = 2 / 5f, r = 1f, g = 1f, b = 1f
        };

        private ProgressBar CreateButton(IUIBackgroundType background, string label = null, string icon = null, Action onTap = null,
                bool interactable=true, Func<bool> canTap = null, Func<string> dynamicContent = null) {
            GameObject progressBarGameObject = Instantiate(ProgressBar, Content.transform);
            ProgressBar result = progressBarGameObject.GetComponent<ProgressBar>();

            if (label != null) result.Text.text = label;
            if (onTap != null) {
                result.OnTap = onTap;
            }
            if (canTap != null) {
                if (!interactable) throw new Exception("当interactable为false时, canTap必须为空");
                dynamicButtons.Add(result, canTap);
                result.Button.interactable = canTap();
            } else {
                result.Button.interactable = interactable;
            }
            if (dynamicContent != null) {
                dynamicButtonContents.Add(result, dynamicContent);
            }
            switch (background) {
                case IUIBackgroundType.None:
                    throw new Exception();
                case IUIBackgroundType.Solid:
                    result.Background.color = solidColor;
                    break;
                case IUIBackgroundType.SemiTranspanrent:
                    result.Background.color = semiTransparentColor;
                    break;
                case IUIBackgroundType.Transparent:
                    result.Background.color = Color.clear;
                    break;
                case IUIBackgroundType.Button:
                    result.Background.sprite = ButtonSprite;
                    result.Background.color = new Color(1, 1, 1, 1 / 5f);
                    break;
                case IUIBackgroundType.ButtonBack:
                    result.Background.sprite = ButtonSpriteBack;
                    result.Background.color = solidColor;
                    break;
                case IUIBackgroundType.InventoryItem:
                    result.Background.sprite = InventoryItemSprite;
                    result.Background.color = new Color(1, 1, 1, 1 / 5f);

                    break;
                default:
                    throw new Exception();
            }
            if (icon != null) {
                result.Text.GetComponent<RectTransform>().anchoredPosition = new Vector2(64, 0);
                Sprite sprite = Res.Ins.TryGetSprite(icon);
                if (sprite != null) {
                    result.IconImage.enabled = true;
                    result.IconImage.sprite = sprite;
                }
            }

            result.SliderRaycast.raycastTarget = false;
            result.Slider.interactable = false;
            result.Slider.enabled = true;
            result.name = "Button";
            return result;
        }

        private ProgressBar CreateOneLineDynamicText(Func<string> dynamicText) {
            ProgressBar result = CreateButton(IUIBackgroundType.Transparent, null, null, null, false, null, dynamicText);
            result.Slider.enabled = false;
            result.Foreground.color = new Color(0, 0, 0, 0);
            result.Background.raycastTarget = false;
            result.name = "Oneline Dynamic Text";
            result.Text.text = dynamicText();
            return result;
        }

        private ProgressBar CreateOneLineStaticText(string content) {
            ProgressBar result = CreateButton(IUIBackgroundType.Transparent, content, null, null, false, null, null);
            result.Slider.enabled = false;
            result.Foreground.color = new Color(0, 0, 0, 0);
            result.Background.raycastTarget = false;
            result.name = "Oneline Static Text";
            return result;
        }


        private ProgressBar CreateProgressBar() {
            ProgressBar result = CreateButton(IUIBackgroundType.SemiTranspanrent);
            result.Background.sprite = ProgressBarBackground;
            result.Foreground.sprite = ProgressBarForeground;
            result.Foreground.color = semiTransparentColor;
            result.Background.color = semiTransparentColor;
            result.Foreground.gameObject.SetActive(true);
            result.Background.raycastTarget = false;
            result.name = "ProgressBar";
            return result;
        }

        private ProgressBar CreateSlider(Func<float, string> dynamicSlider, float initialValue=0) {
            ProgressBar result = CreateProgressBar();
            result.Slider.value = initialValue;
            result.Slider.enabled = true;
            result.Slider.interactable = true;
            result.SliderRaycast.raycastTarget = true;
            result.gameObject.name = "Slider";
            if (dynamicSlider != null) {
                dynamicSliderContents.Add(result, dynamicSlider);
            }
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
            result.SetTo(CalcUpdateDelProgress(value));
            UpdateDelProgress(result, value, barTitle);
            delProgressBar.Add(result, new Tuple<IValue, string>(value, barTitle));
            return null;
        }

        //[SerializeField]
        //private Camera UICamera;
        //public float CameraSize {
        //    set { UICamera.orthographicSize = value; }
        //}

        [SerializeField]
        private GameObject Container;

        private bool activeLastLastTime;
        private bool activeLastTime;
        private bool active;
        public bool Active {
            get => active || activeLastTime || activeLastLastTime;
            set {
                if (!value) {
                    if (!active) return;
                    DestroyChildren();
                    GameMenu.Entry.TrySaveGame();
                }
                active = value;
                // Canvas.enabled = value;
                Container.SetActive(value);
                // GameMenu.Ins.gameObject.SetActive(!value);
                GameMenu.Ins.SetVisible(!value);
            }
        }

        public void PlaySound() {
            // 点退出时
            Sound.Ins.PlayDefaultSound();
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

        private float lastY = 0;
        private void Update() {
            // 因为执行顺序的问题, 等两帧
            activeLastLastTime = activeLastTime;
            activeLastTime = active;
            if (!Active) return;

            // 当位置接近整数时, 字体可能取样模糊
            const float e = 1 / 64f;
            if (Content.transform is RectTransform rect) {
                float y = rect.anchoredPosition.y;
                float deltaY = y - lastY;
                if (deltaY < e && deltaY > -e) { // 没什么速度变化时
                    float fraction = (float)(y - (int)y);
                    if (fraction < e) {
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y + e);
                    } else if (fraction > (1 - e)) {
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y - e);
                    }
                }
                lastY = y;
            }

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
                pair.Key.RealImage.sprite = Res.Ins.TryGetSprite(pair.Value.Invoke());
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
            if (value.Maxed) {
                if (value.Max == 0) {
                    key.SetTo(0); // key.Slider.value = 0;
                    key.Text.text = $"{title} {value.Val} / {value.Max} 无法储存";
                    return;
                } else {
                    key.DampTo(1);
                    key.Text.text = $"{title} {value.Val} / {value.Max} 资源已满";
                }
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
                key.Text.text = $"{title} { Mathf.Max(value.Val, 0)} / {value.Max}";
            }
        }
        private float CalcUpdateTimeProgress(IValue value) {
            return value.Val == value.Max ? 1 : (float)value.ProgressedTicks / value.Del;
        }
        private void UpdateTimeProgress(ProgressBar key, IValue value, string title) {
            float result = CalcUpdateTimeProgress(value);
            if (value.Del <= 5 * Value.Second) {
                key.SetTo(result);
            } else {
                float dampping = 0.2f;
                key.Dampping = value.Del > 2 * Value.Second ? dampping : ((value.Del) / Value.Second - 5) * dampping;
                key.DampTo(result);
            }

            string dec = value.Dec == 0 ? string.Empty : $"需求 {value.Dec}";
            if (value.Val >= value.Max) {
                if (value.Val == 1) {
                    if (value.Max != 0) {
                        key.Text.text = $"{title} {dec} 资源已满";
                    } else {
                        key.SetTo(0);
                        key.Text.text = $"{title} {dec} 无法储存";
                    }
                }
                else {
                    if (value.Max != 0) {
                        key.Text.text = $"{title} 供应 { value.Val} {dec} 资源已满";
                    } else {
                        key.SetTo(0);
                        key.Text.text = $"{title} 供应 { value.Val} {dec} 无法储存";
                    }
                }

            } else {
                if (value.Inc - value.Dec == 1) {
                    key.Text.text = $"{title} {value.RemainingTimeString}";
                } 
                //else if (value.Inc - value.Dec == 0) {
                //    if (value.Inc == 0) {
                //        key.Text.text = $"{title} 供应 {value.Inc} 没有供应";
                //    } else {
                //        key.Text.text = $"{title} 供应 {value.Inc} 需求 { value.Dec} 供求平衡";
                //    }
                //} 
                else {
                    key.Text.text = $"{title} {value.RemainingTimeString} 供应 {value.Inc} {dec}";
                }
            }
        }

        private float CalcUpdateDelProgress(IValue value) {
            if (value.Inc == value.Dec) {
                return 0;
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

        public bool ShowInputFieldNextTime { set; private get; } = false;
        public string InputFieldContent { get => InputFieldTextComponent.text; set => InputFieldTextComponent.text = value; }
        public List<IUIItem> GetItems() => new List<IUIItem>();

        public void ShowItems(string title, List<IUIItem> IUIItems) {
            if (ShowInputFieldNextTime) {
                InputFieldComponent.gameObject.SetActive(true);
                InputFieldTextComponent.text = null;
                ShowInputFieldNextTime = false;
            }
            else {
                InputFieldComponent.gameObject.SetActive(false);
            }

            if (Active) Active = false;
            Active = true;
            TitleText.text = title;
            foreach (IUIItem item in IUIItems) {
                if (item == null) {
                    continue;
                }
                switch (item.Type) {
                    // None, Text, Button, ValueProgress, TimeProgress
                    case IUIItemType.Separator:
                        CreateBarImage(null, null, item.OnTap, 1, 0, true);
                        break;
                    case IUIItemType.Transparency:
                        CreateTransparency(item.Scale);
                        break;
                    case IUIItemType.Image:
                        CreateBarImage(item.Content, item.DynamicContent, item.OnTap, item.Scale, item.LeftPadding);
                        break;
                    case IUIItemType.MultilineText:
                        CreateText(item.Content);
                        break;
                    case IUIItemType.OnelineDynamicText:
                        CreateOneLineDynamicText(item.DynamicContent);
                        break;
                    case IUIItemType.OnelineStaticText:
                        CreateOneLineStaticText(item.Content);
                        break;
                    case IUIItemType.Button:
                        CreateButton(item.BackgroundType, item.Content, item.Icon, item.OnTap, item.Interactable, item.CanTap, item.DynamicContent);
                        break;
                    case IUIItemType.ValueProgress: // val-max
                        if (item.Value == null) throw new Exception();
                        CreateValueProgress(item.Value, item.Content);
                        break;
                    case IUIItemType.TimeProgress: // time-del
                        if (item.Value == null) throw new Exception();
                        CreateTimeProgress(item.Value, item.Content);
                        break;
                    case IUIItemType.SurProgress: // inc dec
                        if (item.Value == null) throw new Exception();
                        CreateDelProgress(item.Value, item.Content);
                        break;
                    case IUIItemType.Slider:
                        if (item.DynamicSliderContent == null) throw new Exception();
                        CreateSlider(item.DynamicSliderContent, item.InitialSliderValue);
                        break;
                    default:
                        throw new Exception(item.Type.ToString());
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(Content.transform as RectTransform);
        }

        public void Error(Exception e) {
            Debug.LogError(e.StackTrace);
            UI.Ins.ShowItems($"<color=red>error</color>: {e.GetType().Name}", new List<IUIItem> {
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "发生错误, 可能存档损坏或版本过期, 是否要清除存档? "
                },
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = "<color=red>清除存档</color>",
                    OnTap = GameMenu.Entry.DeleteGameSave
                },
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = e.Message
                }
            });
        }

        public void ShowItems(string title, params IUIItem[] uiitems) {
            if (uiitems.Length == 0) throw new Exception("不能显示空UI");
            ShowItems(title, new List<IUIItem>(uiitems));
        }
    }
}

