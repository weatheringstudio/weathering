
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
            UI.Ins.UIItems("菜单", new List<IUIItem>() { 
            
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "这是游戏设置菜单"
                },
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Concept.Ins.ColoredNameOf<ExitGame>(),
                    OnTap = UIDecorator.ConfirmBefore(GameEntry.Ins.ExitGame)
                },

                new UIItem {
                    Content = "保存游戏",
                    Type = IUIItemType.Button,
                    OnTap = UIDecorator.InformAfter(GameEntry.Ins.Save, "已经保存"),
                },

                new UIItem {
                    Content = "重置存档",
                    Type = IUIItemType.Button,
                    OnTap = UIDecorator.ConfirmBefore(GameEntry.Ins.DeleteSave, "确认重置存档吗？"),
                }

            });
        }
    }
}

