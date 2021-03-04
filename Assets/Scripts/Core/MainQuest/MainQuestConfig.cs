
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class Quest_CongratulationsQuestAllCompleted { }
    [Concept]
    public class Quest_LandRocket { }

    //[Concept]
    //public class Quest_ExplorePlanet { }
    //public class SubQuest_ExplorePlanet_Sea { }
    //public class SubQuest_ExplorePlanet_Forest { }
    //public class SubQuest_ExplorePlanet_Mountain { }
    //public class SubQuest_ExplorePlanet_Plain { }

    //[Concept]
    //public class Quest_ResearchOnLocalCreature { }
    //public class SubQuest_ResearchOnMeat { }
    //public class SubQuest_ResearchOnBerry { }
    //public class SubQuest_ResearchOnFish { }
    //public class SubQuest_ResearchOnStone { }
    //public class SubQuest_ResearchOnOre { }

    [Concept]
    public class Quest_CollectFood_Hunting { }
    [Concept]
    public class Quest_HavePopulation_Settlement { }
    [Concept]
    public class Quest_CollectFood_Algriculture { }
    [Concept]
    public class Quest_HavePopulation_PopulationGrowth { }
    [Concept]
    public class Quest_CollectWood_Woodcutting { }


    public class MainQuestConfig : MonoBehaviour
    {
        public static MainQuestConfig Ins { get; private set; }

        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;

            int i = 0;
            foreach (var quest in QuestSequence) {
                indexDict.Add(quest, i);
                i++;
            }

            OnTapQuest = new Dictionary<Type, Action<List<IUIItem>>>();
            OnStartQuest = new Dictionary<Type, Action>();
            CreateOnTapQuest();
        }
        public Dictionary<Type, Action<List<IUIItem>>> OnTapQuest { get; private set; }
        public Dictionary<Type, Action> OnStartQuest { get; private set; }

        public List<Type> QuestSequence { get; } = new List<Type> {
            typeof(Quest_LandRocket),
            typeof(Quest_CollectFood_Hunting),
            // typeof(Quest_ExplorePlanet),
            // typeof(Quest_ResearchOnLocalCreature),
            typeof(Quest_CongratulationsQuestAllCompleted),
        };
        private Dictionary<Type, int> indexDict = new Dictionary<Type, int>();
        public int GetIndex(Type quest) {
            return indexDict[quest];
        }

        private const string completed = "【 √ 】";
        private const string notCompleted = "【 × 】";
        private string CompletionLabel<T>() {
            return Globals.Ins.Bool<T>() ? completed : notCompleted;
        }
        private void CreateOnTapQuest() {
            OnTapQuest.Add(typeof(Quest_CongratulationsQuestAllCompleted), items => {
                items.Add(UIItem.CreateMultilineText("已经完成了全部任务！此任务无法完成，并且没有更多任务了"));
            });
            // 登陆星球
            OnTapQuest.Add(typeof(Quest_LandRocket), items => {
                items.Add(UIItem.CreateMultilineText("飞船正在环绕星球飞行，可以找一块平原降落。"));
                items.Add(UIItem.CreateMultilineText("（如何降落？）点击想要降落的平原"));
            });
            //// 调查星球
            //Func<bool> Quest_ExplorePlanet_CanBeCompleted = () => {
            //    return Globals.Ins.Bool<SubQuest_ExplorePlanet_Forest>()
            //    && Globals.Ins.Bool<SubQuest_ExplorePlanet_Mountain>()
            //    && Globals.Ins.Bool<SubQuest_ExplorePlanet_Plain>()
            //    && Globals.Ins.Bool<SubQuest_ExplorePlanet_Sea>();
            //};
            //// CheckQuestCanBeCompleted.Add(typeof(Quest_ExplorePlanet), Quest_ExplorePlanet_CanBeCompleted);
            //OnTapQuest.Add(typeof(Quest_ExplorePlanet), items => {
            //    items.Add(UIItem.CreateMultilineText("飞船已经降落，需要调查当地环境"));
            //    items.Add(UIItem.CreateText($"{(CompletionLabel<SubQuest_ExplorePlanet_Plain>())} 调查平原 "));
            //    items.Add(UIItem.CreateText($"{(CompletionLabel<SubQuest_ExplorePlanet_Forest>())} 调查森林 "));
            //    items.Add(UIItem.CreateText($"{(CompletionLabel<SubQuest_ExplorePlanet_Mountain>())} 调查高山 "));
            //    items.Add(UIItem.CreateText($"{(CompletionLabel<SubQuest_ExplorePlanet_Sea>())} 调查海洋"));
            //    if (Globals.Ins.Refs.Get<CurrentQuest>().Type == typeof(Quest_ExplorePlanet)) {
            //        items.Add(UIItem.CreateButton("完成任务", MainQuest.Ins.CompleteQuest<Quest_ExplorePlanet>, Quest_ExplorePlanet_CanBeCompleted));
            //    }
            //    items.Add(UIItem.CreateMultilineText("（如何调查环境？）走近想调查的地块，然后点击地块"));
            //    items.Add(UIItem.CreateMultilineText("（如果降落的地方没有高山或森林怎么办？）走进飞船，重新起飞"));
            //});
            OnStartQuest.Add(typeof(Quest_CollectFood_Hunting), () => {
                IValue questProgressValue = Globals.Ins.Values.GetOrCreate<QuestProgress>();
                questProgressValue.Max = 1000;
                questProgressValue.Del = Value.Second;
                questProgressValue.Inc = 0;
                Globals.Ins.Refs.GetOrCreate<QuestProgress>().Type = typeof(Food);
            }); 
            // 研究当地生物
            OnTapQuest.Add(typeof(Quest_CollectFood_Hunting), items => {
                items.Add(UIItem.CreateMultilineText("采集食物，检验"));
                items.Add(UIItem.CreateMultilineText("（如何采集食物？）点击森林/水域，建立猎场/渔场，建立道路连接猎场/渔场和飞船。点击道路，点击沿路运输资源"));
                items.Add(UIItem.CreateText($"{(CompletionLabel<Quest_CollectFood_Hunting>())} 任务目标：获取1000任意类型食材"));
            });
        }
    }
}

