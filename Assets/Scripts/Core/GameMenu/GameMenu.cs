
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
    public class UtilityButtonsOnTheLeft { }

    [Concept]
    public class LogisticsAnimationIsLinear { }


    public interface ITileDescription
    {
        string TileDescription { get; }
    }

    public interface IGameEntry
    {
        void EnterMap(Type type);
        void SaveGame();
        void TrySaveGame();
        void DeleteGameSave();

        void ExitGame();
        void ExitGameUnsaved();
    }


    public class GameMenu : MonoBehaviour
    {
        public static IGameEntry Entry { get; set; }

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

#if UNITY_STANDALONE
            IsInStandalone = true;
            IsInMobile = false;
#else
            IsInStandalone = false;
            IsInMobile = true;
#endif
        }

        [SerializeField]
        private UnityEngine.UI.Text TileDescriptionForStandalone;
        public void SetTileDescriptionForStandalong(string text) {
            TileDescriptionForStandalone.text = text;
        }

        private void Start() {
            SynchronizeSettings();
            SyncButtonsOutlines();
            TileDescriptionForStandalone.gameObject.SetActive(IsInStandalone);
        }
        public static void OnConstruct() {
            RestoreDefaultSettings();
            if (!GameConfig.CheatMode) {
                SpecialPages.OpenStartingPage();
            }
            // Sound.Ins.PlayRandomMusic();
        }

        public static void RestoreDefaultSettings() {
            // 现在习惯把和游戏设置有关，游戏逻辑无关的初始化过程，放到GameMenu。和游戏逻辑有关的放到GameConfig

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

            Globals.Ins.Bool<SoundEffectDisabled>(false);
            Globals.Ins.Bool<SoundMusicEnabled>(true);

            globals.Values.GetOrCreate<MapView.TappingSensitivity>().Max = 100;

            Globals.Ins.Bool<UsePixelFont>(true);

            Globals.Ins.Bool<UtilityButtonsOnTheLeft>(false);
            Globals.Ins.Bool<LogisticsAnimationIsLinear>(false);
        }

        public void SynchronizeSettings() {
            SynchronizeFont();
            //SyncSFXVolume();
            //SyncMusicVolume();
            SyncCameraSensitivity();
            SyncDoubleSize();
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

        private void SyncCameraSensitivity() {
            MapView.Ins.TappingSensitivityFactor = MapView.DefaultTappingSensitivity * (Globals.Ins.Values.GetOrCreate<MapView.TappingSensitivity>().Max / 100f);
        }
        private void SyncDoubleSize() {
            ScreenAdaptation.Ins.DoubleSize = Globals.Ins.Bool<ScreenAdaptation.DoubleSizeOption>();
        }
        private void SyncUtilityButtonPosition() {
            if (LinkUnlinkButtonImage.transform is RectTransform rect) {
                rect.anchoredPosition = new Vector2(Globals.Ins.Bool<UtilityButtonsOnTheLeft>() ? (72 - 640) : 0, rect.anchoredPosition.y);
            }
            if (ConstructDestructButtonImage.transform is RectTransform rect2) {
                rect2.anchoredPosition = new Vector2(Globals.Ins.Bool<UtilityButtonsOnTheLeft>() ? (72 - 640) : 0, rect2.anchoredPosition.y);
            }
        }
        public static bool IsLinear = false;
        private void SyncLogisticsAnimation() {
            IsLinear = Globals.Ins.Bool<LogisticsAnimationIsLinear>();
        }

        public enum ShortcutMode
        {
            None, Construct, Destruct, Run, Stop, Consume, Provide, Consume_Undo, Provide_Undo, Consume_Provide, Provide_Consume_Undo, ConstructDestruct, LinkUnlink, RunStop
        }
        public ShortcutMode CurrentShortcutMode { get; set; }



        [Header("Construct Destruct")]
        [SerializeField]
        private Sprite ConstructDestructButtonSprite_Activating;
        [SerializeField]
        private Sprite ConstructDestructButtonSprite;
        [SerializeField]
        private UnityEngine.UI.Image ConstructDestructButtonImage;
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

        // 快捷按钮
        public void OnTapShortcut() {
            MapView.InterceptInteractionOnce = true;

            if (CurrentShortcutMode == ShortcutMode.None) {
                var items = UI.Ins.GetItems();

                //items.Add(UIItem.CreateText("选择工具"));

                //items.Add(UIItem.CreateButton("取消", () => { OnTapShortcut(); }));

                items.Add(UIItem.CreateMultilineText("建议使用左边三个按钮，因为此扳手按钮比较麻烦，而左边三个按钮够用了"));
                items.Add(UIItem.CreateMultilineText("左边三个按钮，从左到右，分别用于“建造和拆除” “物流与取消” “启动与关闭”"));
                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton("建造和拆除", () => {
                    CurrentShortcutMode = ShortcutMode.ConstructDestruct;
                    AfterSetMode();
                }));
                items.Add(UIItem.CreateButton("物流与取消", () => {
                    CurrentShortcutMode = ShortcutMode.LinkUnlink;
                    AfterSetMode();
                }));
                items.Add(UIItem.CreateButton("启动与关闭", () => {
                    CurrentShortcutMode = ShortcutMode.RunStop;
                    AfterSetMode();
                }));

                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton("建造", () => {
                    if (UIItem.ShortcutType == null) {
                        UI.Ins.ShowItems("提示", UIItem.CreateMultilineText("快速建造前，需要手动建造一个此类型建筑，以确定建筑类型"));
                        OnTapShortcut();
                    } else {
                        CurrentShortcutMode = ShortcutMode.Construct;
                        AfterSetMode();
                    }
                }));

                items.Add(UIItem.CreateButton("拆除", () => {
                    CurrentShortcutMode = ShortcutMode.Destruct;
                    AfterSetMode();
                }));

                items.Add(UIItem.CreateText("输入和输出工具"));

                items.Add(UIItem.CreateButton("开始输入输出", () => {
                    CurrentShortcutMode = ShortcutMode.Consume_Provide;
                    AfterSetMode();
                }));

                items.Add(UIItem.CreateButton("停止输入输出", () => {
                    CurrentShortcutMode = ShortcutMode.Provide_Consume_Undo;
                    AfterSetMode();
                }));

                items.Add(UIItem.CreateText("启动和停止工具"));

                items.Add(UIItem.CreateButton("启动", () => {
                    CurrentShortcutMode = ShortcutMode.Run;
                    AfterSetMode();
                }));

                items.Add(UIItem.CreateButton("停止", () => {
                    CurrentShortcutMode = ShortcutMode.Stop;
                    AfterSetMode();
                }));


                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateText("更多工具"));

                items.Add(UIItem.CreateButton("输入", () => {
                    CurrentShortcutMode = ShortcutMode.Consume;
                    AfterSetMode();
                }));

                items.Add(UIItem.CreateButton("输出", () => {
                    CurrentShortcutMode = ShortcutMode.Provide;
                    AfterSetMode();
                }));

                items.Add(UIItem.CreateButton("停止输入", () => {
                    CurrentShortcutMode = ShortcutMode.Consume_Undo;
                    AfterSetMode();
                }));

                items.Add(UIItem.CreateButton("停止输出", () => {
                    CurrentShortcutMode = ShortcutMode.Provide_Undo;
                    AfterSetMode();
                }));

                UI.Ins.ShowItems("选择功能", items);
            } else {
                CurrentShortcutMode = ShortcutMode.None;
                MapView.InterceptInteractionOnce = true;
            }
        }
        private void AfterSetMode() {
            MapView.InterceptInteractionOnce = false;
            UI.Ins.Active = false;
            SyncButtonsOutlines();
        }

        // 问号按钮
        public void OnTapQuest() {
            MapView.InterceptInteractionOnce = true;
            MainQuest.Ins.OnTap();
        }

        // 点玩家自己
        public void OnTapPlayerInventory() {
            Vector2Int position = MapView.Ins.CharacterPosition;
            IMap map = MapView.Ins.TheOnlyActiveMap;
            if (map.Get(position) is PlanetLander planetLander) {
                // 防止玩家卡在一格位置
                planetLander.OnStepOn();
            } else {
                List<IUIItem> items = new List<IUIItem>();
                UIItem.AddEntireInventory(Globals.Ins.Inventory, items, OnTapPlayerInventory);
                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));
                items.Add(UIItem.CreateTimeProgress<Sanity>(Globals.Ins.Values));
                UI.Ins.ShowItems("【随身物品】", items);
            }
            // 新功能：吃饭，补充体力。体力用来临时运转工厂
        }

        // 箱子按钮
        public void OnTapMapInventory() {
            MapView.InterceptInteractionOnce = true;
            List<IUIItem> items = new List<IUIItem>();
            UIItem.AddEntireInventory(MapView.Ins.TheOnlyActiveMap.Inventory, items, OnTapMapInventory);
            items.Add(UIItem.CreateSeparator());
            UI.Ins.ShowItems("【地图资源】", items);
        }

        // 齿轮按钮
        public void OnTapSettings() {
            MapView.InterceptInteractionOnce = true;

            IMap map = MapView.Ins.TheOnlyActiveMap;
            Type mainMap = typeof(MainMap);

            UI.Ins.ShowItems(Localization.Ins.Get<GameMenuLabel>(), new List<IUIItem>() {

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
                string input = ui.GetInputFieldContent;

                if (input.StartsWith("cheat")) {
                    if (!GameConfig.CheatMode) {
                        GameConfig.CheatMode = true;
                        UIPreset.Notify(OpenConsole, "作弊模式已激活（免费建造）");
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
        private const long maxAutiSave = 600;
        public void OpenGameSettingMenu() {
            UI.Ins.ShowItems(Localization.Ins.Get<GameSettings>(), new List<IUIItem>() {

                UIItem.CreateReturnButton(OnTapSettings),

                UIItem.CreateButton("查看所有任务", () => MainQuest.Ins.ViewAllQuests(OpenGameSettingMenu)),

                UIItem.CreateButton("打开控制台", OpenConsole),

                UIItem.CreateSeparator(),

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
                    InitialSliderValue = (Globals.Ins.Values.Get<GameAutoSaveInterval>().Max-minAutoSave)/(float)(maxAutiSave-minAutoSave),
                    DynamicSliderContent = (float x) => {
                        long interval = (long)(x*(maxAutiSave-minAutoSave)+minAutoSave);
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
                        int sensitivity = (int)(50f + x*(200f-50f));
                        Globals.Ins.Values.GetOrCreate<MapView.TappingSensitivity>().Max = sensitivity;
                        SyncCameraSensitivity();
                        return $"镜头灵敏度 {sensitivity}";
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
                    OnTap = UIDecorator.ConfirmBefore(Entry.DeleteGameSave, OpenGameSettingMenu, "确认重置存档吗？需要重启游戏"),
                }
            });
        }

    }
}

