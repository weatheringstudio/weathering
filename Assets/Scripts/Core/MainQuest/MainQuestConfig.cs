
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
            typeof(Quest_HavePopulation_Settlement),
            typeof(Quest_CollectFood_Algriculture),
            typeof(Quest_HavePopulation_PopulationGrowth),
            // typeof(Quest_ExplorePlanet),
            // typeof(Quest_ResearchOnLocalCreature),
            typeof(Quest_CongratulationsQuestAllCompleted),
        };
        private Dictionary<Type, int> indexDict = new Dictionary<Type, int>();
        public int GetIndex(Type quest) {
            return indexDict[quest];
        }

        //private const string completed = "【 √ 】";
        //private const string notCompleted = "【 × 】";
        //private string CompletionLabel<T>() {
        //    return Globals.Ins.Bool<T>() ? completed : notCompleted;
        //}
        private void CreateOnTapQuest() {
            OnTapQuest.Add(typeof(Quest_CongratulationsQuestAllCompleted), items => {
                items.Add(UIItem.CreateMultilineText("已经完成了全部任务！此任务无法完成，并且没有更多任务了"));
            });
            // 登陆星球
            OnTapQuest.Add(typeof(Quest_LandRocket), items => {
                items.Add(UIItem.CreateMultilineText("飞船正在环绕星球飞行，可以找一块平原降落。"));
                items.Add(UIItem.CreateMultilineText("（如何降落？）点击想要降落的平原"));
            });



            // 捕鱼，捕猎
            const long difficulty_Quest_CollectFood_Hunting = 100;
            OnStartQuest.Add(typeof(Quest_CollectFood_Hunting), () => {
                Globals.Ins.Values.GetOrCreate<QuestResource>().Max = difficulty_Quest_CollectFood_Hunting;
                Globals.Ins.Refs.GetOrCreate<QuestResource>().Type = typeof(Food);
            });
            OnTapQuest.Add(typeof(Quest_CollectFood_Hunting), items => {
                items.Add(UIItem.CreateMultilineText("（如何捕猎？）点击森林，建造猎场，建造仓库，建造道路，建立资源连接，点击仓库收取资源"));
                items.Add(UIItem.CreateText($"目标：获取{Localization.Ins.Val(typeof(Food), difficulty_Quest_CollectFood_Hunting)}"));
            });

            // 获取居民
            const long difficulty_Quest_HavePopulation_Settlement = 2;
            OnStartQuest.Add(typeof(Quest_HavePopulation_Settlement), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_HavePopulation_Settlement;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Worker);
            });
            OnTapQuest.Add(typeof(Quest_HavePopulation_Settlement), items => {
                items.Add(UIItem.CreateMultilineText("（如何生产居民？）建造村庄，建造道路连接猎场与村庄，点击村庄获得居民"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(Worker), difficulty_Quest_HavePopulation_Settlement)}"));
            });

            // 原始农业
            const long difficulty_Quest_CollectFood_Algriculture = 10000;
            OnStartQuest.Add(typeof(Quest_CollectFood_Algriculture), () => {
                Globals.Ins.Values.GetOrCreate<QuestResource>().Max = difficulty_Quest_CollectFood_Algriculture;
                Globals.Ins.Refs.GetOrCreate<QuestResource>().Type = typeof(Food);
            });
            OnTapQuest.Add(typeof(Quest_CollectFood_Algriculture), items => {
                items.Add(UIItem.CreateMultilineText("（如何种田？）建造农场，派遣居民。"));
                items.Add(UIItem.CreateText($"目标：获取{Localization.Ins.Val(typeof(Food), difficulty_Quest_CollectFood_Algriculture)}"));
            });

            // 人口增长
            const long difficulty_Quest_HavePopulation_PopulationGrowth = 10;
            OnStartQuest.Add(typeof(Quest_HavePopulation_PopulationGrowth), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_HavePopulation_PopulationGrowth;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Worker);
            });
            OnTapQuest.Add(typeof(Quest_HavePopulation_PopulationGrowth), items => {
                items.Add(UIItem.CreateMultilineText("（如何生产更多居民？）建造农田派遣居民生产食物，建造村庄消耗食物生产居民"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(Worker), difficulty_Quest_HavePopulation_PopulationGrowth)}"));
            });
        }
    }
}

