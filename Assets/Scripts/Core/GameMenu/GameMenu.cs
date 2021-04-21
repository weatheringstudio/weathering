
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Concept]
    public class GameMenuExitGame { }
    [Concept]
    public class GameMenuSaveGame { }
    [Concept]
    public class GameMenuResetGame { }
    [Concept]


    public class GameSettings { }
    [Concept]
    public class GameLanguage { }
    [Concept]
    public class GameMenuLanguageLabel { }


    [Concept]
    public class GameMenuInspectInventory { }

    [Concept]
    public class GameMenuGotoMainMap { }

    [Concept]
    public class GameMenuResetGameConfirmation { }

    [Concept]
    public class GameMenuLabel { }


    [Concept]
    public class UserInterfaceBackgroundTransparency { }


    [Concept]
    public class UtilityButtonsOnTheLeft { }

    [Concept]
    public class LogisticsAnimationIsLinear { }

    [Concept]
    public class InversedMovement { }

    [Concept]
    public class EnableLight { }

    [Concept]
    public class SecondsForADay { }

    public interface ITileDescription
    {
        string TileDescription { get; }
    }

    /// <summary>
    /// 重构方法：配置项：类型，getter，setter
    /// </summary>
    public class GameMenu : MonoBehaviour
    {
        public static GameEntry Entry { get; set; }

        public static GameMenu Ins { get; private set; }

        public static bool IsInEditor { get; private set; }
        public static bool IsInStandalone { get; private set; }
        public static bool IsInMobile { get; private set; }

        private void Awake() {

            if (Ins != null) throw new Exception();
            Ins = this;

#if UNITY_EDITOR
            IsInEditor = true;
#else
            IsInEditor = false;
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
            IsInStandalone = true;
            IsInMobile = false;
#else
            IsInStandalone = false;
            IsInMobile = true;
#endif
            offset = IsInStandalone ? 36 : 0;
            InitializeNotification();

            fullScreenWidth = Screen.width;
            fullScreenHeight = Screen.height;
            if (IsInMobile) {
                TryIncreaseGamePerformance();
            }
        }

        [Header("notification")]
        [SerializeField]
        private GameObject Container;
        public void SetVisible(bool visible) {
            Container.SetActive(visible);
        }

        [SerializeField]
        private UnityEngine.UI.Text TileDescriptionForStandalone;
        [SerializeField]
        private UnityEngine.UI.Text Notification1Text;
        [SerializeField]
        private UnityEngine.UI.Text Notification2Text;
        [SerializeField]
        private RectTransform Notification1Transform;
        [SerializeField]
        private RectTransform Notification2Transform;

        public void SetTileDescriptionForStandalong(string text) {
            TileDescriptionForStandalone.text = text;
        }
        private float pushedTime = 0;
        const float left = 0;
        const float top = 36;
        public void PushNotification(string notice) {
            if (Notification1Text.text.Length == 0) {
                Notification1Text.text = notice;
            } else if (Notification2Text.text.Length == 0) {
                Notification2Text.text = notice;
            } else {
                Notification1Text.text = Notification2Text.text;
                Notification2Text.text = notice;
            }
            Notification1Transform.anchoredPosition = new Vector2(left, -top - offset);
            Notification2Transform.anchoredPosition = new Vector2(left, -top * 2 - offset);
            pushedTime = Time.time;
        }
        float offset = 0;

        private void InitializeNotification() {
            Notification1Text.text = null;
            Notification2Text.text = null;
            Notification1Transform.anchoredPosition = new Vector2(left, -top - offset);
            Notification2Transform.anchoredPosition = new Vector2(left, -top * 2 - offset);
            Notification1Text.material.color = SetA(Notification1Text.color, 0);
            Notification2Text.material.color = SetA(Notification1Text.color, 0);
            pushedTime = 0;
        }

        private float alpha1 = 0;
        private float alpha2 = 0;
        private void Update() {
            float time = Time.time;
            const float animatedTime = 1f;
            float deltaTime = pushedTime == 0 ? animatedTime : time - pushedTime;
            if (deltaTime < animatedTime) {
                float normalTime = deltaTime / animatedTime;
                float sinTime = Mathf.Sin(Mathf.PI * normalTime);
                if (Notification2Text.text.Length == 0) {
                    Notification1Transform.anchoredPosition = new Vector2(left, -top - offset);
                    Notification1Text.material.color = SetA(Notification1Text.color, sinTime > 0.5f ? 1 : Mathf.Lerp(0, 1, sinTime / 0.5f));
                } else {
                    Notification1Transform.anchoredPosition = new Vector2(left, Mathf.Lerp(-top - offset, 0 - offset, normalTime < 0.5f ? sinTime : 1));
                    Notification2Transform.anchoredPosition = new Vector2(left, Mathf.Lerp(-top * 2 - offset, -top - offset, normalTime < 0.5f ? sinTime : 1));

                    alpha1 = normalTime < 0.5f ? 1 - sinTime : 0;
                    alpha2 = sinTime;
                    Notification1Text.material.color = SetA(Notification1Text.color, alpha1);
                    Notification2Text.material.color = SetA(Notification2Text.color, alpha2);
                }
            } else {
                Notification1Text.text = null;
                Notification2Text.text = null;
                if (alpha1 != 0) {
                    alpha1 = 0;
                    Notification1Text.material.color = SetA(Notification1Text.color, alpha1);
                }
                if (alpha2 != 0) {
                    alpha2 = 0;
                    Notification2Text.material.color = SetA(Notification2Text.color, alpha2);
                }
            }
        }
        private Color SetA(Color c, float a) {
            c.a = a;
            return c;
        }

        public int fullScreenWidth = 0;
        public int fullScreenHeight = 0;

        private void Start() {
            SynchronizeSettings();
            SyncButtonsOutlines();
            TileDescriptionForStandalone.gameObject.SetActive(IsInStandalone);

            SyncHammer();
            SyncMagnet();
        }
        public static void OnConstruct() {
            RestoreDefaultSettings();
            if (!GameConfig.CheatMode) {
                SpecialPages.OpenStartingPage();
            }
            // Sound.Ins.PlayRandomMusic();
        }

        public static void RestoreDefaultSettings() {
            // 现在习惯把和游戏设置有关, 游戏逻辑无关的初始化过程, 放到GameMenu。和游戏逻辑有关的放到GameConfig

            IGlobals globals = Globals.Ins;

            // 初始音效音量
            IValue soundEffectVolume = globals.Values.GetOrCreate<SoundEffectVolume>();
            soundEffectVolume.Max = 600;
            // 初始音乐音量
            IValue musicEffectVolume = globals.Values.GetOrCreate<SoundMusicVolume>();
            musicEffectVolume.Max = 600;

            //// 提示设置
            //Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>(true);
            //Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>(true);

            globals.Bool<SoundEffectDisabled>(false);
            globals.Bool<SoundMusicEnabled>(true);

            globals.Values.GetOrCreate<MapView.TappingSensitivity>().Max = 100;

            globals.Bool<UsePixelFont>(Screen.width < UI.DefaultWidth * 2 || Screen.height < UI.DefaultHeight * 2);

            globals.Values.GetOrCreate<UserInterfaceBackgroundTransparency>().Max = (long)(0.75f * userInterfaceBackgroundTransparencyFactor);

            globals.Bool<UtilityButtonsOnTheLeft>(false);
            globals.Bool<LogisticsAnimationIsLinear>(false);
            globals.Bool<InversedMovement>(false);

            globals.Bool<EnableLight>(true);

            globals.Values.GetOrCreate<SecondsForADay>().Max = 120;
        }

        public void SynchronizeSettings() {
            SyncSecondsForADay();
            SyncEnableLight();
            SynchronizeFont();
            //SyncSFXVolume();
            //SyncMusicVolume();
            SyncCameraSensitivity();
            SyncDoubleSize();
            SyncUserInterfaceBackgroundTransparency();
            SyncUtilityButtonPosition();
        }

        //private void SyncMusicVolume() {
        //    if (Globals.Ins.Bool<SoundMusicEnabled>()) {
        //        Sound.Ins.PlayDefaultMusic();
        //    }
        //    else {
        //        Sound.Ins.StopDefaultMusic();
        //    }
        //    Sound.Ins.SetDefaultMusicVolume(Sound.Ins.GetDefaultMusicVolume());
        //}

        //private void SyncSFXVolume() {
        //    Sound.Ins.SetDefaultSoundVolume(Sound.Ins.GetDefaultSoundVolume());
        //}

        
        public void SyncSecondsForADay() {
            GlobalLight.Ins.SecondsForADay = Globals.Ins.Values.Get<SecondsForADay>().Max;
        }

        public static bool LightEnabled { get; private set; }
        public void SyncEnableLight() {
            LightEnabled = Globals.Ins.Bool<EnableLight>();
            (MapView.Ins as MapView).EnableShadowAndLight = LightEnabled;
        }


        private const float camerSensitivityFactor = 100f;
        private void SyncCameraSensitivity() {
            MapView.Ins.TappingSensitivityFactor = MapView.DefaultTappingSensitivity * (Globals.Ins.Values.GetOrCreate<MapView.TappingSensitivity>().Max / camerSensitivityFactor);
        }
        private void SyncDoubleSize() {
            ScreenAdaptation.Ins.DoubleSize = Globals.Ins.Bool<ScreenAdaptation.DoubleSizeOption>();
        }

        private const float userInterfaceBackgroundTransparencyFactor = 100f;
        private void SyncUserInterfaceBackgroundTransparency() {
            UI.Ins.SetBackgroundTransparency(Globals.Ins.Values.GetOrCreate<UserInterfaceBackgroundTransparency>().Max / userInterfaceBackgroundTransparencyFactor);
        }

        public static bool UseInversedMovement { get; private set; }
        private void SyncInversedMovement() {
            UseInversedMovement = Globals.Ins.Bool<InversedMovement>();
        }
        private void SyncUtilityButtonPosition() {
            if (LinkUnlinkButtonImage.transform is RectTransform rect) {
                rect.anchoredPosition = new Vector2(Globals.Ins.Bool<UtilityButtonsOnTheLeft>() ? (72 - 640) : 0, rect.anchoredPosition.y);
            }
            if (ConstructDestructButtonImage.transform is RectTransform rect2) {
                rect2.anchoredPosition = new Vector2(Globals.Ins.Bool<UtilityButtonsOnTheLeft>() ? (72 - 640) : 0, rect2.anchoredPosition.y);
            }
        }
        public static bool IsLinear { get; private set; } = false;
        private void SyncLogisticsAnimation() {
            IsLinear = Globals.Ins.Bool<LogisticsAnimationIsLinear>();
        }

        public enum ShortcutMode
        {
            None,
            // Construct, Destruct, Run, Stop, Consume, Provide, Consume_Undo, Provide_Undo, Consume_Provide, Provide_Consume_Undo, 
            ConstructDestruct, LinkUnlink, RunStop
        }
        public ShortcutMode CurrentShortcutMode { get; set; }



        [Header("Construct Destruct")]
        [SerializeField]
        private Sprite ConstructDestructButtonSprite_Activating;
        [SerializeField]
        private Sprite ConstructDestructButtonSprite;
        [SerializeField]
        private UnityEngine.UI.Image ConstructDestructButtonImage;
        public void SyncHammer() {
            ConstructDestructButtonImage.gameObject.SetActive(Globals.Ins.Bool<KnowledgeOfHammer>());
        }
        public void OnTapConstructDestruct() {
            if (CurrentShortcutMode == ShortcutMode.ConstructDestruct) {
                CurrentShortcutMode = ShortcutMode.None;
            } else {
                CurrentShortcutMode = ShortcutMode.ConstructDestruct;
            }
            SyncButtonsOutlines();
            MapView.InterceptInteractionOnce = true;
        }

        [Header("Link Unlink")]
        [SerializeField]
        private Sprite LinkUnlinkButtonSprite_Activating;
        [SerializeField]
        private Sprite LinkUnlinkButtonSprite;
        [SerializeField]
        private UnityEngine.UI.Image LinkUnlinkButtonImage;
        public void SyncMagnet() {
            LinkUnlinkButtonImage.gameObject.SetActive(Globals.Ins.Bool<KnowledgeOfMagnet>());
        }
        public void OnTapLinkUnlink() {
            if (CurrentShortcutMode == ShortcutMode.LinkUnlink) {
                CurrentShortcutMode = ShortcutMode.None;
            } else {
                CurrentShortcutMode = ShortcutMode.LinkUnlink;
            }
            SyncButtonsOutlines();
            MapView.InterceptInteractionOnce = true;
        }

        [Header("Run Stop")]
        [SerializeField]
        private Sprite RunStopButtonSprite_Activating;
        [SerializeField]
        private Sprite RunStopButtonSprite;
        [SerializeField]
        private UnityEngine.UI.Image RunStopButtonImage;
        public void OnTapRunStop() {
            if (CurrentShortcutMode == ShortcutMode.RunStop) {
                CurrentShortcutMode = ShortcutMode.None;
            } else {
                CurrentShortcutMode = ShortcutMode.RunStop;
            }
            SyncButtonsOutlines();
            MapView.InterceptInteractionOnce = true;
        }

        [Header("Shortcut")]
        [SerializeField]
        private Sprite ShortcutButtonSprite_Activating;
        [SerializeField]
        private Sprite ShortcutButtonSprite;
        [SerializeField]
        private UnityEngine.UI.Image ShortcutButtonImage;
        private void SyncButtonsOutlines() {
            bool noneMode = CurrentShortcutMode == ShortcutMode.None;
            bool constructDestruct = CurrentShortcutMode == ShortcutMode.ConstructDestruct;
            bool linkUnlink = CurrentShortcutMode == ShortcutMode.LinkUnlink;
            bool runStop = CurrentShortcutMode == ShortcutMode.RunStop;

            ShortcutButtonImage.sprite = (!noneMode && !constructDestruct && !linkUnlink && !runStop) ? ShortcutButtonSprite_Activating : ShortcutButtonSprite;

            ConstructDestructButtonImage.sprite = constructDestruct ? ConstructDestructButtonSprite_Activating : ConstructDestructButtonSprite;
            LinkUnlinkButtonImage.sprite = linkUnlink ? LinkUnlinkButtonSprite_Activating : LinkUnlinkButtonSprite;
            RunStopButtonImage.sprite = runStop ? RunStopButtonSprite_Activating : RunStopButtonSprite;
        }




        // 问号按钮
        public void OnTapQuest() {
            MapView.InterceptInteractionOnce = true;
            //MainQuest.Ins.OnTap();
        }

        // 点玩家自己
        public void OnTapPlayerInventory() {
            Vector2Int position = MapView.Ins.CharacterPosition;
            IMap map = MapView.Ins.TheOnlyActiveMap;
            if (map.Get(position) is PlanetLander planetLander) {
                // 防止玩家卡在一格位置
                planetLander.OnTap();
            } else {
                List<IUIItem> items = new List<IUIItem>();
                UIItem.AddEntireInventory(Globals.Ins.Inventory, items, OnTapPlayerInventory, true);
                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));
                items.Add(UIItem.CreateTimeProgress<Sanity>(Globals.Ins.Values));
                items.Add(UIItem.CreateValueProgress<Satiety>(Globals.Ins.Values));
                UI.Ins.ShowItems("【随身物品】", items);
            }
        }

        // 地图资源按钮
        public void OnTapInventoryOfResource() {
            MapView.InterceptInteractionOnce = true;
            List<IUIItem> items = new List<IUIItem>();
            UIItem.AddEntireInventory(MapView.Ins.TheOnlyActiveMap.Inventory, items, OnTapInventoryOfResource, true);
            items.Add(UIItem.CreateSeparator());
            UI.Ins.ShowItems("【星球资源仓库】", items);
        }

        // 地图盈余按钮
        public void OnTapInventoryOfSupply() {
            MapView.InterceptInteractionOnce = true;
            List<IUIItem> items = new List<IUIItem>();
            UIItem.AddEntireInventory(MapView.Ins.TheOnlyActiveMap.InventoryOfSupply, items, OnTapInventoryOfSupply, false);
            items.Add(UIItem.CreateSeparator());
            UI.Ins.ShowItems("【星球盈余产出】", items);
        }

        public void TryIncreaseGamePerformance() {
            int width;
            int height;
            if (Screen.width % UI.DefaultWidth == 0 && Screen.height % UI.DefaultHeight == 0) {
                width = Screen.width - UI.DefaultWidth;
                height = Screen.height - UI.DefaultHeight;
            } else {
                width = Screen.width / 2;
                height = Screen.height / 2;
            }

            if (width >= 640 && height >= 360) {
                Screen.SetResolution(Screen.width / 2, Screen.height / 2, true);
            }

        }

        // 齿轮按钮
        public void OnTapSettings() {
            MapView.InterceptInteractionOnce = true;

            IMap map = MapView.Ins.TheOnlyActiveMap;

            UI.Ins.ShowItems(Localization.Ins.Get<GameMenuLabel>(), new List<IUIItem>() {

#if UNITY_EDITOR
                
            UIItem.CreateButton($"改变作弊模式。当前{(GameConfig.CheatMode ? "开启" : "关闭")}", () => {
                GameConfig.CheatMode = !GameConfig.CheatMode;
                OnTapSettings();
            }),
#endif

            Sound.Ins.IsPlaying ? UIItem.CreateDynamicText(() => $"背景音乐《{Sound.Ins.PlayingMusicName}》播放中") : null,

                UIItem.CreateText($"当前分辨率 {Screen.width}x{Screen.height}"),

                UIItem.CreateStaticButton("提高游戏性能 (可能降低画质)", () => {
                    TryIncreaseGamePerformance();
                    SetFont(true);
                    UI.Ins.Active = false;
                }, Screen.width/2 >= 640 && Screen.height/2 >=360),

                UIItem.CreateStaticButton("提高游戏画质 (可能降低性能)", ()=>{
                    Screen.SetResolution(fullScreenWidth, fullScreenHeight, true);
                    UI.Ins.Active = false;
                }, !(Screen.width == fullScreenWidth && Screen.height == fullScreenHeight)),


                UIItem.CreateButton("打开教程：游戏介绍", SpecialPages.IntroPage),
                UIItem.CreateButton("打开教程：如何使用 “锤子” 工具按钮", SpecialPages.HowToUseHammerButton),
                UIItem.CreateButton("打开教程：如何使用 “磁铁” 工具按钮", SpecialPages.HowToUseMagnetButton),

                UIItem.CreateSeparator(),

                UIItem.CreateButton(Localization.Ins.Get<GameSettings>(), OpenGameSettingMenu),

                UIItem.CreateButton(Localization.Ins.Get<GameMenuSaveGame>(), OnTapSaveGameButton),

                UIItem.CreateButton(Localization.Ins.Get<GameMenuExitGame>(), UIDecorator.ConfirmBefore(() => Entry.ExitGame(), OnTapSettings)),

                UIItem.CreateButton(string.Format(Localization.Ins.Get<GameMenuLanguageLabel>(), Localization.Ins.Get<GameLanguage>()), () => {
                    Localization.Ins.SwitchNextLanguage();
                    OnTapSettings();
                }),

                new UIItem {
                    Type = IUIItemType.Image,
                    Content = "global",
                    LeftPadding = 0,
                    OnTap = () => {
                        Localization.Ins.SwitchNextLanguage();
                        OnTapSettings();
                    }
                }
            });
        }
        private void OnTapSaveGameButton() {
            Entry.SaveGame();
            UI.Ins.ShowItems("提示", new List<IUIItem> {
                UIItem.CreateText("已经保存"),
                UIItem.CreateReturnButton(OnTapSettings),
                UIItem.CreateButton(Localization.Ins.Get<GameMenuExitGame>(), Entry.ExitGame)
            });
        }

        private class UsePixelFont { }

        [Space]
        [SerializeField]
        private Font pixelFont;
        [SerializeField]
        private Font arialFont;
        [SerializeField]
        private GameObject[] objectsWithFonts;


        public void SetFont(bool pixel) {
            Globals.Ins.Bool<UsePixelFont>(pixel);
            SynchronizeFont();
        }
        public void ChangeFont() {
            Globals.Ins.Bool<UsePixelFont>(!Globals.Ins.Bool<UsePixelFont>());
        }
        public void SynchronizeFont() {
            Font fontUsed = Globals.Ins.Bool<UsePixelFont>() ? pixelFont : arialFont;
            // progressBar
            foreach (var obj in objectsWithFonts) {
                UnityEngine.UI.Text text = obj.GetComponent<UnityEngine.UI.Text>();
                if (text != null) {
                    text.font = fontUsed;
                    continue;
                }
                ProgressBar progressBar = obj.GetComponent<ProgressBar>();
                if (progressBar != null) {
                    progressBar.Text.font = fontUsed;
                    continue;
                }
                text = obj.GetComponentInChildren<UnityEngine.UI.Text>();
                if (text != null) {
                    text.font = fontUsed;
                }
                throw new Exception();
            }
            (UI.Ins as UI).Title.GetComponent<UnityEngine.UI.Text>().font = fontUsed;
            TileDescriptionForStandalone.font = fontUsed;
            Notification1Text.font = fontUsed;
            Notification2Text.font = fontUsed;
        }

        public void ToggleMusic() {
            bool result = !Globals.Ins.Bool<SoundMusicEnabled>();
            Globals.Ins.Bool<SoundMusicEnabled>(result);
            if (result) {
                Sound.Ins.PlayDefaultMusic();
            } else {
                Sound.Ins.StopDefaultMusic();
            }
            OpenGameSettingMenu();
        }

        private void OpenConsole() {
            var items = UI.Ins.GetItems();
            UI ui = UI.Ins as UI;
            if (ui == null) throw new Exception();

            items.Add(UIItem.CreateButton("提交输入", () => {

                // 控制台解析
                string input = ui.InputFieldContent;

                if (input.StartsWith("cheat")) {
                    if (!GameConfig.CheatMode) {
                        GameConfig.CheatMode = true;
                        UIPreset.Notify(OpenConsole, "作弊模式已激活（免费建造、免费科研）");
                    } else {
                        GameConfig.CheatMode = false;
                        UIPreset.Notify(OpenConsole, "作弊模式已关闭");
                    }
                } else if (input.StartsWith("help")) {
                    string[] results = input.Split(' ');
                    if (results.Length >= 2 && int.TryParse(results[1], out int arg) && arg > 0) {
                        MapView.Ins.TheOnlyActiveMap.Inventory.Add<Worker>(arg);
                        UIPreset.Notify(OpenConsole, $"已经获得worker {arg}");
                    } else {
                        UIPreset.Notify(OpenConsole, "help指令参数无效");
                    }
                } else if (input.StartsWith("add")) {
                    string[] results = input.Split(' ');
                    if (results.Length >= 3 && int.TryParse(results[2], out int arg) && arg > 0) {
                        Type type = Type.GetType("Weathering." + results[1]);
                        if (type != null && Tag.IsValidTag(type)) {
                            MapView.Ins.TheOnlyActiveMap.Inventory.Add(type, arg);
                            UIPreset.Notify(OpenConsole, $"已经获得 {Localization.Ins.Val(type, arg)} {arg}");
                        } else {
                            UIPreset.Notify(OpenConsole, "add指令type参数无效. 指令格式: add <type> <quantity>");
                        }
                    } else {
                        UIPreset.Notify(OpenConsole, "add指令参数无效. 指令格式: add <type> <quantity>");
                    }
                } else {
                    UIPreset.Notify(OpenConsole, "指令无效");
                }

            }));

            ui.ShowInputFieldNextTime = true;
            UI.Ins.ShowItems("打开控制台", items);
        }

        private const long minAutoSave = 15;
        private const long maxAutoSave = 600;

        private const long minDayNight = 10;
        private const long maxDayNight = 720;

        public void OpenGameSettingMenu() {
            UI.Ins.ShowItems(Localization.Ins.Get<GameSettings>(), new List<IUIItem>() {

                UIItem.CreateReturnButton(OnTapSettings),

                // UIItem.CreateButton("查看所有任务", () => MainQuest.Ins.ViewAllQuests(OpenGameSettingMenu)),

                UIItem.CreateButton("打开控制台", OpenConsole),

                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Globals.Ins.Bool<EnableLight>() ? $"光影效果：启用" : $"光影效果：禁用",
                    OnTap = () => {
                        Globals.Ins.Bool<EnableLight>(!Globals.Ins.Bool<EnableLight>());
                        SyncEnableLight();
                        OpenGameSettingMenu();
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Globals.Ins.Bool<UsePixelFont>() ? "当前字体：像素字体" : "当前字体：圆滑字体",
                    OnTap = () => {
                        ChangeFont();
                        SynchronizeFont();
                        OpenGameSettingMenu();
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Globals.Ins.Bool<ScreenAdaptation.DoubleSizeOption>() ? $"双倍视野：已开启" : $"双倍视野：已关闭",
                    OnTap = () => {
                        Globals.Ins.Bool<ScreenAdaptation.DoubleSizeOption>(!Globals.Ins.Bool<ScreenAdaptation.DoubleSizeOption>());
                        SyncDoubleSize();
                        OpenGameSettingMenu();
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Globals.Ins.Bool<InversedMovement>() ? $"控制反转：启用" : $"控制反转：禁用",
                    OnTap = () => {
                        Globals.Ins.Bool<InversedMovement>(!Globals.Ins.Bool<InversedMovement>());
                        SyncInversedMovement();
                        OpenGameSettingMenu();
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Globals.Ins.Bool<UtilityButtonsOnTheLeft>() ? $"按钮位置：左边" : $"按钮位置：右边",
                    OnTap = () => {
                        Globals.Ins.Bool<UtilityButtonsOnTheLeft>(!Globals.Ins.Bool<UtilityButtonsOnTheLeft>());
                        SyncUtilityButtonPosition();
                        OpenGameSettingMenu();
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Globals.Ins.Bool<LogisticsAnimationIsLinear>() ? $"物流动画：匀速" : $"物流动画：变速",
                    OnTap = () => {
                        Globals.Ins.Bool<LogisticsAnimationIsLinear>(!Globals.Ins.Bool<LogisticsAnimationIsLinear>());
                        SyncLogisticsAnimation();
                        OpenGameSettingMenu();
                    }
                },

                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Slider,
                    InitialSliderValue = (Globals.Ins.Values.Get<SecondsForADay>().Max-minDayNight)/(float)(maxDayNight-minDayNight),
                    DynamicSliderContent = (float x) => {
                        long interval = (long)(x*(maxDayNight-minDayNight)+minDayNight);
                        Globals.Ins.Values.Get<SecondsForADay>().Max = interval;
                        GlobalLight.Ins.SecondsForADay = interval;
                        SyncSecondsForADay();
                        return $"昼夜时长 {interval} 秒";
                    }
                },

                new UIItem {
                    Type = IUIItemType.Slider,
                    InitialSliderValue = (Globals.Ins.Values.Get<GameAutoSaveInterval>().Max-minAutoSave)/(float)(maxAutoSave-minAutoSave),
                    DynamicSliderContent = (float x) => {
                        long interval = (long)(x*(maxAutoSave-minAutoSave)+minAutoSave);
                        Globals.Ins.Values.Get<GameAutoSaveInterval>().Max = interval;
                        return $"自动存档间隔 {interval} 秒";
                    }
                },

                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Slider,
                    InitialSliderValue = Sound.Ins.GetDefaultSoundVolume(),
                    DynamicSliderContent = (float x) => {
                        Sound.Ins.SetDefaultSoundVolume(x);
                        return $"音效音量 {Math.Floor(x*100)}";
                    }
                },

                /// 游戏音效
                new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Globals.Ins.Bool<SoundEffectDisabled>() ? "音效：已关闭" : "音效：已开启",
                    OnTap = () => {
                        Globals.Ins.Bool<SoundEffectDisabled>(!Globals.Ins.Bool<SoundEffectDisabled>());
                    }
                },

                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Slider,
                    InitialSliderValue = Sound.Ins.GetDefaultMusicVolume(),
                    DynamicSliderContent = (float x) => {
                        Sound.Ins.SetDefaultMusicVolume(x);
                        return $"音乐音量 {Math.Floor(x*100)}";
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Globals.Ins.Bool<SoundMusicEnabled>() ? "音乐：已开启" : "音乐：已关闭",
                    OnTap = ToggleMusic
                },

                Sound.Ins.IsPlaying ? UIItem.CreateDynamicText(() => $"《{Sound.Ins.PlayingMusicName}》播放中") : null,

                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Slider,
                    InitialSliderValue = Mathf.InverseLerp(50, 200, Globals.Ins.Values.GetOrCreate<MapView.TappingSensitivity>().Max),
                    DynamicSliderContent = (float x) => {

                        int sensitivity = (int)(camerSensitivityFactor*(1.5f*x+0.5f));
                        Globals.Ins.Values.GetOrCreate<MapView.TappingSensitivity>().Max = sensitivity;
                        SyncCameraSensitivity();
                        return $"镜头灵敏度 {sensitivity}";
                    }
                },


                new UIItem {
                    Type = IUIItemType.Slider,
                    InitialSliderValue =Globals.Ins.Values.GetOrCreate<UserInterfaceBackgroundTransparency>().Max/userInterfaceBackgroundTransparencyFactor,
                    DynamicSliderContent = (float x) => {
                        float alpha = x*userInterfaceBackgroundTransparencyFactor;
                        Globals.Ins.Values.GetOrCreate<UserInterfaceBackgroundTransparency>().Max = (long)alpha;
                        SyncUserInterfaceBackgroundTransparency();
                        return $"背景透明度 {alpha}";
                    }
                },

                //UIItem.CreateSeparator(),

                //new UIItem {
                //    Type = IUIItemType.Button,
                //    Content = Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>() ? "获得资源时提示：已关闭" : "获得资源时提示：已开启",
                //    OnTap = () => {
                //        Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>(!Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>());
                //        OpenGameSettingMenu();
                //    }
                //},

                //new UIItem {
                //    Type = IUIItemType.Button,
                //    Content = Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>() ? "需求资源时提示：已关闭。推荐开启" : "需求资源时提示：已开启",
                //    OnTap = () => {
                //        Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>(!Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>());
                //        OpenGameSettingMenu();
                //    }
                //},

                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"还原默认设置",
                    OnTap = UIDecorator.ConfirmBefore(() => {
                        RestoreDefaultSettings();
                        SynchronizeSettings();
                        OpenGameSettingMenu();
                    }, OpenGameSettingMenu)
                },

                /// 重置存档
                new UIItem {
                    Content = Localization.Ins.Get<GameMenuResetGame>(),
                    Type = IUIItemType.Button,
                    OnTap = UIDecorator.ConfirmBefore(Entry.DeleteGameSave, OpenGameSettingMenu, "确认重置存档吗? 需要重启游戏"),
                }
            });
        }

    }
}

