
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
            IntroPage();
        }
		private static void AskFont() {
			GameMenu.Ins.SynchronizeFont();

			var items = new List<IUIItem>();

			items.Add(UIItem.CreateText($"可选字体: 1 像素字体 2 平滑字体"));

			items.Add(UIItem.CreateButton("切换下一种字体", () => {
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

			int musicCount = (Sound.Ins as Sound).GetMusicCount();

			items.Add(UIItem.CreateText($"共有背景音乐 {musicCount} 首"));

			if (musicCount < 5) items.Add(UIItem.CreateMultilineText("检测到音乐数量过少, 可能当前游戏版本是压缩版"));

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

			// string color = "<color=#ff6666ff>";

			var items = new List<IUIItem>();

			items.Add(UIItem.CreateText("欢迎来到《挂机工厂》! "));

			// items.Add(UIItem.CreateMultilineText("玩家在游戏中, 会模拟经营、角色扮演、建造沙盒、解锁科技、探索地图。"));

			items.Add(UIItem.CreateMultilineText("在这个版本里, 可以在殖民各种星球, 把人送入太空, 建立太空帝国"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("游戏支持离线挂机, 即使关闭了游戏, 游戏中的所有工厂仍然在运转。"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("直接关闭游戏会让一定时间内的所有操作无效, 点击右上角设置，可以保存并退出。在设置菜单中也可以进行字体、音量等更多设置。"));

			// items.Add(UIItem.CreateMultilineText("如果缺少资源, 那么可以尝试扩大生产规模, 或者建立更多仓库自动收集资源"));

			//items.Add(UIItem.CreateSeparator());

			//items.Add(UIItem.CreateMultilineText("七大教程"));

			//items.Add(UIItem.CreateMultilineText($"1 {color}拖拽</color>屏幕, 移动飞船, 或移动人物"));

			//items.Add(UIItem.CreateMultilineText($"2 {color}点击</color>屏幕, 与平原、森林、山地、海洋互动"));

			//items.Add(UIItem.CreateMultilineText($"3 点击屏幕右上角的 “{color}文件夹</color>” 查看{color}物资</color>(即查看背包)"));

			//items.Add(UIItem.CreateMultilineText($"4 点击屏幕右上角的 “{color}?</color>” 查看主线{color}任务</color>"));

			//items.Add(UIItem.CreateMultilineText($"5 点击屏幕右上角的 “{color}齿轮</color>” 可以进行游戏{color}设置</color>, 也可以再次打开此教程"));

			//items.Add(UIItem.CreateMultilineText($"6 学习使用屏幕右方的 “{color}锤子</color>” 工具按钮, 可以简化建筑的 {color}建造</color>、{color}拆除</color>、{color}复制</color>"));

			//items.Add(UIItem.CreateMultilineText($"7 学习使用屏幕右方的 “{color}磁铁</color>” 工具按钮, 可以简化建筑的 {color}输入</color>和{color}输出</color>"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateButton("已阅, 关闭教程", ClosingPage));

			UI.Ins.ShowItems("教程", items);
		}

		private static void ClosingPage() {

			(UI.Ins as UI).SetExitIndicatorVisible(true);

			var items = new List<IUIItem>();

			//items.Add(UIItem.CreateSeparator());

			//items.Add(UIItem.CreateMultilineText("如果某项操作没反应, 有可能是右上角的“文件夹” (即地图资源背包) 满了"));

			items.Add(UIItem.CreateMultilineText($"点击平原, 建造一个{Localization.Ins.Get<TotemOfNature>()}, 开始游戏"));

			items.Add(UIItem.CreateText("点击屏幕上方半透明黑色区域, 关闭此界面"));

			UI.DontCloseOnIntroduction = false;

			UI.Ins.ShowItems("开始游戏", items);
		}

		public static void HowToUseHammerButton() {
			var items = new List<IUIItem>();

			items.Add(UIItem.CreateMultilineText("教程还没做"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("大概用法是：点击空地, 粘贴一个(刚刚造过的)同类建筑。点击建筑, 可以拆除建筑(如果可以拆除), 并且复制此建筑。"));
			items.Add(UIItem.CreateReturnButton(GameMenu.Ins.OnTapSettings));


			UI.Ins.ShowItems("游戏即将开始", items);
		}

		public static void HowToUseMagnetButton() {
			var items = new List<IUIItem>();

			items.Add(UIItem.CreateMultilineText("教程还没做"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("大概用法是：点击建筑(包括道路), 把周围4格的东西都吸过来(如果能吸过来)。如果建筑已经吸引了东西, 那么会把吸的东西还回去。"));

			items.Add(UIItem.CreateSeparator());

			items.Add(UIItem.CreateMultilineText("所以, 沿着道路, 可以把东西吸很远的距离。"));

			items.Add(UIItem.CreateReturnButton(GameMenu.Ins.OnTapSettings));

			UI.Ins.ShowItems("游戏即将开始", items);
		}
	}
}

