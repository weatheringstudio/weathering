

using System;
using System.Collections.Generic;

namespace Weathering
{
    /// <summary>
    /// 迟早要换方式重构，不如先配置在这里吧
    /// </summary>
    public static class MapOfPlanetDefaultTile_ConstructionConditionConfiguration
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
            { typeof(SeaWaterPump) , (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceElectricity>() },
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
}
