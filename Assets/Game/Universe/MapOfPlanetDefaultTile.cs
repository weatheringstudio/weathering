
using System;
using System.Collections.Generic;
using UnityEngine;

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

    public class MapOfPlanetDefaultTile : StandardTile, IPassable, IDontSave, IIgnoreTool, ITileDescription
    {

        private static List<IUIItem> ItemsBuffer;
        private void OnTapNearly(List<IUIItem> items) {

            ItemsBuffer = items;

            // 快捷方式建造
            if (UIItem.ShortcutMap == Map) { // 上次建造的建筑和自己处于同一个地图。standardtile
                if (TryConstructButton(UIItem.ShortcutType)) {
                    items.Add(UIItem.CreateSeparator());
                }
            }



            StandardMap map = Map as StandardMap;
            if (map == null) throw new Exception();

            // 探索功能
            if (TerraformedTerrainType == typeof(TerrainType_Forest)) items.Add(UIItem.CreateButton("探索森林", ExplorationPage));
            // 其他建造方法
            bool isPlain = TerraformedTerrainType == typeof(TerrainType_Plain);


            if (OriginalTerrainType != typeof(TerrainType_Forest) && MainQuest.Ins.IsOnOrBefore<Quest_CollectFood_Initial>()) {
                items.Add(UIItem.CreateMultilineText("点击地图右上角 “?” 查看当前任务。\n点击屏幕上方半透明黑色区域，关闭此界面。"));
            }

            // TryConstructButton<LaunchSite>();

            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_ConstructBerryBushAndWareHouse_Initial>()) items.Add(UIItem.CreateButton("建造【物流】类", ConstructLogisticsPage));

            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>()) items.Add(UIItem.CreateButton("建造【经济】类", ConstructEconomyPage));

            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_ConstructBerryBushAndWareHouse_Initial>()) {
                items.Add(UIItem.CreateButton("建造【农业】类", ConstructAgriculturePage));
            } else if (TerraformedTerrainType == typeof(TerrainType_Forest) && MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>()) {
                items.Add(UIItem.CreateButton("建造【林业】类", ConstructForestryPage));
            } else if (TerraformedTerrainType == typeof(TerrainType_Mountain) && MainQuest.Ins.IsUnlocked<Quest_CollectStone_Stonecutting>()) {
                items.Add(UIItem.CreateButton("建造【矿业】类", ConstructMiningPage));
            } else if (TerraformedTerrainType == typeof(TerrainType_Sea) && MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>()) {
                // 水域的建筑列表展开了
                TryConstructButton<RoadAsBridge>();
                TryConstructButton<TransportStationPort>();
                TryConstructButton<TransportStationDestPort>();

                TryConstructButton<SeaFishery>();
                TryConstructButton<ResidenceCoastal>();
                TryConstructButton<SeaWaterPump>();
                TryConstructButton<OilDrillerOnSea>();
            }

            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>()) items.Add(UIItem.CreateButton("建造【住房】类", ConstructResidencePage));
            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>()) items.Add(UIItem.CreateButton("建造【工业】类", ConstructIndustryPage));
            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_CongratulationsQuestAllCompleted>()) items.Add(UIItem.CreateButton("建造【航天】类", ConstructSpaceIndustryPage));

            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>()) items.Add(UIItem.CreateButton("建造【特殊】类", ConstructSpecialsPage));


            ItemsBuffer = null;
        }

        private void TerraformPage() {
            var items = UI.Ins.GetItems();
            string title = "地貌改造";

            items.Add(UIItem.CreateReturnButton(ConstructSpecialsPage));

            items.Add(UIItem.CreateStaticButton($"恢复默认地形{Localization.Ins.Get(OriginalTerrainType)}", () => {
                TerraformedTerrainType = OriginalTerrainType;
                UI.Ins.Active = false;
            }, TerraformedTerrainType != OriginalTerrainType));

            items.Add(UIItem.CreateStaticButton($"改造成{Localization.Ins.Get<TerrainType_Plain>()}", () => {
                TerraformedTerrainType = typeof(TerrainType_Plain);
                UI.Ins.Active = false;
            }, TerraformedTerrainType != typeof(TerrainType_Plain)));

            items.Add(UIItem.CreateStaticButton($"改造成{Localization.Ins.Get<TerrainType_Forest>()}", () => {
                TerraformedTerrainType = typeof(TerrainType_Forest);
                UI.Ins.Active = false;
            }, TerraformedTerrainType != typeof(TerrainType_Forest)));

            items.Add(UIItem.CreateStaticButton($"改造成{Localization.Ins.Get<TerrainType_Sea>()}", () => {
                TerraformedTerrainType = typeof(TerrainType_Sea);
                UI.Ins.Active = false;
            }, TerraformedTerrainType != typeof(TerrainType_Sea)));

            items.Add(UIItem.CreateStaticButton($"改造成{Localization.Ins.Get<TerrainType_Mountain>()}", () => {
                TerraformedTerrainType = typeof(TerrainType_Mountain);
                UI.Ins.Active = false;
            }, TerraformedTerrainType != typeof(TerrainType_Mountain)));

            UI.Ins.ShowItems(title, items);
        }


        private void ExplorationPage() {

            IInventory inventory = Map.Inventory;
            if (inventory == null) throw new Exception();

            string title;

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Sanity));
            items.Add(UIItem.CreateTimeProgress<CoolDown>(Globals.CoolDown));

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
            }, () => Globals.Sanity.Val >= cost && Globals.IsCool);
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

        private void ConstructEconomyPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            TryConstructButton<CellarForPersonalStorage>();

            TryConstructButton<MarketForPlayer>();
            TryConstructButton<MarketForSpaceProgram>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("经济", items);
        }

        private void ConstructSpecialsPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            items.Add(UIItem.CreateButton("建造【装饰】类", ConstructDecorationPage));


            items.Add(UIItem.CreateButton("地貌改造", TerraformPage));

            if (GameConfig.CheatMode) {
                TryConstructButton<CheatHouse>();
            }

            ItemsBuffer = null;

            UI.Ins.ShowItems("特殊", items);
        }

        private void ConstructDecorationPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructSpecialsPage));

            ItemsBuffer = items;

            TryConstructButton<Pyramid>();
            TryConstructButton<Torii>();
            TryConstructButton<WallOfStoneBrick>();
            TryConstructButton<MagicSchool>();
            TryConstructButton<Wardenclyffe>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("装饰", items);
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
            if (MainQuest.Ins.IsUnlocked<Quest_ProduceLPG>()) items.Add(UIItem.CreateButton("【电子工业】", ConstructElectronicsPage));
            if (MainQuest.Ins.IsUnlocked<Quest_ProduceLPG>()) items.Add(UIItem.CreateButton("【化学工业】", ConstructChemistryPage));
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
            TryConstructButton<PowerGeneratorOfSolarPanelStation>();
            TryConstructButton<PowerGeneratorOfWindTurbineStation>();
            TryConstructButton<PowerGeneratorOfNulearFissionEnergy>();
            TryConstructButton<PowerGeneratorOfNulearFusionEnergy>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("电力", items);
        }
        private void ConstructElectronicsPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructIndustryPage));

            ItemsBuffer = items;

            TryConstructButton<FactoryOfConductorOfCopperWire>();
            TryConstructButton<FactoryOfCircuitBoardSimple>();
            TryConstructButton<FactoryOfCircuitBoardIntegrated>();
            TryConstructButton<FactoryOfCircuitBoardAdvanced>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("电子", items);
        }
        private void ConstructChemistryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructIndustryPage));

            ItemsBuffer = items;

            TryConstructButton<FactoryOfFuelPack_Oxygen_Hydrogen>();
            TryConstructButton<FactoryOfFuelPack_Oxygen_JetFuel>();
            TryConstructButton<FactoryOfElectrolysisOfWater>();
            TryConstructButton<FactoryOfElectrolysisOfSaltedWater>();
            TryConstructButton<FactoryOfDesalination>();
            TryConstructButton<AirSeparator>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("化学", items);
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
            TryConstructButton<FactoryOfJetFuel>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("石油", items);
        }



        private void ConstructSpaceIndustryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            TryConstructButton<MarketForSpaceProgram>();
            TryConstructButton<LaunchSite>();
            TryConstructButton<SpaceElevator>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("航天", items);
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
                if (TerraformedTerrainType != attr.BindedType) {
                    return false;
                }
            } else {
                // 没指定的建筑，默认必须在平原上
                if (TerraformedTerrainType != typeof(TerrainType_Plain)) return false;
            }
            // 自定义条件测试
            if (MapOfPlanetDefaultTile_ConstructionConditionConfiguration.Conditions.TryGetValue(type, out var test)) {
                if (!test(type, this)) {
                    return false;
                }
            }
            // 通过测试
            return true;
        }


        // --------------------------------------------------




        // 当接近平原，并且没有被CanBeBuildOnNotPassableTerrainAttribute强制建造时，无视工具
        public bool IgnoreTool {
            get {
                return !IsNearPlain() && !(UIItem.ShortcutType != null && Tag.GetAttribute<CanBeBuildOnNotPassableTerrainAttribute>(UIItem.ShortcutType) != null);
            }
        }
        // 只有平原可以通过
        public bool Passable => (Map as MapOfPlanet).GetRealTerrainType(Pos) == typeof(TerrainType_Plain);



        protected override bool PreserveLandscape => true;

        // 如果没有改造，并且是Map的默认类型，则不保存
        public bool DontSave => Refs == null && Values == null && Inventory == null; //; TerraformRef == null && GetType() == Map.DefaultTileType;

        public class Terraform { }

        private IRef TerraformRef;
        public Type OriginalTerrainType { get; private set; }
        public Type TerraformedTerrainType { get => TerraformRef == null ? OriginalTerrainType : TerraformRef.Type; set {
                if (value == null) throw new ArgumentNullException();
                // 为了DontSave，逻辑比较麻烦
                if (value == OriginalTerrainType) {
                    if (TerraformRef != null) {
                        TerraformRef = null;
                        if (Refs != null) {
                            Refs.Remove<Terraform>();
                            if (Refs.Dict.Count == 0) {
                                Refs = null;
                            }
                        }
                    }
                }
                else {
                    if (TerraformRef == null) {
                        if (Refs == null) {
                            Refs = Weathering.Refs.GetOne();
                            TerraformRef = Refs.Create<Terraform>();
                        }
                        else {
                            if (Refs.TryGet<Terraform>(out TerraformRef)) {

                            }
                            else {
                                TerraformRef = Refs.Create<Terraform>();
                            }
                        }
                    }
                    TerraformRef.Type = value;
                }
                LinkUtility.NeedUpdateNeighbors8(this);
            } 
        }



        public string TileDescription => Localization.Ins.Get(TerraformedTerrainType);

        public override void OnConstruct(ITile oldTile) {
            base.OnConstruct(oldTile);

            if (oldTile != null) {
                if (oldTile.Refs != null) {
                    if (oldTile.Refs.TryGet<Terraform>(out TerraformRef)) {
                        TerraformedTerrainType = TerraformRef.Type;

                    }
                }
            }
        }

        public override void OnDestruct(ITile newTile) {
            base.OnDestruct(newTile);
            if (newTile != null && OriginalTerrainType != TerraformedTerrainType) {
                if (newTile.Refs == null) {
                    ISavableDefinition savableDefinition = (newTile as ISavableDefinition);
                    if (savableDefinition == null) throw new Exception();
                    savableDefinition.SetRefs(Weathering.Refs.GetOne());
                }
                newTile.Refs.Create<Terraform>().Type = TerraformedTerrainType;
            }
        }

        public override void OnEnable() {

            // 计算原始类型

            if (OriginalTerrainType == null) {
                OriginalTerrainType = (Map as MapOfPlanet).GetOriginalTerrainType(Pos);
            }

            // 计算改造类型
            if (TerraformRef != null) {
                if (TerraformRef.Type == null) throw new Exception();
            }
            else if (Refs != null && Refs.TryGet<Terraform>(out TerraformRef)) {
                throw new Exception();
                //if (TerraformRef.Type == null || TerraformRef.Type == OriginalTerrainType) throw new Exception();
                // TerraformedTerrainType = TerraformRef.Type;
            } else {
                TerraformedTerrainType = OriginalTerrainType;
            }

        }


        public override void OnTap() {
            ILandable landable = Map as ILandable;
            if (landable == null) {
                throw new Exception();
            }

            var items = new List<IUIItem>();

            string title = $"{Localization.Ins.Get(TerraformedTerrainType)}";

            if (IsNearPlain()) {
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
            } else if (TerraformedTerrainType == typeof(TerrainType_Forest)) {
                items.Add(UIItem.CreateMultilineText($"这片森林位置太深，只能探索平原附近的森林"));
            } else if (TerraformedTerrainType == typeof(TerrainType_Sea)) {
                items.Add(UIItem.CreateMultilineText($"这片海洋离海岸太远，只能探索海岸"));
            } else if (TerraformedTerrainType == typeof(TerrainType_Mountain)) {
                items.Add(UIItem.CreateMultilineText($"这片山地海拔太高，只能探索山地的边界"));
            } else {
                // !IgnoreTool 的情况下，居然此地形不是以上三种
                throw new Exception();
            }

            UI.Ins.ShowItems(title, items);
        }

        private bool IsNearPlain() {
            MapOfPlanet map = Map as MapOfPlanet;
            Type plain = typeof(TerrainType_Plain);
            return map.GetRealTerrainType(Pos) == plain
                    || map.GetRealTerrainType(Pos + Vector2Int.up) == plain
                    || map.GetRealTerrainType(Pos + Vector2Int.down) == plain
                    || map.GetRealTerrainType(Pos + Vector2Int.left) == plain
                    || map.GetRealTerrainType(Pos + Vector2Int.right) == plain;
        }

    }
}

