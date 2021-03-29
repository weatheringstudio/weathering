
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public static class SpecialPages
	{
		public static void OpenStartingPage() {
			UI.DontCloseOnIntroduction = true;
			(UI.Ins as UI).SetExitIndicatorVisible(false);
			AskFont();
        }
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
				Sound.Ins.PlayDefaultMusic();
				IntroPage();
			}));

			items.Add(UIItem.CreateButton("不播放音乐", () => {
				Sound.Ins.StopDefaultMusic();
				IntroPage();
			}));

			UI.Ins.ShowItems("是否播放背景音乐", items);
		}
		public static void IntroPage() {
			var items = new List<IUIItem>();

			items.Add(UIItem.CreateText("欢迎来到《挂机工厂》！"));

			// items.Add(UIItem.CreateMultilineText("玩家在游戏中，会模拟经营、角色扮演、建造沙盒、解锁科技、探索地图。"));

			items.Add(UIItem.CreateMultilineText("在这个版本里，建造电力工业和石油工业是一个后期目标"));

			items.Add(UIItem.CreateMultilineText("游戏支持离线挂机，即使关闭了游戏，游戏中的所有工厂仍然在运转"));

			items.Add(UIItem.CreateMultilineText("如果缺少资源，那么可以尝试提高生产效率，或者建立仓库，过几个小时再来进行游戏吧"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("六项教程"));

			items.Add(UIItem.CreateMultilineText("1 点击地块，与地块互动"));

			items.Add(UIItem.CreateMultilineText("2 点击屏幕右上角的 “?” 查看主线任务"));

			items.Add(UIItem.CreateMultilineText("3 点击屏幕右上角的 “文件夹” 查看物资"));

			items.Add(UIItem.CreateMultilineText("4 点击屏幕右上角的 “齿轮” 可再次打开此提示，并进行游戏设置"));

			items.Add(UIItem.CreateMultilineText("5 学习使用屏幕右方的 “锤子” 工具按钮，可以简化建筑的 建造、拆除、复制"));

			items.Add(UIItem.CreateMultilineText("6 学习使用屏幕右方的 “磁铁” 工具按钮，可以进行 输入和输出"));

			items.Add(UIItem.CreateButton("关闭教程", ClosingPage));

			UI.Ins.ShowItems("教程", items);
		}

		private static void ClosingPage() {

			(UI.Ins as UI).SetExitIndicatorVisible(true);

			var items = new List<IUIItem>();

			items.Add(UIItem.CreateMultilineText("游戏定时自动保存，如果直接退出游戏可能会让若干秒内操作重置，在设置页退出游戏可以自动保存"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("如果某项操作没反应，有可能是右上角的“文件夹” (即地图资源背包) 满了"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateText("点击右上角（或屏幕上方黑色区域）关闭此界面"));

			UI.DontCloseOnIntroduction = false;

			UI.Ins.ShowItems("开始游戏", items);
		}

		public static void HowToUseHammerButton() {
			var items = new List<IUIItem>();

			items.Add(UIItem.CreateMultilineText("教程还没做"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("大概用法是：点击空地，粘贴一个(刚刚造过的)同类建筑。点击建筑，可以拆除建筑(如果可以拆除)，并且复制此建筑。"));
			items.Add(UIItem.CreateReturnButton(GameMenu.Ins.OnTapSettings));


			UI.Ins.ShowItems("游戏即将开始", items);
		}

		public static void HowToUseMagnetButton() {
			var items = new List<IUIItem>();

			items.Add(UIItem.CreateMultilineText("教程还没做"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("大概用法是：点击建筑(包括道路)，把周围4格的东西都吸过来(如果能吸过来)。如果建筑已经吸引了东西，那么会把吸的东西还回去。"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("所以，沿着道路，可以把东西吸很远的距离。"));

			items.Add(UIItem.CreateReturnButton(GameMenu.Ins.OnTapSettings));

			UI.Ins.ShowItems("游戏即将开始", items);
		}
	}
}

