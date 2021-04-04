
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface ITerrainType { }
    [Concept]
    public class TerrainType_None : ITerrainType { }
    [Concept]
    public class TerrainType_Any : ITerrainType { }
    [Concept]
    public class TerrainType_Plain : ITerrainType { }
    [Concept]
    public class TerrainType_Mountain : ITerrainType { }
    [Concept]
    public class TerrainType_Sea : ITerrainType { }
    [Concept]
    public class TerrainType_Forest : ITerrainType { }



    public class BindTerrainTypeAttribute : Attribute
    {
        public Type Data { get; private set; }
        public BindTerrainTypeAttribute(Type terrainType) {
            if (!(typeof(ITerrainType)).IsAssignableFrom(terrainType)) throw new Exception();
            Data = terrainType;
        }
    }

    public class CanBeBuildOnNotPassableTerrainAttribute : Attribute
    {

    }

    /// <summary>
    /// 迟早要换方式重构，不如先配置在这里吧
    /// </summary>
    public static class ConstructionConditionConfig
    {

        public static readonly Dictionary<Type, Func<Type, ITile, bool>> Conditions = new Dictionary<Type, Func<Type, ITile, bool>>() {

            { typeof(WareHouseOfGrass), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ConstructBerryBushAndWareHouse_Initial>() },
            { typeof(BerryBush), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ConstructBerryBushAndWareHouse_Initial>() },

            { typeof(HuntingGround), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>() },
            { typeof(SeaFishery), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>() && !TileUtility.FindOnNeightbors(tile, (ITile tile_, Type dir) => tile_ is SeaFishery)},

            { typeof(ResidenceOfGrass), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>() },
            { typeof(CellarForPersonalStorage), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>() },

            { typeof(Farm), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Algriculture>() },
            { typeof(Pasture), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Algriculture>() },
            { typeof(Hennery), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Algriculture>() },

            { typeof(ForestLoggingCamp), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectWood_Woodcutting>() },

            { typeof(WorkshopOfWoodcutting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>() },
            { typeof(ResidenceOfWood), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>() },
            { typeof(WareHouseOfWood), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>() },
            { typeof(ResidenceCoastal), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>() },
            { typeof(ResidenceOverTree), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>() },

            { typeof(MountainQuarry), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectStone_Stonecutting>() },
            { typeof(WorkshopOfStonecutting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceStoneProduct_StoneProcessing>() },
            { typeof(ResidenceOfStone), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceStoneProduct_StoneProcessing>() },
            { typeof(WareHouseOfStone), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceStoneProduct_StoneProcessing>() },
            { typeof(WallOfStoneBrick), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceStoneProduct_StoneProcessing>() },

            { typeof(RoadAsBridge), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceStoneProduct_StoneProcessing>() },

            { typeof(WorkshopOfToolPrimitive), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceToolPrimitive>() },
            { typeof(WorkshopOfBrickMaking), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceToolPrimitive>() },
            { typeof(MineOfClay), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceToolPrimitive>() },
            { typeof(ResidenceOfBrick), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceToolPrimitive>() },
            { typeof(WareHouseOfBrick), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceToolPrimitive>() },

            { typeof(WorkshopOfWheelPrimitive), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWheelPrimitive>() },
            { typeof(TransportStationSimpliest), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWheelPrimitive>() },
            { typeof(TransportStationDestSimpliest), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWheelPrimitive>() },

            { typeof(MarketForPlayer),(Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_GainGoldCoinThroughMarket>()  },
            { typeof(MineOfGold), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectMetalOre_Mining>() },

            { typeof(MineOfCopper), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectMetalOre_Mining>() },
            { typeof(MineOfIron), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectMetalOre_Mining>() },

            { typeof(WorkshopOfCopperSmelting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },
            { typeof(WorkshopOfIronSmelting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },

            { typeof(WorkshopOfCopperCasting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
            { typeof(WorkshopOfIronCasting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },

            { typeof(WorkshopOfMachinePrimitive), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMachinePrimitive>() },
            { typeof(TransportStationPort), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMachinePrimitive>() },
            { typeof(TransportStationDestPort), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMachinePrimitive>() },

            { typeof(MineOfCoal), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectCoal>() },

            { typeof(WorkshopOfSteelWorking), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceSteel>() },

            { typeof(FactoryOfConcrete), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceConcrete>() },
            { typeof(WareHouseOfConcrete), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceConcrete>() },
            { typeof(ResidenceOfConcrete), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceConcrete>() },
            { typeof(RoadOfConcrete), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceConcrete>() },

            { typeof(FactoryOfBuildingPrefabrication), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceBuildingPrefabrication>() },

            { typeof(PowerGeneratorOfWood), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceElectricity>() },
            { typeof(PowerGeneratorOfCoal), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceElectricity>() },
            { typeof(WaterPump) , (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceElectricity>() },
            { typeof(RoadForFluid), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceElectricity>() },

            { typeof(FactoryOfIronSmelting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceElectricity>() },
            { typeof(FactoryOfCopperSmelting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceElectricity>() },
            { typeof(FactoryOfSteelWorking), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceElectricity>() },

            { typeof(FactoryOfAluminiumWorking), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceAluminum>() },
            { typeof(MineOfAluminum), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceAluminum>() },

            { typeof(OilDriller), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceLPG>() },
            { typeof(FactoryOfPetroleumRefining), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceLPG>() },

            { typeof(FactoryOfPlastic), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProducePlastic>() },

            { typeof(FactoryOfLightMaterial), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceLightMaterial>() },

        };
    }

    public class TerrainDefault : StandardTile, IPassable, IDontSave, IIgnoreTool, ITileDescription
    {
        public bool DontSave => true;

        private List<IUIItem> ItemsBuffer;


        private void OnTapNearly(List<IUIItem> items) {

            ItemsBuffer = items;

            // 快捷方式建造
            if (UIItem.ShortcutMap == Map) { // 上次建造的建筑和自己处于同一个地图。standardtile
                if (TryConstructButton(UIItem.ShortcutType)) {
                    items.Add(UIItem.CreateSeparator());
                }
            }

            //if (GameConfig.CheatMode) {
            //    TryConstructButton<CheatHouse>();
            //}

            StandardMap map = Map as StandardMap;
            if (map == null) throw new Exception();

            // 探索功能
            if (TerrainType == typeof(TerrainType_Forest)) items.Add(UIItem.CreateButton("探索森林", ExplorationPage));
            // 其他建造方法
            bool isPlain = TerrainType == typeof(TerrainType_Plain);


            if (TerrainType != typeof(TerrainType_Forest) && MainQuest.Ins.IsOnOrBefore<Quest_CollectFood_Initial>()) {
                items.Add(UIItem.CreateMultilineText("点击地图右上角 “?” 查看当前任务。\n点击屏幕上方半透明黑色区域，关闭此界面。"));
            }

            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_ConstructBerryBushAndWareHouse_Initial>()) items.Add(UIItem.CreateButton("建造【物流】类", ConstructLogisticsPage));

            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>()) items.Add(UIItem.CreateButton("建造【特殊】类", ConstructSpecialsPage));

            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_ConstructBerryBushAndWareHouse_Initial>()) items.Add(UIItem.CreateButton("建造【农业】类", ConstructAgriculturePage));
            else if (TerrainType == typeof(TerrainType_Forest) && MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>()) items.Add(UIItem.CreateButton("建造【林业】类", ConstructForestryPage));
            else if (TerrainType == typeof(TerrainType_Mountain) && MainQuest.Ins.IsUnlocked<Quest_CollectStone_Stonecutting>()) items.Add(UIItem.CreateButton("建造【矿业】类", ConstructMiningPage));
            else if (TerrainType == typeof(TerrainType_Sea) && MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>()) {

                TryConstructButton<RoadAsBridge>();
                TryConstructButton<TransportStationPort>();
                TryConstructButton<TransportStationDestPort>();

                TryConstructButton<SeaFishery>();
                TryConstructButton<ResidenceCoastal>();
                TryConstructButton<WaterPump>();
            }

            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>()) items.Add(UIItem.CreateButton("建造【住房】类", ConstructResidencePage));
            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>()) items.Add(UIItem.CreateButton("建造【工业】类", ConstructIndustryPage));

            ItemsBuffer = null;
        }

        private static IValue sanity;
        private void ExplorationPage() {
            StandardMap map = Map as StandardMap;
            if (map == null) throw new Exception();
            if (sanity == null) sanity = Globals.Sanity;
            if (sanity == null) throw new Exception();

            IInventory inventory = Map.Inventory;
            if (inventory == null) throw new Exception();

            string title;
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateValueProgress<Sanity>(sanity));
            items.Add(UIItem.CreateTimeProgress<CoolDown>(Globals.CoolDown));

            if (!IsForestLike(map, Pos)) throw new Exception();

            title = $"探索森林中";
            items.Add(CreateGatheringButton("采集", typeof(Berry), 2, 3));
            items.Add(CreateGatheringButton("捕猎", typeof(DeerMeat), 2, 5));
            items.Add(CreateGatheringButton("伐木", typeof(Wood), 2, 3));

            UI.Ins.ShowItems(title, items);
        }
        private UIItem CreateGatheringButton(string text, Type type, long cost, long revenue) {
            return UIItem.CreateDynamicButton($"{text} {Localization.Ins.ValPlus(type, revenue)} {Localization.Ins.ValPlus<Sanity>(-cost)}", () => {
                if (Map.Inventory.CanAdd((type, revenue))) {
                    if (!Globals.SanityCheck(cost)) return;
                    Map.Inventory.Add((type, revenue));
                    Globals.SetCooldown = cost / 2;
                } else {
                    UIPreset.InventoryFull(() => UI.Ins.Active = false, Map.Inventory);
                }
            }, () => sanity.Val >= cost && Globals.IsCool);
        }
















        private void ConstructLogisticsPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;
            TryConstructButton<RoadForTransportable>();
            TryConstructButton<RoadOfConcrete>();
            TryConstructButton<RoadForFluid>();

            TryConstructButton<WareHouseOfGrass>();
            TryConstructButton<WareHouseOfWood>();
            TryConstructButton<WareHouseOfStone>();
            TryConstructButton<WareHouseOfBrick>();
            TryConstructButton<WareHouseOfConcrete>();

            TryConstructButton<TransportStationSimpliest>();
            TryConstructButton<TransportStationDestSimpliest>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("物流", items);
        }

        private void ConstructSpecialsPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            TryConstructButton<CellarForPersonalStorage>();
            TryConstructButton<MarketForPlayer>();

            TryConstructButton<WallOfStoneBrick>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("特殊", items);
        }

        private void ConstructResidencePage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;
            TryConstructButton<ResidenceOfGrass>();
            TryConstructButton<ResidenceOfWood>();
            TryConstructButton<ResidenceOfStone>();
            TryConstructButton<ResidenceOfBrick>();
            TryConstructButton<ResidenceOfConcrete>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("住房", items);
        }

        private void ConstructAgriculturePage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            // 平原
            TryConstructButton<BerryBush>();
            TryConstructButton<Farm>();
            TryConstructButton<Pasture>();
            TryConstructButton<Hennery>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("农业", items);
        }

        private void ConstructForestryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            // 森林
            TryConstructButton<HuntingGround>();
            TryConstructButton<ForestLoggingCamp>();
            TryConstructButton<ResidenceOverTree>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("林业", items);
        }
        //private void ConstructFisheryPage() {
        //    var items = UI.Ins.GetItems();
        //    items.Add(UIItem.CreateReturnButton(OnTap));

        //    ItemsBuffer = items;


        //    ItemsBuffer = null;

        //    UI.Ins.ShowItems("渔业", items);
        //}
        private void ConstructMiningPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            // 山地
            TryConstructButton<MineOfSand>();
            TryConstructButton<MineOfSalt>();
            TryConstructButton<MineOfClay>();
            TryConstructButton<MountainQuarry>();
            TryConstructButton<MineOfGold>();
            TryConstructButton<MineOfIron>();
            TryConstructButton<MineOfCopper>();
            TryConstructButton<MineOfCoal>();
            TryConstructButton<MineOfAluminum>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("矿业", items);
        }

        private void ConstructIndustryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            if (MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>()) items.Add(UIItem.CreateButton("【制造工业】", ConstructAssemblerPage));
            if (MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>()) items.Add(UIItem.CreateButton("【冶金工业】", ConstructSmelterPage));
            if (MainQuest.Ins.IsUnlocked<Quest_ProduceElectricity>()) items.Add(UIItem.CreateButton("【电力工业】", ConstructPowerGenerationPage));
            if (MainQuest.Ins.IsUnlocked<Quest_ProduceLPG>()) items.Add(UIItem.CreateButton("【石油工业】", ConstructPetroleumIndustryPage));

            ItemsBuffer = null;

            UI.Ins.ShowItems("工业", items);
        }
        private void ConstructAssemblerPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructIndustryPage));

            ItemsBuffer = items;

            TryConstructButton<WorkshopOfWoodcutting>();
            TryConstructButton<WorkshopOfStonecutting>();
            TryConstructButton<WorkshopOfToolPrimitive>();
            TryConstructButton<WorkshopOfWheelPrimitive>();
            TryConstructButton<WorkshopOfBrickMaking>();
            TryConstructButton<WorkshopOfMachinePrimitive>();
            TryConstructButton<FactoryOfConcrete>();
            TryConstructButton<FactoryOfBuildingPrefabrication>();
            TryConstructButton<FactoryOfLightMaterial>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("制造", items);
        }
        private void ConstructSmelterPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructIndustryPage));

            ItemsBuffer = items;

            TryConstructButton<WorkshopOfCopperSmelting>();
            TryConstructButton<WorkshopOfIronSmelting>();

            TryConstructButton<WorkshopOfCopperCasting>();
            TryConstructButton<WorkshopOfIronCasting>();

            TryConstructButton<WorkshopOfSteelWorking>();

            TryConstructButton<FactoryOfCopperSmelting>();
            TryConstructButton<FactoryOfIronSmelting>();

            TryConstructButton<FactoryOfSteelWorking>();
            TryConstructButton<FactoryOfAluminiumWorking>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("冶金", items);
        }
        private void ConstructPowerGenerationPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructIndustryPage));

            ItemsBuffer = items;

            TryConstructButton<PowerGeneratorOfWood>();
            TryConstructButton<PowerGeneratorOfCoal>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("电力", items);
        }
        private void ConstructPetroleumIndustryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructIndustryPage));

            ItemsBuffer = items;

            TryConstructButton<OilDriller>();
            TryConstructButton<FactoryOfPetroleumRefining>();
            TryConstructButton<FactoryOfHeavyOilCracking>();
            TryConstructButton<FactoryOfLightOilCracking>();
            TryConstructButton<FactoryOfPlastic>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("石油", items);
        }













        // --------------------------------------------------

        private bool TryConstructButton<T>() => TryConstructButton(typeof(T));
        private bool TryConstructButton(Type type) {
            if (CanConstruct(type)) {
                ItemsBuffer.Add(UIItem.CreateConstructionButton(type, this));
                return true;
            }
            return false;
        }
        public bool CanConstruct<T>() => CanConstruct(typeof(T));
        public bool CanConstruct(Type type) {
            // 土地类型测试
            StandardMap map = Map as StandardMap;
            var attr = Tag.GetAttribute<BindTerrainTypeAttribute>(type);
            if (attr != null) {
                if (TerrainType != attr.Data && attr.Data != typeof(TerrainType_Any)) {
                    return false;
                }
            } else {
                // 没指定的建筑，默认必须在平原上
                if (TerrainType != typeof(TerrainType_Plain)) return false;
            }
            // 自定义条件测试
            if (ConstructionConditionConfig.Conditions.TryGetValue(type, out var test)) {
                if (!test(type, this)) {
                    return false;
                }
            }
            // 通过测试
            return true;
        }


        // --------------------------------------------------












        public string TileDescription => Localization.Ins.Get(TerrainType);

        public Type TerrainType { get; private set; }
        public override void OnEnable() {
            StandardMap map = Map as StandardMap;
            if (map == null) throw new Exception();
            if (IsSeaLike(map, Pos)) {
                TerrainType = typeof(TerrainType_Sea);
            } else if (IsPlainLike(map, Pos)) {
                TerrainType = typeof(TerrainType_Plain);
            } else if (IsForestLike(map, Pos)) {
                TerrainType = typeof(TerrainType_Forest);
            } else if (IsMountainLike(map, Pos)) {
                TerrainType = typeof(TerrainType_Mountain);
            } else {
                throw new Exception();
            }
        }
        public static bool IsSeaLike(StandardMap map, Vector2Int pos) {
            pos = map.Validate(pos);
            return map.AltitudeTypes[pos.x, pos.y] == typeof(AltitudeSea);
        }
        public static bool IsMountainLike(StandardMap map, Vector2Int pos) {
            pos = map.Validate(pos);
            return map.AltitudeTypes[pos.x, pos.y] == typeof(AltitudeMountain);
        }
        public static bool IsForestLike(StandardMap map, Vector2Int pos) {
            pos = map.Validate(pos);
            return map.AltitudeTypes[pos.x, pos.y] == typeof(AltitudePlain) && map.MoistureTypes[pos.x, pos.y] == typeof(MoistureForest);
        }
        public static bool IsPlainLike(StandardMap map, Vector2Int pos) {
            pos = map.Validate(pos);
            return map.AltitudeTypes[pos.x, pos.y] == typeof(AltitudePlain) && map.MoistureTypes[pos.x, pos.y] != typeof(MoistureForest);
        }

        public static bool IsPassable(StandardMap standardMap, Vector2Int pos) {
            if (standardMap == null) throw new Exception();
            return IsPlainLike(standardMap, pos);
        }
        public bool Passable => IsPassable(Map as StandardMap, Pos);



        private Type SpriteKeyBaseType;
        private Type SpriteKeyType;

        public Type TemporatureType { get; private set; }
        public Type AltitudeType { get; private set; }
        public Type MoistureType { get; private set; }


        //private string Longitude() {
        //    if (Pos.x < Map.Width / 2) {
        //        if (Pos.x == 0) {
        //            return "经线180°";
        //        }
        //        return $"西经{(int)((Map.Width / 2 - Pos.x) * 360f / Map.Width)}°";
        //    } else if (Pos.x > Map.Width / 2) {
        //        return $"东经{(int)((Pos.x - Map.Width / 2) * 360f / Map.Width)}°";
        //    } else {
        //        return "经线180°";
        //    }
        //}
        //private string Latitude() {
        //    if (Pos.y < Map.Width / 2) {
        //        if (Pos.x == 0) {
        //            return "极地90°";
        //        }
        //        return $"南纬{(int)((Map.Width / 2 - Pos.y) * 180f / Map.Width)}°";
        //    } else if (Pos.x > Map.Width / 2) {
        //        return $"北纬{(int)((Pos.y - Map.Width / 2) * 180f / Map.Width)}°";
        //    } else {
        //        return "极地90°";
        //    }
        //}

        //private void Test() {
        //    string source = AESPack.CorrectAnswer;
        //    string answerKey = "!!!!!!";
        //    string encrypted = AESUtility.Encrypt(source, answerKey);
        //    string decrypted = AESUtility.Decrypt(encrypted, answerKey);
        //    Debug.LogWarning($" {source} {encrypted} {decrypted}");
        //}

        public override void OnTap() {
            // Test();
            StandardMap map = Map as StandardMap;
            if (Map == null) throw new Exception();
            ILandable landable = Map as ILandable;
            if (landable == null) {
                throw new Exception();
            }


            var items = new List<IUIItem>();

            // string title = $"{Localization.Ins.Get(TerrainType)} {Longitude()} {Latitude()}";
            string title = $"{Localization.Ins.Get(TerrainType)}";

            if (IsBuildable()) {
                if (MapView.Ins.TheOnlyActiveMap.ControlCharacter) {
                    OnTapNearly(items);
                    //int distance = TileUtility.Distance(MapView.Ins.CharacterPosition, Pos, Map.Width, Map.Height);
                    //const int tapNearlyDistance = 5;
                    //if (distance <= tapNearlyDistance) {
                    //    OnTapNearly(items);
                    //} else {
                    //    items.Add(UIItem.CreateText($"点击的位置距离玩家{distance - 1}，太远了，无法互动"));
                    //}
                } else {
                    OnTapNearly(items);
                }
            } else if (TerrainType == typeof(TerrainType_Forest)) {
                items.Add(UIItem.CreateMultilineText($"这片森林位置太深，只能探索平原附近的森林"));
            } else if (TerrainType == typeof(TerrainType_Sea)) {
                items.Add(UIItem.CreateMultilineText($"这片海洋离海岸太远，只能探索海岸"));
            } else if (TerrainType == typeof(TerrainType_Mountain)) {
                items.Add(UIItem.CreateMultilineText($"这片山地海拔太高，只能探索山地的边界"));
            } else {
                // !IgnoreTool 的情况下，居然此地形不是以上三种
                throw new Exception();
            }

            UI.Ins.ShowItems(title, items);
        }
        public bool IsBuildable() {
            StandardMap map = Map as StandardMap;
            return IsPassable(map, Pos)
                    || IsPassable(map, Pos + Vector2Int.up)
                    || IsPassable(map, Pos + Vector2Int.down)
                    || IsPassable(map, Pos + Vector2Int.left)
                    || IsPassable(map, Pos + Vector2Int.right);
        }
        public bool IgnoreTool {
            get {
                StandardMap map = Map as StandardMap;
                return !(IsBuildable() || (UIItem.ShortcutType != null && Tag.GetAttribute<CanBeBuildOnNotPassableTerrainAttribute>(UIItem.ShortcutType) != null));
            }
        }

        // 优化
        private static string[] spriteKeyBuffer;
        private static void InitSpriteKeyBuffer() {
            spriteKeyBuffer = new string[6 * 8];
            for (int i = 0; i < 6 * 8; i++) {
                spriteKeyBuffer[i] = "Sea_" + i.ToString();
            }
        }

        public override string SpriteKeyBase {
            get {
                TryCalcSpriteKey();
                StandardMap standardMap = Map as StandardMap;
                if (standardMap == null) throw new Exception();
                return CalculateTerrainName(standardMap, Pos);
            }
        }

        private void TryCalcSpriteKey() {
            if (SpriteKeyType == null && SpriteKeyBaseType == null) {
                StandardMap standardMap = Map as StandardMap;
                if (standardMap == null) throw new Exception();

                AltitudeType = standardMap.AltitudeTypes[Pos.x, Pos.y];
                MoistureType = standardMap.MoistureTypes[Pos.x, Pos.y];
                TemporatureType = standardMap.TemporatureTypes[Pos.x, Pos.y];
            }
        }

        public static string CalculateTerrainName(StandardMap standardMap, Vector2Int pos) {
            ITile tile = standardMap.Get(pos);
            Type AltitudeType = standardMap.AltitudeTypes[pos.x, pos.y];
            Type MoistureType = standardMap.MoistureTypes[pos.x, pos.y];
            Type TemporatureType = standardMap.TemporatureTypes[pos.x, pos.y];
            // 海洋的特殊ruletile
            if (IsSeaLike(standardMap, pos)) {
                int index = TileUtility.Calculate6x8RuleTileIndex(otherTile => {
                    Vector2Int otherPos = otherTile.GetPos();
                    return IsSeaLike(otherTile.GetMap() as StandardMap, otherTile.GetPos());
                }, standardMap, pos);
                if (spriteKeyBuffer == null) InitSpriteKeyBuffer();
                return spriteKeyBuffer[index];
            }
            // 山地的特殊ruletile
            if (IsMountainLike(standardMap, pos)) {
                int index = TileUtility.Calculate6x8RuleTileIndex(otherTile => {
                    Vector2Int otherPos = otherTile.GetPos();
                    return IsMountainLike(otherTile.GetMap() as StandardMap, otherTile.GetPos());
                }, standardMap, pos);
                return "MountainSea_" + index.ToString();
            }
            if (MoistureType == typeof(MoistureForest)) {
                int index = TileUtility.Calculate6x8RuleTileIndex(otherTile => {
                    Vector2Int otherPos = otherTile.GetPos();
                    return IsForestLike(otherTile.GetMap() as StandardMap, otherTile.GetPos());
                }, standardMap, pos);
                return "Forest_" + index.ToString();
                // return $"Forest_{tile.GetTileHashCode() % 16}";
            }
            return null;
        }

    }
}

