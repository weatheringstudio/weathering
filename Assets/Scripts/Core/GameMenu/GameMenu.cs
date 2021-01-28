
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

    public class GameMenu : MonoBehaviour
    {
        public static GameMenu Ins { get; private set; }

        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        public void OnTap() {
            // 点齿轮时
            Sound.Ins.PlayDefaultSound();

            IMap map = MapView.Ins.Map;

            UI.Ins.ShowItems(Localization.Ins.Get<GameMenuLabel>(), new List<IUIItem>() {

                UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values),
                UIItem.CreateTimeProgress<Sanity>(Globals.Ins.Values),

                UIItem.CreateSeparator(),

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Get<GameMenuInspectInventory>(),
                    OnTap = () => {
                        UIPreset.ShowInventory(OnTap, map.Inventory);
                    },
                    CanTap = () => {
                        return !(MapView.Ins.Map is MainMap);
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Get<GameMenuGotoMainMap>(),
                    OnTap = () => {
                        GameEntry.Ins.EnterMap(typeof(MainMap));
                        UI.Ins.Active = false;
                    },
                    CanTap = () => {
                        return !(MapView.Ins.Map is MainMap);
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
                    OnTap = UIDecorator.ConfirmBefore(() => GameEntry.Ins.ExitGame())
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => string.Format(Localization.Ins.Get<GameMenuLanguageLabel>(), Localization.Ins.Get<GameLanguage>()),
                    OnTap = () => {
                        Localization.Ins.SwitchNextLanguage();
                        OnTap();
                    }
                },

                new UIItem {
                    Type = IUIItemType.Image,
                    Content = "global",
                    LeftPadding = 0,
                    OnTap = () => {
                        Localization.Ins.SwitchNextLanguage();
                        OnTap();
                    }
                },
            });
        }
        private void OnTapSaveGameButton() {
            GameEntry.Ins.SaveGame();
            UI.Ins.ShowItems("提示", new List<IUIItem> {
                UIItem.CreateText("已经保存"),
                UIItem.CreateReturnButton(OnTap),
                UIItem.CreateButton(Localization.Ins.Get<GameMenuExitGame>(), () => GameEntry.Ins.ExitGame())
            });
        }

        private void OpenGameSettingMenu() {
            UI.Ins.ShowItems(Localization.Ins.Get<GameSettings>(), new List<IUIItem>() {
                UIItem.CreateReturnButton(OnTap),

                /// 游戏音效
                new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Globals.Ins.Bool<SoundEffectDisabled>() ? "音效：已关闭" : "音效：已开启",
                    OnTap = () => {
                        Globals.Ins.Bool<SoundEffectDisabled>(!Globals.Ins.Bool<SoundEffectDisabled>());
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>() ? "获得资源时提示：已关闭" : "获得资源时提示：已开启",
                    OnTap = () => {
                        Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>(!Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>());
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>() ? "需求资源时提示：已关闭。建议开启" : "需求资源时提示：已开启",
                    OnTap = () => {
                        Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>(!Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>());
                    }
                },

                /// 重置存档
                new UIItem {
                    Content = Localization.Ins.Get<GameMenuResetGame>(),
                    Type = IUIItemType.Button,
                    OnTap = UIDecorator.ConfirmBefore(GameEntry.Ins.DeleteGameSave, OpenGameSettingMenu, "确认重置存档吗？"),
                }
            });
        }

    }
}

