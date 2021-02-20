
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

	namespace First10Mins
    {
		public class RocketLanded { }
    }

	public class QuestProgress { }
	public static class MainQuest
	{
		public static void OnTap() {
			IRef quest = Globals.Ins.Refs.GetOrCreate<QuestProgress>();

			var items = UI.Ins.GetItems();
			string title = null;
			switch (quest.Type) {
				case null:
					items.Add(UIItem.CreateMultilineText("火箭在天上飞啊"));
					items.Add(UIItem.CreateMultilineText("火箭需要找一块平坦的地方着陆，不能在海洋、高山、森林着陆"));
					break;
				// case typeof(First10Mins.RocketLanded):
				default:
					break;
            }
			if (title == null) title = "【任务】";
			UI.Ins.ShowItems(title, items);
		}
	}
}

