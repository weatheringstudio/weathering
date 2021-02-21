
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public static class MainQuestConfig
	{
        public static List<Type> QuestSequence { get; } = new List<Type> {
            typeof(Quest.LandRocket),
            typeof(Quest.GatherLocalResources),
            typeof(Quest.CongratulationsQuestAllCompleted),
        };

        // 任务在此配置
        public static Dictionary<Type, Func<List<IUIItem>, string>> OnTapQuest { get; } = new Dictionary<Type, Func<List<IUIItem>, string>>() {

            { typeof(Quest.LandRocket), items => {
                items.Add(UIItem.CreateMultilineText("飞船正在环绕星球飞行，可以找一块平原降落。"));
                return "登陆星球";
            } },

            { typeof(Quest.GatherLocalResources), items => {
                items.Add(UIItem.CreateMultilineText("飞船已经降落，需要调查当地环境"));
                items.Add(UIItem.CreateText("任务目标：调查平原"));
                items.Add(UIItem.CreateText("任务目标：调查森林"));
                items.Add(UIItem.CreateText("任务目标：调查高山"));
                return "调查星球";
            } },
        };
    }
}

