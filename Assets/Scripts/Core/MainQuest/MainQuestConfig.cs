
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class Quest_CongratulationsQuestAllCompleted { }
    [Concept]
    public class Quest_LandRocket { }
    [Concept]
    public class Quest_ExplorePlanet { }
    [Concept]
    public class Quest_GatherFood { }

    public class SubQuest_ExplorePlanet_Forest { }
    public class SubQuest_ExplorePlanet_Mountain { }
    public class SubQuest_ExplorePlanet_Plain { }

    public class MainQuestConfig : MonoBehaviour
    {
        public static MainQuestConfig Ins { get; private set; }

        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;

            OnTapQuest = new Dictionary<Type, Action<List<IUIItem>>>();
            CheckQuestCanBeCompleted = new Dictionary<Type, Func<bool>>();
            CreateOnTapQuest();
        }
        public Dictionary<Type, Action<List<IUIItem>>> OnTapQuest { get; private set; }
        public Dictionary<Type, Func<bool>> CheckQuestCanBeCompleted { get; private set; }

        public List<Type> QuestSequence { get; } = new List<Type> {
            typeof(Quest_LandRocket),
            typeof(Quest_ExplorePlanet),
            typeof(Quest_CongratulationsQuestAllCompleted),
        };

        private const string completed = "【 √ 】";
        private const string notCompleted = "【 × 】";
        private string CompletionLabel<T>() {
            return Globals.Ins.Bool<T>() ? completed : notCompleted;
        }
        private void CreateOnTapQuest() {
            OnTapQuest.Add(typeof(Quest_CongratulationsQuestAllCompleted), items => {
                items.Add(UIItem.CreateMultilineText("已经完成了全部任务！此任务无法完成，并且没有更多任务了"));
            });
            OnTapQuest.Add(typeof(Quest_LandRocket), items => {
                items.Add(UIItem.CreateMultilineText("飞船正在环绕星球飞行，可以找一块平原降落。"));
                items.Add(UIItem.CreateMultilineText("（如何降落？）点击想要降落的平原"));
            });
            Func<bool> Quest_ExplorePlanet_CanBeCompleted = () => {
                return Globals.Ins.Bool<SubQuest_ExplorePlanet_Forest>()
                && Globals.Ins.Bool<SubQuest_ExplorePlanet_Mountain>()
                && Globals.Ins.Bool<SubQuest_ExplorePlanet_Plain>();
            };
            CheckQuestCanBeCompleted.Add(typeof(Quest_ExplorePlanet), Quest_ExplorePlanet_CanBeCompleted);
            OnTapQuest.Add(typeof(Quest_ExplorePlanet), items => {
                items.Add(UIItem.CreateMultilineText("飞船已经降落，需要调查当地环境"));
                items.Add(UIItem.CreateText($"{(CompletionLabel<SubQuest_ExplorePlanet_Plain>())} 任务目标：调查平原 "));
                items.Add(UIItem.CreateText($"{(CompletionLabel<SubQuest_ExplorePlanet_Forest>())} 任务目标：调查森林 "));
                items.Add(UIItem.CreateText($"{(CompletionLabel<SubQuest_ExplorePlanet_Mountain>())} 任务目标：调查高山 "));
                if (Globals.Ins.Refs.Get<CurrentQuest>().Type == typeof(Quest_ExplorePlanet)) {
                    items.Add(UIItem.CreateButton("完成任务", MainQuest.Ins.CompleteQuest<Quest_ExplorePlanet>, Quest_ExplorePlanet_CanBeCompleted));
                }
                items.Add(UIItem.CreateMultilineText("（如何调查环境？）走近想调查的地块，然后点击地块"));
                items.Add(UIItem.CreateMultilineText("（如果降落的地方没有高山或森林怎么办？）走进飞船，重新起飞"));
            });
        }
    }
}

