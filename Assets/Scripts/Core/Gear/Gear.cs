
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public class Gear : MonoBehaviour
	{
		public static Gear Ins { get; private set; }

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
                }
            
            });
        }
    }
}

