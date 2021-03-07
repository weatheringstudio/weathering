
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
    public class Quest_CollectFood_Hunting { } // 解锁：猎场，道路，仓库
    [Concept]
    public class Quest_HavePopulation_Settlement { } // 解锁：村庄
    [Concept]
    public class Quest_CollectFood_Algriculture { } // 解锁：农田
    [Concept]
    public class Quest_HavePopulation_PopulationGrowth { } // 解锁：森林里可以建造道路
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
            typeof(Quest_CollectWood_Woodcutting),
            typeof(Quest_CongratulationsQuestAllCompleted),
        };
        private Dictionary<Type, int> indexDict = new Dictionary<Type, int>();
        public int GetIndex(Type quest) {
            return indexDict[quest];
        }

        private string FAQ(string question) {
            return $"<color=#ff9999>({question})</color>";
        }

        public readonly static Type StartingQuest = typeof(Quest_CollectFood_Algriculture);
        private void CreateOnTapQuest() {
            OnTapQuest.Add(typeof(Quest_CongratulationsQuestAllCompleted), items => {
                items.Add(UIItem.CreateMultilineText("已经完成了全部任务！此任务无法完成，并且没有更多任务了"));
            });

            // 登陆星球
            OnTapQuest.Add(typeof(Quest_LandRocket), items => {
                items.Add(UIItem.CreateMultilineText("飞船正在环绕星球飞行，可以找一块平原降落。"));
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何降落?")} 点击想要降落的平原"));
            });


            // 捕鱼，捕猎
            const long difficulty_Quest_CollectFood_Hunting = 100;
            OnStartQuest.Add(typeof(Quest_CollectFood_Hunting), () => {
                Globals.Ins.Values.GetOrCreate<QuestResource>().Max = difficulty_Quest_CollectFood_Hunting;
                Globals.Ins.Refs.GetOrCreate<QuestResource>().Type = typeof(DeerMeat);
            });
            OnTapQuest.Add(typeof(Quest_CollectFood_Hunting), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<HuntingGround>()}{Localization.Ins.Get<WareHouse>()}"));
                items.Add(UIItem.CreateMultilineText($"目标：获取{Localization.Ins.Val(typeof(DeerMeat), difficulty_Quest_CollectFood_Hunting)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何捕猎?")} 点击森林、建造猎场；点击平原、建造仓库；建立资源连接；点击仓库、收取资源"));
            });

            // 获取居民
            const long difficulty_Quest_HavePopulation_Settlement = 2;
            OnStartQuest.Add(typeof(Quest_HavePopulation_Settlement), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_HavePopulation_Settlement;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Worker);
            });
            OnTapQuest.Add(typeof(Quest_HavePopulation_Settlement), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<Road>()}{Localization.Ins.Get<Village>()}"));
                items.Add(UIItem.CreateMultilineText($"目标：拥有{Localization.Ins.Val(typeof(Worker), difficulty_Quest_HavePopulation_Settlement)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何生产居民?")} 建造村庄，建造道路连接猎场与村庄，点击村庄获得居民"));
            });

            // 原始农业
            const long difficulty_Quest_CollectFood_Algriculture = 2000;
            OnStartQuest.Add(typeof(Quest_CollectFood_Algriculture), () => {
                Globals.Ins.Values.GetOrCreate<QuestResource>().Max = difficulty_Quest_CollectFood_Algriculture;
                Globals.Ins.Refs.GetOrCreate<QuestResource>().Type = typeof(Grain);
            });
            OnTapQuest.Add(typeof(Quest_CollectFood_Algriculture), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<Farm>()}"));
                items.Add(UIItem.CreateText($"目标：获取{Localization.Ins.Val(typeof(Grain), difficulty_Quest_CollectFood_Algriculture)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何种田?")} 建造农场，派遣居民。"));
            });

            // 人口增长
            const long difficulty_Quest_HavePopulation_PopulationGrowth = 10;
            OnStartQuest.Add(typeof(Quest_HavePopulation_PopulationGrowth), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_HavePopulation_PopulationGrowth;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Worker);
            });
            OnTapQuest.Add(typeof(Quest_HavePopulation_PopulationGrowth), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<Road>()}可以建造在森林"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(Worker), difficulty_Quest_HavePopulation_PopulationGrowth)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何生产更多居民?")} 建造农场，派遣居民生产食物，建造村庄消耗食物生产居民"));
            });

            // 初次伐木
            const long difficulty_Quest_CollectWood_Woodcutting = 100;
            OnStartQuest.Add(typeof(Quest_CollectWood_Woodcutting), () => {
                Globals.Ins.Values.GetOrCreate<QuestResource>().Max = difficulty_Quest_CollectWood_Woodcutting;
                Globals.Ins.Refs.GetOrCreate<QuestResource>().Type = typeof(Wood);
            });
            OnTapQuest.Add(typeof(Quest_CollectWood_Woodcutting), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<ForestLoggingCamp>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(Wood), difficulty_Quest_CollectWood_Woodcutting)}"));
            });

            // 木材加工
            //OnStartQuest.Add(typeof(Quest_CollectWood_Woodcutting), () => {
            //    Globals.Ins.Values.GetOrCreate<QuestResource>().Max = difficulty_Quest_CollectWood_Woodcutting;
            //    Globals.Ins.Refs.GetOrCreate<QuestResource>().Type = typeof(Wood);
            //});
            //OnTapQuest.Add(typeof(Quest_CollectWood_Woodcutting), items => {
            //    items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<ForestLoggingCamp>()}"));
            //    items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(Wood), difficulty_Quest_CollectWood_Woodcutting)}"));
            //});
        }

        public static void QuestConfigNotProvidedThrowException(Type type) {
            // 在上面配置任务
            throw new Exception($"没有配置任务内容{type}");
        }
    }
}

