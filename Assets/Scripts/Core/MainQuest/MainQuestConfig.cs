
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    namespace Quest
    {
        public class CongratulationsQuestAllCompleted { }
        public class LandRocket { }
        public class ExplorePlanet { }
        public class GatherFood { }

    }

    public static class MainQuestConfig
	{
        public static List<Type> QuestSequence { get; } = new List<Type> {
            typeof(Quest.LandRocket),
            typeof(Quest.ExplorePlanet),
            typeof(Quest.CongratulationsQuestAllCompleted),
        };


        // 任务在此配置
        public static Dictionary<Type, Func<List<IUIItem>, string>> OnTapQuest { get; } = new Dictionary<Type, Func<List<IUIItem>, string>>() {

            { typeof(Quest.LandRocket), items => {
                items.Add(UIItem.CreateMultilineText("飞船正在环绕星球飞行，可以找一块平原降落。"));
                items.Add(UIItem.CreateMultilineText("（如何降落？点击想要降落的平原）"));
                return "登陆星球";
            } },

            { typeof(Quest.ExplorePlanet), items => {
                items.Add(UIItem.CreateMultilineText("飞船已经降落，需要调查当地环境"));
                items.Add(UIItem.CreateText("任务目标：调查平原"));
                items.Add(UIItem.CreateText("任务目标：调查森林"));
                items.Add(UIItem.CreateText("任务目标：调查高山"));
                items.Add(UIItem.CreateMultilineText("（如何调查环境？走近想调查的地块，然后点击地块）"));
                items.Add(UIItem.CreateMultilineText("（如果降落的地方没有高山或森林怎么办？走进飞船，重新起飞）"));
                return "调查星球";
            } },
        };
    }
}

