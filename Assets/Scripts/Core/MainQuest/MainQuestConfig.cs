
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
    public class Quest_CollectFood_Initial { } // 解锁：探索功能
    [Concept]
    public class Quest_ConstructBerryBushAndWareHouse_Initial { } // 解锁：浆果丛，茅草仓库
    [Concept]
    public class Quest_CollectFood_Hunting { } // 解锁：道路，猎场，渔场
    [Concept]
    public class Quest_HavePopulation_Settlement { } // 解锁：草屋
    [Concept]
    public class Quest_CollectFood_Algriculture { } // 解锁：农田
    [Concept]
    public class Quest_HavePopulation_PopulationGrowth { } // 
    [Concept]
    public class Quest_CollectWood_Woodcutting { } // 解锁：伐木场
    [Concept]
    public class Quest_ProduceWoodProduct_WoodProcessing { } // 解锁：锯木厂
    [Concept]
    public class Quest_CollectStone_Stonecutting { } // 解锁：采石场
    [Concept]
    public class Quest_ProduceStoneProduct_StoneProcessing { } // 解锁：石材加工厂
    [Concept]
    public class Quest_ConstructBridge { } // 解锁：桥
    [Concept]
    public class Quest_ProduceToolPrimitive { } // 解锁：工具厂
    [Concept]
    public class Quest_HavePopulation_ResidenceConstruction { } // 解锁：砖厂
    [Concept]
    public class Quest_ProduceWheelPrimitive { } // 解锁：车轮厂
    [Concept]
    public class Quest_GainGoldCoinThroughMarket { } // 解锁：市场
    [Concept]
    public class Quest_CollectMetalOre_Mining { } // 解锁：采矿厂
    [Concept]
    public class Quest_ProduceMetal_Smelting { } // 解锁：冶炼厂，运输站，运输站终点
    [Concept]
    public class Quest_ProduceMetalProduct_Casting { } // 解锁：铸造场
    [Concept]
    public class Quest_ProduceMachinePrimitive { } // 解锁：简易机器厂

    [Concept]
    public class Quest_CollectCoal { } // 解锁：煤矿
    [Concept]
    public class Quest_ProduceSteel { } // 解锁：钢厂
    [Concept]
    public class Quest_ProduceConcrete { } // 解锁：水泥厂
    [Concept]
    public class Quest_ProduceBuildingPrefabrication { } // 解锁：建筑结构厂

    [Concept]
    public class Quest_ProduceElectricity { } // 解锁：水泵，管道，发电厂
    [Concept]
    public class Quest_ProduceAluminum { } // 解锁：铝土矿，炼铝厂
    [Concept]
    public class Quest_ProduceLPG { } // 解锁：炼油厂，裂解厂
    [Concept]
    public class Quest_ProducePlastic { } // 解锁：塑料厂
    [Concept]
    public class Quest_ProduceLightMaterial { } // 解锁：铝土矿，炼铝厂


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
            CanCompleteQuest = new Dictionary<Type, Func<bool>>();
            CreateOnTapQuest();
        }
        public Dictionary<Type, Action<List<IUIItem>>> OnTapQuest { get; private set; }
        public Dictionary<Type, Action> OnStartQuest { get; private set; }
        public Dictionary<Type, Func<bool>> CanCompleteQuest { get; private set; }

        public List<Type> QuestSequence { get; } = new List<Type> {
            typeof(Quest_LandRocket),
            typeof(Quest_CollectFood_Initial),
            typeof(Quest_ConstructBerryBushAndWareHouse_Initial),

            typeof(Quest_CollectFood_Hunting),
            typeof(Quest_HavePopulation_Settlement),
            typeof(Quest_CollectFood_Algriculture),
            typeof(Quest_HavePopulation_PopulationGrowth),

            typeof(Quest_CollectWood_Woodcutting),
            typeof(Quest_ProduceWoodProduct_WoodProcessing),
            typeof(Quest_CollectStone_Stonecutting),
            typeof(Quest_ProduceStoneProduct_StoneProcessing),
            typeof(Quest_ConstructBridge),
            typeof(Quest_ProduceToolPrimitive),
            typeof(Quest_HavePopulation_ResidenceConstruction),
            typeof(Quest_ProduceWheelPrimitive),

            typeof(Quest_GainGoldCoinThroughMarket),

            typeof(Quest_CollectMetalOre_Mining),
            typeof(Quest_ProduceMetal_Smelting),
            typeof(Quest_ProduceMetalProduct_Casting),
            typeof(Quest_ProduceMachinePrimitive),

            typeof(Quest_CollectCoal),
            typeof(Quest_ProduceSteel),
            typeof(Quest_ProduceConcrete),
            typeof(Quest_ProduceBuildingPrefabrication),

            typeof(Quest_ProduceElectricity),
            typeof(Quest_ProduceAluminum),
            typeof(Quest_ProduceLPG),
            typeof(Quest_ProducePlastic),
            typeof(Quest_ProduceLightMaterial),

            typeof(Quest_CongratulationsQuestAllCompleted),
        };
        private readonly Dictionary<Type, int> indexDict = new Dictionary<Type, int>();
        public int GetIndex(Type quest) {
            if (!indexDict.TryGetValue(quest, out int id)) {
                throw new Exception($"找不到任务{quest.Name}对应的id");
            }
            return id;
        }

        private string FAQ(string question) {
            return $"<color=#ff9999>({question})</color>";
        }

        // public readonly static Type StartingQuest = typeof(Quest_CollectMetalOre_Mining);
        public readonly static Type StartingQuest = GameConfig.CheatMode ? typeof(Quest_CongratulationsQuestAllCompleted) : typeof(Quest_LandRocket);

        private void CreateOnTapQuest() {
            OnTapQuest.Add(typeof(Quest_CongratulationsQuestAllCompleted), items => {
                items.Add(UIItem.CreateMultilineText("已经完成了全部任务！此任务无法完成，并且没有更多任务了"));
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<OilDriller>()}{Localization.Ins.Get<RoadForFluid>()}"));
                items.Add(UIItem.CreateMultilineText($"刚解锁的东西并没有什么用"));
            });

            // 登陆星球
            OnTapQuest.Add(typeof(Quest_LandRocket), items => {
                items.Add(UIItem.CreateMultilineText("飞船正在环绕星球飞行，是时候找一块平原降落了"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何降落?")} 点击平原，点击降落"));
            });

            // 采集浆果
            const long difficulty_Quest_CollectFood_Initial = 10;
            CanCompleteQuest.Add(typeof(Quest_CollectFood_Initial), () => MapView.Ins.TheOnlyActiveMap.Inventory.CanRemove<Berry>() >= difficulty_Quest_CollectFood_Initial);
            OnTapQuest.Add(typeof(Quest_CollectFood_Initial), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 探索{Localization.Ins.Get<TerrainType_Forest>()}"));
                items.Add(UIItem.CreateMultilineText($"目标: 获得{Localization.Ins.Val<Berry>(difficulty_Quest_CollectFood_Initial)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何获得{Localization.Ins.ValUnit<Berry>()}")} 点击{Localization.Ins.Get<TerrainType_Forest>()}，点击探索"));
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何获得查看浆果数量?")} 点击地图右上角“文件夹”按钮"));
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何获得查看当前体力?")} 点击小人"));
            });

            // 建造浆果丛和仓库
            CanCompleteQuest.Add(typeof(Quest_ConstructBerryBushAndWareHouse_Initial), () => {
                IMap map = MapView.Ins.TheOnlyActiveMap;
                IRefs refs = map.Refs;
                return refs.GetOrCreate<WareHouseOfGrass>().Value >= 1 && refs.GetOrCreate<BerryBush>().Value >= 1;
            });
            OnTapQuest.Add(typeof(Quest_ConstructBerryBushAndWareHouse_Initial), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<BerryBush>()}{Localization.Ins.Get<WareHouseOfGrass>()}"));
                items.Add(UIItem.CreateMultilineText($"目标: 建造{Localization.Ins.Get<BerryBush>()}和{Localization.Ins.Get<WareHouseOfGrass>()}"));
                items.Add(UIItem.CreateText($"当前人口数: {Localization.Ins.Val(typeof(Worker), MapView.Ins.TheOnlyActiveMap.Values.GetOrCreate<Worker>().Max)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何建造{Localization.Ins.Get<WareHouseOfGrass>()}?")} 点击{Localization.Ins.Get<TerrainType_Plain>()}，点击物流"));
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何建造{Localization.Ins.Get<BerryBush>()}?")} 点击{Localization.Ins.Get<TerrainType_Plain>()}，点击农业"));
                items.Add(UIItem.CreateMultilineText($"{FAQ($"快速收集仓库资源的方法?")} 走过仓库，自动收集。收集成功时有音效"));
            });

            // 食物
            const long difficulty_Quest_CollectFood_Hunting = 200;
            OnStartQuest.Add(typeof(Quest_CollectFood_Hunting), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_CollectFood_Hunting;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Food);
            });
            OnTapQuest.Add(typeof(Quest_CollectFood_Hunting), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WareHouseOfGrass>()}{Localization.Ins.Get<RoadForTransportable>()}{Localization.Ins.Get<BerryBush>()}{Localization.Ins.Get<HuntingGround>()}{Localization.Ins.Get<SeaFishery>()}"));
                items.Add(UIItem.CreateMultilineText($"目标: 拥有{Localization.Ins.Val(typeof(Food), difficulty_Quest_CollectFood_Hunting)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何自动获取大量食材?")} 建造{Localization.Ins.Get<BerryBush>()}或{Localization.Ins.Get<HuntingGround>()}或{Localization.Ins.Get<SeaFishery>()}；点击平原、建造{Localization.Ins.Get<WareHouseOfGrass>()}；使用磁铁工具，建立资源连接；走过或点击{Localization.Ins.Get<WareHouseOfGrass>()}、收取资源"));
            });

            // 获取居民
            const long difficulty_Quest_HavePopulation_Settlement = 5;
            CanCompleteQuest.Add(typeof(Quest_HavePopulation_Settlement), () => MapView.Ins.TheOnlyActiveMap.Values.GetOrCreate<Worker>().Max >= difficulty_Quest_HavePopulation_Settlement);
            // 注释掉的是拥有空闲工人人物的配置
            //OnStartQuest.Add(typeof(Quest_HavePopulation_Settlement), () => {
            //    Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_HavePopulation_Settlement;
            //    Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Worker);
            //});
            OnTapQuest.Add(typeof(Quest_HavePopulation_Settlement), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<ResidenceOfGrass>()}{Localization.Ins.Get<CellarForPersonalStorage>()}"));
                items.Add(UIItem.CreateMultilineText($"目标: 总人口数达到{Localization.Ins.Val(typeof(Worker), difficulty_Quest_HavePopulation_Settlement)}"));
                items.Add(UIItem.CreateText($"当前人口数: {Localization.Ins.Val(typeof(Worker), MapView.Ins.TheOnlyActiveMap.Values.GetOrCreate<Worker>().Max)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何生产居民?")} 建造村庄，建造道路连接猎场与村庄，点击村庄获得居民"));
            });

            // 原始农业
            const long difficulty_Quest_CollectFood_Algriculture = 10000;
            OnStartQuest.Add(typeof(Quest_CollectFood_Algriculture), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_CollectFood_Algriculture;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Grain);
            });
            OnTapQuest.Add(typeof(Quest_CollectFood_Algriculture), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<Farm>()}{Localization.Ins.Get<Pasture>()}{Localization.Ins.Get<Hennery>()}"));
                items.Add(UIItem.CreateText($"目标: 拥有{Localization.Ins.Val(typeof(Grain), difficulty_Quest_CollectFood_Algriculture)}"));
                items.Add(UIItem.CreateText($"当前产量: {Localization.Ins.Val(typeof(GrainSupply), MapView.Ins.TheOnlyActiveMap.Values.GetOrCreate<GrainSupply>().Max)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何种田?")} 建造{Localization.Ins.Get<Farm>()}，派遣居民。"));
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何获得{Localization.Ins.ValUnit<WoodPlank>()}?")} 暂时无法获得，完成后续任务解锁"));
            });

            // 人口增长
            const long difficulty_Quest_HavePopulation_PopulationGrowth = 30;
            CanCompleteQuest.Add(typeof(Quest_HavePopulation_PopulationGrowth), () => MapView.Ins.TheOnlyActiveMap.Values.GetOrCreate<Worker>().Max >= difficulty_Quest_HavePopulation_PopulationGrowth);
            //OnStartQuest.Add(typeof(Quest_HavePopulation_PopulationGrowth), () => {
            //    Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_HavePopulation_PopulationGrowth;
            //    Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Worker);
            //});
            OnTapQuest.Add(typeof(Quest_HavePopulation_PopulationGrowth), items => {
                items.Add(UIItem.CreateText($"目标: 总人口数达到{Localization.Ins.Val(typeof(Worker), difficulty_Quest_HavePopulation_PopulationGrowth)}"));
                items.Add(UIItem.CreateText($"当前人口数: {Localization.Ins.Val(typeof(Worker), MapView.Ins.TheOnlyActiveMap.Values.GetOrCreate<Worker>().Max)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何生产更多居民?")} 建造{Localization.Ins.Get<Farm>()}和{Localization.Ins.Get<ResidenceOfGrass>()}。可以查看建筑功能页面，注意建筑数量搭配"));
            });

            // 初次伐木
            const long difficulty_Quest_CollectWood_Woodcutting = 100;
            OnStartQuest.Add(typeof(Quest_CollectWood_Woodcutting), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_CollectWood_Woodcutting;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Wood);
            });
            OnTapQuest.Add(typeof(Quest_CollectWood_Woodcutting), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<ForestLoggingCamp>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(Wood), difficulty_Quest_CollectWood_Woodcutting)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何建造{Localization.Ins.Get<ForestLoggingCamp>()}?")} 点击{Localization.Ins.Get<TerrainType_Forest>()}，点击林业"));
            });

            // 木材加工
            const long difficulty_Quest_ProduceWoodProduct_WoodProcessing = 100;
            OnStartQuest.Add(typeof(Quest_ProduceWoodProduct_WoodProcessing), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_ProduceWoodProduct_WoodProcessing;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(WoodPlank);
            });
            OnTapQuest.Add(typeof(Quest_ProduceWoodProduct_WoodProcessing), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WorkshopOfWoodcutting>()}{Localization.Ins.Get<ResidenceOfWood>()}{Localization.Ins.Get<WareHouseOfWood>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(WoodPlank), difficulty_Quest_ProduceWoodProduct_WoodProcessing)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何建造{Localization.Ins.Get<WorkshopOfWoodcutting>()}?")} 点击{Localization.Ins.Get<TerrainType_Plain>()}，点击工业"));
            });

            // 初次采石
            const long difficulty_Quest_CollectStone_Stonecutting = 100;
            OnStartQuest.Add(typeof(Quest_CollectStone_Stonecutting), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_CollectStone_Stonecutting;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Stone);
            });
            OnTapQuest.Add(typeof(Quest_CollectStone_Stonecutting), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<MountainQuarry>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(Stone), difficulty_Quest_CollectStone_Stonecutting)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何建造{Localization.Ins.Get<MountainQuarry>()}?")} 点击{Localization.Ins.Get<TerrainType_Mountain>()}，点击矿业"));
            });

            // 石材加工
            const long difficulty_Quest_ProduceStoneProduct_StoneProcessing = 100;
            OnStartQuest.Add(typeof(Quest_ProduceStoneProduct_StoneProcessing), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_ProduceStoneProduct_StoneProcessing;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(StoneBrick);
            });
            OnTapQuest.Add(typeof(Quest_ProduceStoneProduct_StoneProcessing), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WorkshopOfStonecutting>()}{Localization.Ins.Get<ResidenceOfStone>()}{Localization.Ins.Get<WareHouseOfStone>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(StoneBrick), difficulty_Quest_ProduceStoneProduct_StoneProcessing)}"));
            });

            // 造桥
            const long difficulty_Quest_ConstructBridge = 1;
            CanCompleteQuest.Add(typeof(Quest_ConstructBridge), () => MapView.Ins.TheOnlyActiveMap.Refs.GetOrCreate<RoadAsBridge>().Value >= difficulty_Quest_ConstructBridge);
            OnTapQuest.Add(typeof(Quest_ConstructBridge), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<RoadAsBridge>()}"));
                items.Add(UIItem.CreateText($"目标: 建造{Localization.Ins.Get<RoadAsBridge>()}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何建造{Localization.Ins.Get<RoadAsBridge>()}?")} 点击{Localization.Ins.Get<TerrainType_Sea>()}"));
                items.Add(UIItem.CreateMultilineText($"{FAQ($"{Localization.Ins.Get<RoadAsBridge>()}有什么用?")} 跨越岛屿。桥也能像道路一样运输资源"));
            });

            // 制造工具
            const long difficulty_Quest_ProduceToolPrimitive = 100;
            OnStartQuest.Add(typeof(Quest_ProduceToolPrimitive), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_ProduceToolPrimitive;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(ToolPrimitive);
            });
            OnTapQuest.Add(typeof(Quest_ProduceToolPrimitive), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WorkshopOfToolPrimitive>()}{Localization.Ins.Get<MineOfClay>()}{Localization.Ins.Get<ResidenceOfBrick>()}{Localization.Ins.Get<WareHouseOfBrick>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(ToolPrimitive), difficulty_Quest_ProduceToolPrimitive)}"));
            });

            // 住房建设
            const long difficulty_Quest_HavePopulation_ResidenceConstruction = 50;
            CanCompleteQuest.Add(typeof(Quest_HavePopulation_ResidenceConstruction), () => MapView.Ins.TheOnlyActiveMap.Values.GetOrCreate<Worker>().Max >= difficulty_Quest_HavePopulation_ResidenceConstruction);
            OnTapQuest.Add(typeof(Quest_HavePopulation_ResidenceConstruction), items => {
                items.Add(UIItem.CreateText($"目标: 总人口数达到{Localization.Ins.Val(typeof(Worker), difficulty_Quest_HavePopulation_ResidenceConstruction)}"));
                items.Add(UIItem.CreateText($"当前人口数: {Localization.Ins.Val(typeof(Worker), MapView.Ins.TheOnlyActiveMap.Values.GetOrCreate<Worker>().Max)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何生产更多居民? 建筑木屋、石屋、砖屋。草屋占地面积太大，而且越来越贵了")}"));
            });

            // 制造车轮
            const long difficulty_Quest_ProduceWheelPrimitive = 100;
            OnStartQuest.Add(typeof(Quest_ProduceWheelPrimitive), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_ProduceWheelPrimitive;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(WheelPrimitive);
            });
            OnTapQuest.Add(typeof(Quest_ProduceWheelPrimitive), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WorkshopOfWheelPrimitive>()}{Localization.Ins.Get<TransportStationSimpliest>()}{Localization.Ins.Get<TransportStationDestSimpliest>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(WheelPrimitive), difficulty_Quest_ProduceWheelPrimitive)}"));
            });

            // 市场
            OnStartQuest.Add(typeof(Quest_GainGoldCoinThroughMarket), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = 1;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(GoldCoin);
            });
            OnTapQuest.Add(typeof(Quest_GainGoldCoinThroughMarket), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<MarketForPlayer>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(GoldCoin), 1)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ($"如何建造{Localization.Ins.Get<WorkshopOfWoodcutting>()}?")} 点击{Localization.Ins.Get<TerrainType_Plain>()}，点击特殊建筑"));
            });

            // 初次采矿
            OnStartQuest.Add(typeof(Quest_CollectMetalOre_Mining), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = 100;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(MetalOre);
            });
            OnTapQuest.Add(typeof(Quest_CollectMetalOre_Mining), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<MineOfCopper>()}{Localization.Ins.Get<MineOfIron>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(MetalOre), 100)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("金属矿在哪里？")}铜矿、铁矿，都是金属矿。收集铜矿或铁矿都能完成任务"));
            });

            // 金属冶炼
            OnStartQuest.Add(typeof(Quest_ProduceMetal_Smelting), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = 100;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(MetalIngot);
            });
            OnTapQuest.Add(typeof(Quest_ProduceMetal_Smelting), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WorkshopOfCopperSmelting>()} {Localization.Ins.Get<WorkshopOfIronSmelting>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(MetalIngot), 100)}"));
            });

            // 金属铸造
            OnStartQuest.Add(typeof(Quest_ProduceMetalProduct_Casting), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = 100;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(MetalProduct);
            });
            OnTapQuest.Add(typeof(Quest_ProduceMetalProduct_Casting), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WorkshopOfCopperCasting>()}{Localization.Ins.Get<WorkshopOfIronCasting>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(MetalProduct), 100)}"));
            });

            // 制作机器
            OnStartQuest.Add(typeof(Quest_ProduceMachinePrimitive), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = 100;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(MachinePrimitive);
            });
            OnTapQuest.Add(typeof(Quest_ProduceMachinePrimitive), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WorkshopOfMachinePrimitive>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(MachinePrimitive), 100)}"));
            });

            // 煤的开采
            OnStartQuest.Add(typeof(Quest_CollectCoal), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = 100;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Coal);
            });
            OnTapQuest.Add(typeof(Quest_CollectCoal), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<MineOfCoal>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(Coal), 100)}"));
            });

            // 钢的冶炼
            OnStartQuest.Add(typeof(Quest_ProduceSteel), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = 100;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(SteelIngot);
            });
            OnTapQuest.Add(typeof(Quest_ProduceSteel), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WorkshopOfSteelWorking>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(SteelIngot), 100)}"));
            });

            // 水泥制造
            OnStartQuest.Add(typeof(Quest_ProduceConcrete), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = 100;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(ConcretePowder);
            });
            OnTapQuest.Add(typeof(Quest_ProduceConcrete), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<FactoryOfConcrete>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(ConcretePowder), 100)}"));
            });

            // 建筑合成
            OnStartQuest.Add(typeof(Quest_ProduceBuildingPrefabrication), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = 100;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(BuildingPrefabrication);
            });
            OnTapQuest.Add(typeof(Quest_ProduceBuildingPrefabrication), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<FactoryOfBuildingPrefabrication>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(BuildingPrefabrication), 100)}"));
            });



            //[Concept]
            //public class Quest_ProduceElectricity { } // 解锁：水泵，管道，发电厂
            //[Concept]
            //public class Quest_ProduceAluminum { } // 解锁：铝土矿，炼铝厂
            //[Concept]
            //public class Quest_ProduceNaturalGas { } // 解锁：炼油厂，裂解厂
            //[Concept]
            //public class Quest_ProducePlastic { } // 解锁：塑料厂
            //[Concept]
            //public class Quest_ProduceLightMaterial { } // 解锁：铝土矿，炼铝厂

            // 发电
            const long difficulty_Quest_ProduceElectricity = 100;
                OnStartQuest.Add(typeof(Quest_ProduceElectricity), () => {
                    Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_ProduceElectricity;
                    Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(ElectricitySupply);
                });
                OnTapQuest.Add(typeof(Quest_ProduceElectricity), items => {
                    items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<WaterPump>()} {Localization.Ins.Get<RoadForFluid>()}  {Localization.Ins.Get<PowerGeneratorOfWood>()} {Localization.Ins.Get<PowerGeneratorOfCoal>()}"));
                    items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(ElectricitySupply), difficulty_Quest_ProduceElectricity)}"));

                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateMultilineText($"{FAQ("如何运输海水？")} 海水必须通过管道运输。磁铁工具可以用于海水"));
                });

            //typeof(Quest_ProducePlastic),
            //typeof(Quest_ProduceLightMaterial),

            // 炼铝
            const long difficulty_Quest_ProduceAluminum = 100;
            OnStartQuest.Add(typeof(Quest_ProduceAluminum), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_ProduceAluminum;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(AluminiumIngot);
            });
            OnTapQuest.Add(typeof(Quest_ProduceAluminum), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<MineOfAluminum>()} {Localization.Ins.Get<FactoryOfAluminiumWorking>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(AluminiumIngot), difficulty_Quest_ProduceAluminum)}"));
            });

            // 液化石油气
            const long difficulty_Quest_ProduceLPG = 100;
            OnStartQuest.Add(typeof(Quest_ProduceLPG), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_ProduceLPG;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(LiquefiedPetroleumGas);
            });
            OnTapQuest.Add(typeof(Quest_ProduceLPG), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<OilDriller>()} {Localization.Ins.Get<FactoryOfPetroleumRefining>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(LiquefiedPetroleumGas), difficulty_Quest_ProduceLPG)}"));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateMultilineText($"{FAQ("如何运输原油及其产品？")} 液体必须通过管道运输。磁铁工具可以用于液体"));
            });

            // 塑料
            const long difficulty_Quest_ProducePlastic = 100;
            OnStartQuest.Add(typeof(Quest_ProducePlastic), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_ProducePlastic;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(Plastic);
            });
            OnTapQuest.Add(typeof(Quest_ProducePlastic), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<FactoryOfPlastic>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(Plastic), difficulty_Quest_ProducePlastic)}"));
            });

            // 轻质材料
            const long difficulty_Quest_ProduceLightMaterial = 100;
            OnStartQuest.Add(typeof(Quest_ProduceLightMaterial), () => {
                Globals.Ins.Values.GetOrCreate<QuestRequirement>().Max = difficulty_Quest_ProduceLightMaterial;
                Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = typeof(LightMaterial);
            });
            OnTapQuest.Add(typeof(Quest_ProduceLightMaterial), items => {
                items.Add(UIItem.CreateMultilineText($"已解锁 {Localization.Ins.Get<FactoryOfLightMaterial>()}"));
                items.Add(UIItem.CreateText($"目标：拥有{Localization.Ins.Val(typeof(LightMaterial), difficulty_Quest_ProduceLightMaterial)}"));
            });
        }

        public static void QuestConfigNotProvidedThrowException(Type type) {
            // 在上面配置任务
            throw new Exception($"没有配置任务内容{type}");
        }
    }
}

