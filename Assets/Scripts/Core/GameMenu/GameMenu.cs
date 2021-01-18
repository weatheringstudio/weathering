
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

            UI.Ins.UIItems("菜单", new List<IUIItem>() { 
            
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "这是游戏设置菜单"
                },

                //new UIItem {
                //    Content = Concept.Ins.ColoredNameOf<Sanity>(),
                //    Type = IUIItemType.ValueProgress,
                //    Value = map.Values.Get<Sanity>()
                //},
                UIItem.CreateValueProgress<Sanity>(map.Values),

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = "查看背包内容",
                    OnTap = () => {
                        List<IUIItem> items = new List<IUIItem>();
                        UIItem.AddInventory(map.Inventory, items);
                        UI.Ins.UIItems("背包", items);
                    }
                },

                new UIItem {
                    Type = IUIItemType.Image,
                    Content = "GrasslandBanner",
                    LeftPadding = 0,
                },

                new UIItem {
                    Content = Concept.Ins.ColoredNameOf<GameSettings>(),
                    Type = IUIItemType.Button,
                    OnTap = OpenGameSettingMenu
                },

                new UIItem {
                    Content = Concept.Ins.ColoredNameOf<SaveGame>(),
                    Type = IUIItemType.Button,
                    OnTap = UIDecorator.InformAfter(GameEntry.Ins.Save, "已经保存"),
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Concept.Ins.ColoredNameOf<ExitGame>(),
                    OnTap = UIDecorator.ConfirmBefore(GameEntry.Ins.ExitGame)
                },

            });
        }

        private void OpenGameSettingMenu() {
            UI.Ins.UIItems(Concept.Ins.ColoredNameOf<GameSettings>(), new List<IUIItem>() {
                new UIItem {
                    Content = Concept.Ins.ColoredNameOf<ReturnMenu>(),
                    Type = IUIItemType.Button,
                    OnTap = OnTap,
                },
                new UIItem {
                    Content = Concept.Ins.ColoredNameOf<ResetGame>(),
                    Type = IUIItemType.Button,
                    OnTap = UIDecorator.ConfirmBefore(GameEntry.Ins.DeleteSave, OpenGameSettingMenu, "确认重置存档吗？"),
                }
            });
        }

    }
}

