    
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class GameMenu : MonoBehaviour
    {
        public static GameMenu Ins { get; private set; }

        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        public void OnTap() {
            IMap map = MapView.Ins.Map;

            UI.Ins.ShowItems("菜单", new List<IUIItem>() {

                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "这是游戏设置菜单"
                },

                UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values),

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = "查看背包内容",
                    OnTap = () => {
                        List<IUIItem> items = new List<IUIItem>();
                        UIItem.AddEntireInventory(map.Inventory, items);
                        UI.Ins.ShowItems("背包", items);
                    },
                    CanTap = () => {
                        return !(MapView.Ins.Map is MainMap);
                    }
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = "回到主地图",
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
                    Content = Concept.Ins.ColoredNameOf<GameSettings>(),
                    Type = IUIItemType.Button,
                    OnTap = OpenGameSettingMenu
                },

                new UIItem {
                    Content = Concept.Ins.ColoredNameOf<SaveGame>(),
                    Type = IUIItemType.Button,
                    OnTap = OnTapSaveGameButton,
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Concept.Ins.ColoredNameOf<ExitGame>(),
                    OnTap = UIDecorator.ConfirmBefore(() => GameEntry.Ins.ExitGame())
                },

            });
        }
        private void OnTapSaveGameButton() {
            GameEntry.Ins.SaveGame();
            UI.Ins.ShowItems("提示", new List<IUIItem> {
                UIItem.CreateText("已经保存"),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Concept.Ins.ColoredNameOf<ExitGame>(),
                    OnTap = () => GameEntry.Ins.ExitGame()
                }
            });
        }

        private void OpenGameSettingMenu() {
            UI.Ins.ShowItems(Concept.Ins.ColoredNameOf<GameSettings>(), new List<IUIItem>() {
                new UIItem {
                    Content = Concept.Ins.ColoredNameOf<ReturnMenu>(),
                    Type = IUIItemType.Button,
                    OnTap = OnTap,
                },
                new UIItem {
                    Content = Concept.Ins.ColoredNameOf<ResetGame>(),
                    Type = IUIItemType.Button,
                    OnTap = UIDecorator.ConfirmBefore(GameEntry.Ins.DeleteGameSave, OpenGameSettingMenu, "确认重置存档吗？"),
                }
            });
        }

    }
}

