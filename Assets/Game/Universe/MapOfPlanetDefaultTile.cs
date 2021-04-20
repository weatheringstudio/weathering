
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public interface IMagnetAttraction
    {
        (Type, long) Attracted { get; }
    }

    public interface IMineralType { }

    public class BindMineralAttribute : Attribute
    {
        public Type TheType { get; private set; }
        public BindMineralAttribute(Type type) { TheType = type; }
    }

    public class BindMine : Attribute
    {
        public Type TheType { get; private set; }
        public BindMine(Type type) { TheType = type; }
    }


    public class MapOfPlanetDefaultTile : StandardTile, IPassable, IDontSave, IIgnoreTool, ITileDescription, IHasFrameAnimationOnSpriteKey, IMagnetAttraction
    {

        private static List<IUIItem> ItemsBuffer;
        private static bool unlockedGatheringBerryBuffer = false;
        private void OnTapNearly(List<IUIItem> items) {

            ItemsBuffer = items;

            // 快捷方式建造
            if (UIItem.HasShortcut && UIItem.ShortcutMap == Map) { // 上次建造的建筑和自己处于同一个地图。standardtile
                if (TryConstructButton(UIItem.ShortcutType)) {
                    items.Add(UIItem.CreateSeparator());
                }
            }


            StandardMap map = Map as StandardMap;
            if (map == null) throw new Exception();


            // 探索功能
            if (TerraformedTerrainType == typeof(TerrainType_Forest) && Unlocked<KnowledgeOfGatheringBerry>()) items.Add(UIItem.CreateButton("探索森林", ExplorationPage));

            if (!unlockedGatheringBerryBuffer) {
                unlockedGatheringBerryBuffer = Unlocked<KnowledgeOfGatheringBerry>();
                if (!unlockedGatheringBerryBuffer && TerraformedTerrainType != typeof(TerrainType_Plain)) {
                    items.Add(UIItem.CreateText($"需要点击平原，建造{Localization.Ins.Get<TotemOfNature>()}，解锁{Localization.Ins.Get<KnowledgeOfGatheringBerry>()}"));
                }
            }

            if (TerraformedTerrainType == typeof(TerrainType_Plain)) {
                items.Add(UIItem.CreateButton("建造【科技】类", ConstructTechnologyPage));
                if (Unlocked<WareHouseOfGrass>()) items.Add(UIItem.CreateButton("建造【物流】类", ConstructLogisticsPage));
                if (Unlocked<BerryBush>()) items.Add(UIItem.CreateButton("建造【农业】类", ConstructAgriculturePage));
                if (Unlocked<ResidenceOfGrass>()) items.Add(UIItem.CreateButton("建造【住房】类", ConstructResidencePage));
                if (Unlocked<WorkshopOfPaperMaking>()) items.Add(UIItem.CreateButton("建造【工业】类", ConstructIndustryPage));

                if (Unlocked<LibraryOfEconomy>()) items.Add(UIItem.CreateButton("建造【经济】类", ConstructEconomyPage));
                // items.Add(UIItem.CreateButton("建造【服务】类", ConstructServicePage));
                if (Unlocked<SchoolOfSpace>()) items.Add(UIItem.CreateButton("建造【航天】类", ConstructSpaceIndustryPage));
                // items.Add(UIItem.CreateButton("建造【特殊】类", ConstructSpecialsPage));
            } else if (TerraformedTerrainType == typeof(TerrainType_Forest)) {
                if (Unlocked<ForestLoggingCamp>()) items.Add(UIItem.CreateButton("建造【林业】类", ConstructForestryPage));
            } else if (TerraformedTerrainType == typeof(TerrainType_Mountain)) {
                // if (Unlocked<WorkshopOfWoodcutting>()) items.Add(UIItem.CreateButton("建造【矿业】类", ConstructMiningPage));

                if (MineralType == null) {
                    TryConstructButton<RoadAsTunnel>();
                    // TryConstructButton<MineOfSand>();
                    TryConstructButton<MineOfClay>();
                    TryConstructButton<MountainQuarry>();
                } else {
                    var attr = Tag.GetAttribute<BindMine>(MineralType);
                    if (attr != null) {
                        Type mineType = Tag.GetAttribute<BindMine>(MineralType).TheType;
                        if (mineType != null) {
                            if (Unlocked(mineType)) {
                                TryConstructButton(mineType);
                            } else {
                                items.Add(UIItem.CreateMultilineText($"目前科技水平不足以开采{Localization.Ins.ValUnit(MineralType)}"));
                            }
                        } else {
                            items.Add(UIItem.CreateText($"{Localization.Ins.Get(MineralType)}无法找到对应矿井"));
                        }
                    }
                }

            } else if (TerraformedTerrainType == typeof(TerrainType_Sea)) {
                // 水域的建筑列表展开了
                TryConstructButton<RoadAsBridge>();
                TryConstructButton<TransportStationPort>();
                TryConstructButton<TransportStationDestPort>();

                TryConstructButton<SeaFishery>();
                TryConstructButton<ResidenceCoastal>();
                TryConstructButton<SeaWaterPump>();
                TryConstructButton<OilDrillerOnSea>();
            }
            
            if (GameConfig.CheatMode) {
                // items.Add(UIItem.CreateButton("地貌改造", TerraformPage));
                TryConstructButton<CheatHouse>();
            }


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

            items.Add(UIItem.CreateTimeProgress<Sanity>(Globals.Sanity));
            items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Sanity));
            items.Add(UIItem.CreateTimeProgress<CoolDown>(Globals.CoolDown));

            title = $"探索森林中";
            if (Unlocked<KnowledgeOfGatheringBerry>()) {
                bool efficient = Unlocked<KnowledgeOfGatheringBerryEfficiently>();
                items.Add(CreateGatheringButton("采集", typeof(Berry), 2, efficient ? 5 : 1));
            }
            if (Unlocked<KnowledgeOfGatheringWood>()) {
                items.Add(CreateGatheringButton("伐木", typeof(Wood), 6, 1));

            }
            // items.Add(CreateGatheringButton("捕猎", typeof(DeerMeat), 2, 5));

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


        private void ConstructTechnologyPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            if (!Unlocked<ResidenceOfGrass>() && Unlocked<TotemOfAncestors>()) {
                items.Add(UIItem.CreateMultilineText($"对草地使用{Localization.Ins.Get<KnowledgeOfMagnet>()}可以获得{Localization.Ins.ValUnit<Grain>()}"));
            }

            TryConstructButton<TotemOfNature>();
            TryConstructButton<TotemOfAncestors>();

            TryConstructButton<LibraryOfAll>();
            TryConstructButton<SchoolOfAll>();

            if (Unlocked<WorkshopOfPaperMaking>()) items.Add(UIItem.CreateButton("建造【配套生产设施】类", ConstructTechnologyInfrastracturePage));
            if (Unlocked<LibraryOfAll>()) items.Add(UIItem.CreateButton("建造【图书馆】类", ConstructLibraryPage));
            if (Unlocked<SchoolOfAll>()) items.Add(UIItem.CreateButton("建造【学园】类", ConstructSchoolPage));

            ItemsBuffer = null;

            UI.Ins.ShowItems("科技", items);
        }

        private void ConstructTechnologyInfrastracturePage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructTechnologyPage));

            ItemsBuffer = items;

            TryConstructButton<WorkshopOfPaperMaking>();
            TryConstructButton<WorkshopOfBook>();
            TryConstructButton<WorkshopOfSchoolEquipment>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("配套生产设施", items);
        }

        private void ConstructLibraryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructTechnologyPage));

            ItemsBuffer = items;

            TryConstructButton<LibraryOfAll>();
            TryConstructButton<LibraryOfAgriculture>();
            TryConstructButton<LibraryOfGeography>();
            TryConstructButton<LibraryOfHandcraft>();
            TryConstructButton<LibraryOfLogistics>();
            TryConstructButton<LibraryOfEconomy>();
            TryConstructButton<LibraryOfConstruction>();
            TryConstructButton<LibraryOfMetalWorking>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("图书馆", items);
        }

        private void ConstructSchoolPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructTechnologyPage));

            ItemsBuffer = items;

            TryConstructButton<SchoolOfAll>();
            TryConstructButton<SchoolOfGeology>();
            TryConstructButton<SchoolOfEngineering>();
            TryConstructButton<SchoolOfLogistics>();
            TryConstructButton<SchoolOfChemistry>();
            TryConstructButton<SchoolOfPhysics>();
            TryConstructButton<SchoolOfElectronics>();
            TryConstructButton<SchoolOfSpace>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("学园", items);
        }


        private void ConstructLogisticsPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            if (Unlocked<RoadForSolid>()) items.Add(UIItem.CreateButton("建造【道路】类", ConstructRoadPage));
            if (Unlocked<WareHouseOfGrass>()) items.Add(UIItem.CreateButton("建造【仓库】类", ConstructWareHousePage));
            if (Unlocked<LibraryOfLogistics>()) items.Add(UIItem.CreateButton("建造【快递】类", ConstructDeliveryPage));
            if (Unlocked<LibraryOfLogistics>()) items.Add(UIItem.CreateButton("建造【批发】类", ConstructVehiclePage));

            ItemsBuffer = null;

            UI.Ins.ShowItems("物流", items);
        }

        private void ConstructRoadPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            TryConstructButton<RoadForSolid>();
            TryConstructButton<RoadOfStone>();
            TryConstructButton<RoadOfConcrete>();
            TryConstructButton<RoadForFluid>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("道路", items);
        }

        private void ConstructWareHousePage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            TryConstructButton<WareHouseOfGrass>();
            TryConstructButton<WareHouseOfWood>();
            TryConstructButton<WareHouseOfStone>();
            TryConstructButton<WareHouseOfBrick>();
            TryConstructButton<WareHouseOfConcrete>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("仓库", items);
        }

        private void ConstructVehiclePage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            TryConstructButton<RoadAsCanal>();
            TryConstructButton<RoadLoaderOfRoadAsCanal>();

            TryConstructButton<RoadAsRailRoad>();
            TryConstructButton<RoadLoaderOfRoadAsRailRoad>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("批发", items);
        }

        private void ConstructDeliveryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            TryConstructButton<TransportStationSimpliest>();
            TryConstructButton<TransportStationDestSimpliest>();

            TryConstructButton<TransportStationAirport>();
            TryConstructButton<TransportStationDestAirport>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("快递", items);
        }


        private void ConstructEconomyPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            TryConstructButton<CellarForPersonalStorage>();
            TryConstructButton<RecycleStation>();

            // TryConstructButton<MarketForPlayer>();
            TryConstructButton<MarketOfAgriculture>();
            TryConstructButton<MarketOfMineral>();
            TryConstructButton<MarketOfHandcraft>();
            TryConstructButton<MarketOfMetalProduct>();

            TryConstructButton<MarketForSpaceProgram>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("经济", items);
        }

        private void ConstructSpecialsPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            // items.Add(UIItem.CreateButton("建造【装饰】类", ConstructDecorationPage));


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

        private void ConstructServicePage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            // TryConstructButton<PrisonOfTruth>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("服务", items);
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





        public override string SpriteKey => MineralType == null ? null : MineralType.Name;

        public override string SpriteKeyOverlay {
            get {
                if (TerraformedTerrainType != typeof(TerrainType_Mountain)) return null;

                int index = TileUtility.Calculate6x8RuleTileIndex(this,
                    (ITile tile) =>
                    tile is MapOfPlanetDefaultTile defaultTile &&
                    defaultTile.TerraformedTerrainType == typeof(TerrainType_Mountain)
                );
                return $"Planet_Fog_{index}";
            }
        }

        public override string SpriteKeyGrass => base.SpriteKeyGrass;

        //private void ConstructMiningPage() {
        //    var items = UI.Ins.GetItems();
        //    items.Add(UIItem.CreateReturnButton(OnTap));

        //    ItemsBuffer = items;

        //    // 山地
        //    TryConstructButton<RoadAsTunnel>();
        //    TryConstructButton<MineOfSalt>();
        //    TryConstructButton<MineOfSand>();
        //    TryConstructButton<MineOfClay>();
        //    TryConstructButton<MountainQuarry>();
        //    TryConstructMineButton<MineOfGold>();
        //    TryConstructMineButton<MineOfIron>();
        //    TryConstructMineButton<MineOfCopper>();
        //    TryConstructMineButton<MineOfCoal>();
        //    TryConstructMineButton<MineOfAluminum>();

        //    ItemsBuffer = null;

        //    UI.Ins.ShowItems("矿业", items);
        //}

        //private void TryConstructMineButton<T>() {
        //    var attr = Tag.GetAttribute<MineOfMineralTypeAttribute>(typeof(T));
        //    if (attr == null) throw new Exception();

        //    if (attr.TheType != MineralType) return;
        //    TryConstructButton<T>();
        //}


        private void ConstructIndustryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            if (Unlocked<WorkshopOfPaperMaking>()) items.Add(UIItem.CreateButton("【制造工业】", ConstructAssemblerPage));
            if (Unlocked<LibraryOfMetalWorking>()) items.Add(UIItem.CreateButton("【冶金工业】", ConstructSmelterPage));
            if (Unlocked<SchoolOfPhysics>()) items.Add(UIItem.CreateButton("【电力工业】", ConstructPowerGenerationPage));
            if (Unlocked<SchoolOfElectronics>()) items.Add(UIItem.CreateButton("【电子工业】", ConstructElectronicsPage));
            if (Unlocked<SchoolOfChemistry>()) items.Add(UIItem.CreateButton("【化学工业】", ConstructChemistryPage));
            if (Unlocked<SchoolOfChemistry>()) items.Add(UIItem.CreateButton("【石油工业】", ConstructPetroleumIndustryPage));

            ItemsBuffer = null;

            UI.Ins.ShowItems("工业", items);
        }
        private void ConstructAssemblerPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructIndustryPage));

            ItemsBuffer = items;

            TryConstructButton<WorkshopOfPaperMaking>();
            TryConstructButton<WorkshopOfBook>();

            TryConstructButton<WorkshopOfWoodcutting>();
            TryConstructButton<WorkshopOfStonecutting>();
            TryConstructButton<WorkshopOfToolPrimitive>();
            TryConstructButton<WorkshopOfWheelPrimitive>();
            TryConstructButton<WorkshopOfBrickMaking>();
            TryConstructButton<WorkshopOfMachinePrimitive>();
            TryConstructButton<WorkshopOfSchoolEquipment>();

            TryConstructButton<WorkshopOfConcrete>();
            TryConstructButton<WorkshopOfBuildingPrefabrication>();
            TryConstructButton<FactoryOfLightMaterial>();

            TryConstructButton<FactoryOfCombustionMotor>();
            TryConstructButton<FactoryOfElectroMotor>();
            TryConstructButton<FactoryOfTurbine>();

            TryConstructButton<FactoryOfWindTurbineComponent>();
            TryConstructButton<FactoryOfSolarPanelComponent>();

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

            TryConstructButton<FactoryOfSteelPlate>();
            TryConstructButton<FactoryOfSteelPipe>();
            TryConstructButton<FactoryOfSteelRod>();
            TryConstructButton<FactoryOfSteelWire>();
            TryConstructButton<FactoryOfSteelGear>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("冶金", items);
        }
        private void ConstructPowerGenerationPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructIndustryPage));

            ItemsBuffer = items;

            TryConstructButton<FactoryOfDesalination>();
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

            TryConstructButton<FactoryOfDesalination>();
            TryConstructButton<AirSeparator>();
            TryConstructButton<FactoryOfElectrolysisOfSaltedWater>();
            TryConstructButton<FactoryOfElectrolysisOfWater>();
            TryConstructButton<FactoryOfFuelPack_Oxygen_Hydrogen>();
            TryConstructButton<FactoryOfFuelPack_Oxygen_JetFuel>();

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
            TryConstructButton<SpaceElevatorDest>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("航天", items);
        }








































































        // --------------------------------------------------

        private bool Unlocked<T>() {
            return Unlocked(typeof(T));
        }
        private bool Unlocked(Type type) {
            return Globals.Unlocked(type);
        }


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

            // 科技解锁测试
            if (!Unlocked(type)) {
                return false;
            }

            // 土地类型测试
            var attr = Tag.GetAttribute<BindTerrainTypeAttribute>(type);
            if (attr != null) {
                if (TerraformedTerrainType != attr.BindedType) {
                    return false;
                }
            } else {
                // 没指定的建筑, 默认必须在平原上
                if (TerraformedTerrainType != typeof(TerrainType_Plain)) return false;
            }

            //// 自定义条件测试
            //if (MapOfPlanetDefaultTile_ConstructionConditionConfiguration.Conditions.TryGetValue(type, out var test)) {
            //    if (!test(type, this)) {
            //        return false;
            //    }
            //}
            // 通过测试
            return true;
        }


        // --------------------------------------------------




        // 当不接近可移动地块, 并且没有被CanBeBuildOnNotPassableTerrainAttribute强制建造时, 无视工具
        public bool IgnoreTool {
            get {
                return !IsNearPlain(); // && !(UIItem.ShortcutType != null && Tag.GetAttribute<CanBeBuildOnNotPassableTerrainAttribute>(UIItem.ShortcutType) != null);
            }
        }
        public bool Passable {
            get {
                return (Map as MapOfPlanet).GetRealTerrainType(Pos) == typeof(TerrainType_Plain);
            }
        }



        protected override bool PreserveLandscape => true;

        // 如果没有改造, 并且是Map的默认类型, 则不保存
        public bool DontSave => Refs == null && Values == null && Inventory == null; //; TerraformRef == null && GetType() == Map.DefaultTileType;




        public class Terraform { }

        private IRef TerraformRef; // 记录 TerraformedTerrainType
        public Type OriginalTerrainType { get; private set; }


        public Type TerraformedTerrainType {
            get => TerraformRef == null ? OriginalTerrainType : TerraformRef.Type;
            set {
                if (value == null) throw new ArgumentNullException();
                // 为了DontSave, 逻辑比较麻烦
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
                } else {
                    if (TerraformRef == null) {
                        if (Refs == null) {
                            Refs = Weathering.Refs.GetOne();
                            TerraformRef = Refs.Create<Terraform>();
                        } else {
                            if (Refs.TryGet<Terraform>(out TerraformRef)) {

                            } else {
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

        public int HasFrameAnimation => 0; // TerraformedTerrainType == typeof(TerrainType_Sea) ? 10 : 0;


        public (Type, long) Attracted {
            get {
                float quantity = (float)(HashUtility.Hash((uint)TimeUtility.GetTicks()) % 100) / 100;
                quantity = quantity * quantity;
                long lerped = (long)Mathf.Lerp(1, 10, quantity);
                if (TerraformedTerrainType == typeof(TerrainType_Plain)) {
                    return (typeof(Grain), lerped);
                } else if (TerraformedTerrainType == typeof(TerrainType_Forest)) {
                    return (typeof(Wood), lerped);
                } else if (TerraformedTerrainType == typeof(TerrainType_Sea)) {
                    return (typeof(FishFlesh), lerped);
                } else if (TerraformedTerrainType == typeof(TerrainType_Mountain)) {
                    return (typeof(Stone), lerped);
                }
                return (null, 0);
            }
        }

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

        private Type MineralType = null;

        public override void OnEnable() {

            // 计算原始类型
            if (OriginalTerrainType == null) {
                OriginalTerrainType = (Map as MapOfPlanet).GetOriginalTerrainType(Pos);

                // 生成矿物
                if (OriginalTerrainType == typeof(TerrainType_Mountain)) {
                    uint hashcode = TileHashCode;

                    int mineralDensity = (Map as MapOfPlanet).MineralDensity;
                    if (mineralDensity <= 0) throw new Exception();

                    if (HashUtility.Hashed(ref hashcode) % mineralDensity == 0) {
                        // 有矿物
                        if (HashUtility.Hashed(ref hashcode) % 10 == 0) {
                            MineralType = typeof(GoldOre);
                        } else if (HashUtility.Hashed(ref hashcode) % 2 == 0) {
                            MineralType = typeof(Coal);
                        } else if (HashUtility.Hashed(ref hashcode) % 4 == 0) {
                            MineralType = typeof(CopperOre);
                        } else if (HashUtility.Hashed(ref hashcode) % 3 != 0) {
                            MineralType = typeof(IronOre);
                        } else {
                            MineralType = typeof(AluminumOre);
                        }
                    }
                }

            }

            // 计算改造类型
            if (TerraformRef != null) {
                if (TerraformRef.Type == null) throw new Exception();
            } else if (Refs != null && Refs.TryGet<Terraform>(out TerraformRef)) {
                throw new Exception();
                //if (TerraformRef.Type == null || TerraformRef.Type == OriginalTerrainType) throw new Exception();
                // TerraformedTerrainType = TerraformRef.Type;
            } else {
                TerraformedTerrainType = OriginalTerrainType;
            }

        }






        public override void OnTap() {
            /// 可降落
            if (Map as ILandable == null) throw new Exception();

            var items = new List<IUIItem>();

            items.Add(UIItem.CreateMultilineText($"debug pos: x {Pos.x} y {Pos.y}"));

            if (IsNearPlain()) {
                if (MapView.Ins.TheOnlyActiveMap.ControlCharacter) {
                    OnTapNearly(items);
                    //int distance = TileUtility.Distance(MapView.Ins.CharacterPosition, Pos, Map.Width, Map.Height);
                    //const int tapNearlyDistance = 5;
                    //if (distance <= tapNearlyDistance) {
                    //    OnTapNearly(items);
                    //} else {
                    //    items.Add(UIItem.CreateText($"点击的位置距离玩家{distance - 1}, 太远了, 无法互动"));
                    //}
                } else {
                    OnTapNearly(items);
                }
            } else if (TerraformedTerrainType == typeof(TerrainType_Forest)) {
                items.Add(UIItem.CreateMultilineText($"这片森林位置太深, 只能探索平原附近的森林"));
            } else if (TerraformedTerrainType == typeof(TerrainType_Sea)) {
                items.Add(UIItem.CreateMultilineText($"这片海洋离海岸太远, 只能探索海岸"));
            } else if (TerraformedTerrainType == typeof(TerrainType_Mountain)) {
                if (MineralType == null) {
                    items.Add(UIItem.CreateMultilineText($"这片山地海拔太高, 只能探索山地的边界"));
                } else {
                    OnTapNearly(items);
                }
            } else {
                // !IgnoreTool 的情况下, 居然此地形不是以上三种
                throw new Exception();
            }

            UI.Ins.ShowItems($"{Localization.Ins.Get(TerraformedTerrainType)}", items);
        }

        private bool IsNearPlain() {
            MapOfPlanet map = Map as MapOfPlanet;
            Type plain = typeof(TerrainType_Plain);
            return map.GetRealTerrainType(Pos) == plain
                    || (map.Get(Pos + Vector2Int.up) is IPassable passable0 && passable0.Passable)
                    || (map.Get(Pos + Vector2Int.down) is IPassable passable1 && passable1.Passable)
                    || (map.Get(Pos + Vector2Int.left) is IPassable passable2 && passable2.Passable)
                    || (map.Get(Pos + Vector2Int.right) is IPassable passable3 && passable3.Passable);
        }

    }
}

