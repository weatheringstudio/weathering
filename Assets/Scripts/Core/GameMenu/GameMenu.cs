
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

        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        private void Start() {
            SynchronizeSettings();
        }

        public void SynchronizeSettings() {
            // 字体设置
            Globals.Ins.Bool<UsePixelFont>(true);
            Ins.SynchronizeFont();
            SyncSFXVolume();
            SyncMusicVolume();
            SyncCameraSensitivity();
        }

        private void SyncMusicVolume() {
            Sound.Ins.SetDefaultMusicVolume(Sound.Ins.GetDefaultMusicVolume());
        }

        private void SyncSFXVolume() {
            Sound.Ins.SetDefaultSoundVolume(Sound.Ins.GetDefaultSoundVolume());
        }

        private void SyncCameraSensitivity() {
            MapView.Ins.TappingSensitivityFactor = MapView.DefaultTappingSensitivity * (Globals.Ins.Values.GetOrCreate<MapView.TappingSensitivity>().Max / 100f);
        }

        public void OnTapInventory() {
            UIPreset.ShowInventory(null, MapView.Ins.Map.Inventory);
        }

        public void OnTapSettings() {

            IMap map = MapView.Ins.Map;
            Type mainMap = typeof(MainMap);

            UI.Ins.ShowItems(Localization.Ins.Get<GameMenuLabel>(), new List<IUIItem>() {

                UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values),
                UIItem.CreateTimeProgress<Sanity>(Globals.Ins.Values),

                UIItem.CreateSeparator(),


                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Get<GameMenuInspectInventory>(),
                    OnTap = () => {
                        UIPreset.ShowInventory(OnTapSettings, map.Inventory);
                    },
                    //CanTap = () => {
                    //    return !(MapView.Ins.Map.GetType() == mainMap);
                    //}
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Get<GameMenuGotoMainMap>(),
                    OnTap = () => {
                        Entry.EnterMap(mainMap);
                        UI.Ins.Active = false;
                    },
                    CanTap = () => {
                        return !(MapView.Ins.Map.GetType() == mainMap);
                    }
                },

                UIItem.CreateSeparator(),

                new UIItem {
                    Content = Localization.Ins.Get<GameSettings>(),
                    Type = IUIItemType.Button,
                    OnTap = OpenGameSettingMenu
                },

                new UIItem {
                    Content = Localization.Ins.Get<GameMenuSaveGame>(),
                    Type = IUIItemType.Button,
                    OnTap = OnTapSaveGameButton,
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Get<GameMenuExitGame>(),
                    OnTap = UIDecorator.ConfirmBefore(() => Entry.ExitGame(), OnTapSettings)
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => string.Format(Localization.Ins.Get<GameMenuLanguageLabel>(), Localization.Ins.Get<GameLanguage>()),
                    OnTap = () => {
                        Localization.Ins.SwitchNextLanguage();
                        OnTapSettings();
                    }
                },

                new UIItem {
                    Type = IUIItemType.Image,
                    Content = "global",
                    LeftPadding = 0,
                    OnTap = () => {
                        Localization.Ins.SwitchNextLanguage();
                        OnTapSettings();
                    }
                },
            }); ;
        }
        private void OnTapSaveGameButton() {
            Entry.SaveGame();
            UI.Ins.ShowItems("提示", new List<IUIItem> {
                UIItem.CreateText("已经保存"),
                UIItem.CreateReturnButton(OnTapSettings),
                UIItem.CreateButton(Localization.Ins.Get<GameMenuExitGame>(), () => Entry.ExitGame())
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
        }

        public void PlayDefaultMusic() {
            bool result = !Globals.Ins.Bool<SoundMusicEnabled>();
            Globals.Ins.Bool<SoundMusicEnabled>(result);
            if (result) {
                Sound.Ins.PlayDefaultMusic();
            } else {
                Sound.Ins.StopDefaultMusic();
            }
        }

        private void OpenGameSettingMenu() {
            string fontLabell = Globals.Ins.Bool<UsePixelFont>() ? "像素字体" : "圆滑字体";
            UI.Ins.ShowItems(Localization.Ins.Get<GameSettings>(), new List<IUIItem>() {

                UIItem.CreateReturnButton(OnTapSettings),

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = "关于游戏",
                    OnTap = SpecialPages.IntroPage
                },



                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => $"字体：已使用{fontLabell}",
                    OnTap = () => {
                        ChangeFont();
                        SynchronizeFont();
                        OpenGameSettingMenu();
                    }
                },

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
                    DynamicContent = () => Globals.Ins.Bool<SoundMusicEnabled>() ? "音乐：已开启" : "音乐：已关闭",
                    OnTap = PlayDefaultMusic
                },

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

                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>() ? "获得资源时提示：已关闭" : "获得资源时提示：已开启",
                    OnTap = () => {
                        Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>(!Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>());
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>() ? "需求资源时提示：已关闭。推荐开启" : "需求资源时提示：已开启",
                    OnTap = () => {
                        Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>(!Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>());
                    }
                },

                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"还原默认设置",
                    OnTap = UIDecorator.ConfirmBefore(() => {
                        GameConfig.RestoreDefaultSettings();
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

