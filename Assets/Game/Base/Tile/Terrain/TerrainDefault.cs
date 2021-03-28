
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public enum TerrainType
    {
        None, Any, Plain, Sea, Forest, Mountain
    }
    public class BindTerrainTypeAttribute : Attribute
    {
        public TerrainType Data { get; private set; }
        public BindTerrainTypeAttribute(TerrainType terrainType) {
            Data = terrainType;
        }
    }

    /// <summary>
    /// 迟早要换方式重构，不如先配置在这里吧
    /// </summary>
    public static class ConstructionConditionConfig
    {

        public static readonly Dictionary<Type, Func<Type, ITile, bool>> Conditions = new Dictionary<Type, Func<Type, ITile, bool>>() {

            { typeof(RoadForTransportable) ,  (Type type, ITile tile) => TerrainDefault.IsPassable(tile.GetMap() as StandardMap, tile.GetPos()) },

            { typeof(WareHouseOfGrass), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>() },
            { typeof(HuntingGround), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>() },
            { typeof(SeaFishery), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>() },
            { typeof(BerryBush), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>() },

            { typeof(ResidenceOfGrass), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>() },

            { typeof(Farm), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Algriculture>() },

            { typeof(ForestLoggingCamp), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectWood_Woodcutting>() },

            { typeof(WorkshopOfWoodcutting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>() },
            { typeof(ResidenceOfWood), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>() },
            { typeof(WareHouseOfWood), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>() },

            { typeof(MountainQuarry), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectMetalOre_Mining>() },
            { typeof(WorkshopOfStonecutting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectMetalOre_Mining>() },

            { typeof(MineOfCopper), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectMetalOre_Mining>() },
            { typeof(MineOfIron), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectMetalOre_Mining>() },

            { typeof(WorkshopOfCopperSmelting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },
            { typeof(WorkshopOfIronSmelting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },

            { typeof(WorkshopOfWheelPrimitive), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },
            { typeof(TransportStationSimpliest), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },
            { typeof(TransportStationDestSimpliest), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },

            { typeof(WorkshopOfCopperCasting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
            { typeof(WorkshopOfIronCasting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },

            { typeof(MineOfCoal), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
            // { typeof(RefineryOfCoal), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
            { typeof(MineOfClay), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
            { typeof(WorkshopOfBrickMaking), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },

            { typeof(PowerPlant), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
            { typeof(OilDriller), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
            { typeof(RoadForFluid), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
        };
    }

    public class TerrainDefault : StandardTile, IPassable, IDontSave, IIgnoreTool
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

            if (GameConfig.CheatMode) {
                TryConstructButton<CheatHouse>();
            }

            StandardMap map = Map as StandardMap;
            if (map == null) throw new Exception();

            // 探索功能
            items.Add(UIItem.CreateButton("探索", ExplorationPage));
            // 其他建造方法
            bool isPlain = IsPlainLike(map, Pos);

            if (isPlain) items.Add(UIItem.CreateButton("建造【物流】类", ConstructLogisticsPage));
            if (isPlain) items.Add(UIItem.CreateButton("建造【农业】类", ConstructAgriculturePage));
            else if (IsForestLike(map, Pos) && MainQuest.Ins.IsUnlocked<Quest_CollectWood_Woodcutting>()) items.Add(UIItem.CreateButton("建造【林业】类", ConstructForestryPage));
            else if (IsMountainLike(map, Pos) && MainQuest.Ins.IsUnlocked<Quest_CollectMetalOre_Mining>()) items.Add(UIItem.CreateButton("建造【矿业】类", ConstructMiningPage));
            else if (IsSeaLike(map, Pos)) items.Add(UIItem.CreateButton("建造【渔业】类", ConstructFisheryPage));
            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>()) items.Add(UIItem.CreateButton("建造【住房】类", ConstructResidencePage));
            if (isPlain && MainQuest.Ins.IsUnlocked<Quest_CollectWood_Woodcutting>()) items.Add(UIItem.CreateButton("建造【工业】类", ConstructIndustryPage));

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

            if (IsForestLike(map, Pos)) {
                title = $"探索森林中";
                items.Add(CreateGatheringButton("捕猎", typeof(DeerMeat), 2, 1));
                items.Add(CreateGatheringButton("伐木", typeof(Wood), 2, 1));
            } else if (IsPlainLike(map, Pos)) {
                title = $"探索平原中";
                items.Add(CreateGatheringButton("采集", typeof(Berry), 2, 1));
            } else if (IsSeaLike(map, Pos)) {
                title = $"探索海岸中";
                items.Add(CreateGatheringButton("捕鱼", typeof(FishFlesh), 2, 1));
            } else if (IsMountainLike(map, Pos)) {
                title = $"探索山地中";
                items.Add(CreateGatheringButton("采石", typeof(Stone), 5, 1));
                items.Add(CreateGatheringButton("采铜矿", typeof(CopperOre), 10, 1));
                items.Add(CreateGatheringButton("采铁矿", typeof(IronOre), 10, 1));
            } else {
                title = $"这里没有可探索的东西";
            }

            UI.Ins.ShowItems(title, items);
        }
        private UIItem CreateGatheringButton(string text, Type type, long cost, long revenue) {
            return UIItem.CreateDynamicButton($"{text} {Localization.Ins.ValPlus(type, revenue)} {Localization.Ins.ValPlus<Sanity>(-cost)}", () => {
                if (Map.Inventory.CanAdd((type, revenue))) {
                    if (!Globals.SanityCheck(cost)) return;
                    Map.Inventory.Add((type, revenue));
                    Globals.SetCooldown = cost / 2;
                }
                else {
                    UIPreset.InventoryFull(() => UI.Ins.Active = false, Map.Inventory);
                }
            }, () => sanity.Val >= cost && Globals.IsCool);
        }














        private void ConstructLogisticsPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;
            TryConstructButton<RoadForTransportable>();
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

        private void ConstructResidencePage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;
            TryConstructButton<ResidenceOfGrass>();
            TryConstructButton<ResidenceOfWood>();
            TryConstructButton<ResidenceOfStone>();
            TryConstructButton<ResidenceOfBrick>();
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

            ItemsBuffer = null;

            UI.Ins.ShowItems("林业", items);
        }
        private void ConstructFisheryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;
            // 海岸
            TryConstructButton<SeaFishery>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("渔业", items);
        }
        private void ConstructMiningPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            // 山地
            TryConstructButton<MineOfSand>();
            TryConstructButton<MineOfSalt>();
            TryConstructButton<MineOfClay>();
            TryConstructButton<MountainQuarry>();
            TryConstructButton<MineOfIron>();
            TryConstructButton<MineOfCopper>();
            TryConstructButton<MineOfCoal>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("矿业", items);
        }

        private void ConstructIndustryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            items.Add(UIItem.CreateButton("【制造工业】", ConstructAssemblerPage));
            items.Add(UIItem.CreateButton("【冶金工业】", ConstructSmelterPage));
            items.Add(UIItem.CreateButton("【电力工业】", ConstructPowerGenerationPage));
            items.Add(UIItem.CreateButton("【石油工业】", ConstructPetroleumIndustryPage));

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

            TryConstructButton<PowerPlant>();

            ItemsBuffer = null;
            UI.Ins.ShowItems("电力", items);
        }
        private void ConstructPetroleumIndustryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(ConstructIndustryPage));

            ItemsBuffer = items;

            TryConstructButton<OilDriller>();

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
                switch (attr.Data) {
                    case TerrainType.Any:
                        // 可以建造到任何地形上
                        break;
                    case TerrainType.Mountain:
                        if (!IsMountainLike(map, Pos)) return false;
                        break;
                    case TerrainType.Forest:
                        if (!IsForestLike(map, Pos)) return false;
                        break;
                    case TerrainType.Plain:
                        if (!IsPlainLike(map, Pos)) return false;
                        break;
                    case TerrainType.Sea:
                        if (!IsSeaLike(map, Pos)) return false;
                        break;
                    default:
                        throw new Exception();
                }
            } else {
                // 没指定的建筑，默认必须在平原上
                if (!IsPlainLike(map, Pos)) return false;
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


















        private Type SpriteKeyBaseType;
        private Type SpriteKeyType;

        public Type TemporatureType { get; private set; }
        public Type AltitudeType { get; private set; }
        public Type MoistureType { get; private set; }

        public bool IsLikeSea { get => AltitudeType == typeof(AltitudeSea); }
        public bool IsLikeMountain { get => AltitudeType == typeof(AltitudeMountain) || TemporatureType == typeof(TemporatureFreezing); }
        private string Longitude() {
            if (Pos.x < Map.Width / 2) {
                if (Pos.x == 0) {
                    return "经线180°";
                }
                return $"西经{(int)((Map.Width / 2 - Pos.x) * 360f / Map.Width)}°";
            } else if (Pos.x > Map.Width / 2) {
                return $"东经{(int)((Pos.x - Map.Width / 2) * 360f / Map.Width)}°";
            } else {
                return "经线180°";
            }
        }
        private string Latitude() {
            if (Pos.y < Map.Width / 2) {
                if (Pos.x == 0) {
                    return "极地90°";
                }
                return $"南纬{(int)((Map.Width / 2 - Pos.y) * 180f / Map.Width)}°";
            } else if (Pos.x > Map.Width / 2) {
                return $"北纬{(int)((Pos.y - Map.Width / 2) * 180f / Map.Width)}°";
            } else {
                return "极地90°";
            }
        }

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

            string title;
            if (AltitudeType == typeof(AltitudeSea)) {
                title = $"海洋 {Longitude()} {Latitude()}";
            } else {
                title = $"{Localization.Ins.Get(TemporatureType)}{Localization.Ins.Get(MoistureType)}{Localization.Ins.Get(AltitudeType)} {Longitude()} {Latitude()}";
            }

            if (!landable.Landable) {
                bool allQuestsCompleted = MainQuest.Ins.IsUnlocked<Quest_CongratulationsQuestAllCompleted>();
                if (Passable) {
                    items.Add(UIItem.CreateMultilineText("这里地势平坦，火箭是否在此着陆"));
                    items.Add(UIItem.CreateButton("就在这里着陆", () => {
                        MainQuest.Ins.CompleteQuest(typeof(Quest_LandRocket));
                        Map.UpdateAt<PlanetLander>(Pos);
                        landable.Land(Pos);
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateButton("换个地方着陆", () => {
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateStaticButton(allQuestsCompleted ? "离开这个星球" : "离开这个星球 (主线任务通关后解锁)", () => {
                        GameEntry.Ins.EnterMap(typeof(MainMap));
                        UI.Ins.Active = false;
                    }, allQuestsCompleted));
                } else {
                    items.Add(UIItem.CreateMultilineText("火箭只能在空旷的平地着陆"));
                    items.Add(UIItem.CreateButton("继续寻找着陆点", () => {
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateStaticButton(allQuestsCompleted ? "离开这个星球" : "离开这个星球 (主线任务通关后解锁)", () => {
                        GameEntry.Ins.EnterMap(typeof(MainMap));
                        UI.Ins.Active = false;
                    }, allQuestsCompleted));
                }
            } else {
                if (!IgnoreTool) {
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
                } else if (IsForestLike(map, Pos)) {
                    items.Add(UIItem.CreateMultilineText("这片森林位置太深，只能探索平原附近的森林"));
                } else if (IsSeaLike(map, Pos)) {
                    items.Add(UIItem.CreateMultilineText("这片海洋离海岸太远，只能探索海岸"));
                } else if (IsMountainLike(map, Pos)) {
                    items.Add(UIItem.CreateMultilineText("这片高原太高，只能探索高原的边界"));
                }
            }

            UI.Ins.ShowItems(title, items);
        }
        public bool IgnoreTool {
            get {
                StandardMap map = Map as StandardMap;
                return !(IsPassable(map, Pos + Vector2Int.up) || IsPassable(map, Pos + Vector2Int.down)
                        || IsPassable(map, Pos + Vector2Int.left) || IsPassable(map, Pos + Vector2Int.right));
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
    }
}

