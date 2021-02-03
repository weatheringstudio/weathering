
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public static class SpecialPages
	{
		public static void AskFont() {
			var items = new List<IUIItem>();

			items.Add(UIItem.CreateText("可以在游戏设置里更改"));

			items.Add(UIItem.CreateButton("尝试另一种字体", () => {
				GameMenu.Ins.ChangeFont();
				GameMenu.Ins.SynchronizeFont();
				AskFont();
			}));

			items.Add(UIItem.CreateButton("使用当前字体", () => {
				GameMenu.Ins.SynchronizeFont();
				AskBGM();
			}));

			UI.Ins.ShowItems("是否切换字体", items);
		}
		private static void AskBGM() {
			var items = new List<IUIItem>();

			items.Add(UIItem.CreateText("可以在游戏设置里更改"));

			items.Add(UIItem.CreateButton("播放音乐", () => {
				GameMenu.Ins.PlayDefaultMusic();
				IntroPage();
			}));

			items.Add(UIItem.CreateButton("不播放音乐", () => {
				IntroPage();
			}));

			UI.Ins.ShowItems("是否播放背景音乐", items);
		}
		public static void IntroPage() {
			var items = new List<IUIItem>();

			items.Add(UIItem.CreateText("欢迎来到《挂机工厂》！"));

			items.Add(UIItem.CreateMultilineText("这是一个将有丰富内容和玩法的游戏。玩家在游戏中，会模拟经营、角色扮演、建造沙盒、解锁科技、探索地图。"));

			items.Add(UIItem.CreateMultilineText("游戏支持离线挂机，即使关闭了游戏，游戏中的所有工厂仍然在运转。如果缺少资源，那么可以尝试提高生产效率，或是过几个小时再来进行游戏吧。"));


			items.Add(UIItem.CreateMultilineText("不幸的是，目前游戏还没有完整的流程，没有游戏性，只能体验和测试一些操作和功能。"));

			items.Add(UIItem.CreateMultilineText("刚开始游戏时，可以建造几个浆果丛或是猎场，自动获取食物资源。然后建造村庄，为村庄提供食物，让村民去种田、采矿、探索、战斗，获取各种资源。"));

			items.Add(UIItem.CreateMultilineText("游戏30秒每自动保存，如果直接退出游戏可能会让30秒内的操作重置，在设置页退出游戏可以自动保存"));

			items.Add(UIItem.CreateText("点击右上角（或屏幕上方黑色区域）关闭此界面"));

			UI.Ins.ShowItems("游戏即将开始！", items);
		}
	}
}

