
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

    public static class ConstructionConditionConfig
    {

        public static readonly Dictionary<Type, Func<Type, ITile, bool>> Conditions = new Dictionary<Type, Func<Type, ITile, bool>>() {
            {typeof(Road) ,  (Type type, ITile tile) => Road.CanBeBuiltOn(tile) },

            { typeof(HuntingGround), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>() },

            { typeof(Village), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>() },
            { typeof(BerryBush), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Hunting>() },
            { typeof(SeaFishery), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_HavePopulation_Settlement>() },
            { typeof(Farm), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectFood_Algriculture>() },

            { typeof(ForestLoggingCamp), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectWood_Woodcutting>() },
            { typeof(WorkshopOfWoodcutting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>() },

            { typeof(MineOfCopper), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_CollectMetalOre_Mining>() },
            { typeof(WorkshopOfMetalSmelting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },
            { typeof(TransportStation), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },
            { typeof(TransportStationDest), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetal_Smelting>() },
            { typeof(WorkshopOfMetalCasting), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },

            { typeof(MineOfCoal), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
            { typeof(PowerPlant), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
            { typeof(OilDriller), (Type type, ITile tile) => MainQuest.Ins.IsUnlocked<Quest_ProduceMetalProduct_Casting>() },
        };
    }

    public class TerrainDefault : StandardTile
    {
        private List<IUIItem> ItemsBuffer;


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

            // 其他建造方法
            items.Add(UIItem.CreateButton("建造【物流类建筑】", ConstructLogisticsPage));
            items.Add(UIItem.CreateButton("建造【生产类建筑】", ConstructionProductionPage));
            items.Add(UIItem.CreateButton("建造【工业类建筑】", ConstructIndustryPage));

            ItemsBuffer = null;
        }

        private void ConstructionProductionPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;

            // TryConstruct<BerryBush>();
            TryConstructButton<SeaFishery>();
            TryConstructButton<HuntingGround>();
            TryConstructButton<Farm>();

            TryConstructButton<MineOfCoal>();
            TryConstructButton<MineOfCopper>();

            TryConstructButton<Village>();

            TryConstructButton<ForestLoggingCamp>();
            TryConstructButton<WorkshopOfWoodcutting>();
            TryConstructButton<WorkshopOfMetalSmelting>();
            TryConstructButton<WorkshopOfMetalCasting>();

            ItemsBuffer = null;

            UI.Ins.ShowItems("【生产类建筑】", items);
        }

        private void ConstructLogisticsPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;
            TryConstructButton<Road>();
            TryConstructButton<WareHouse>();
            TryConstructButton<TransportStation>();
            TryConstructButton<TransportStationDest>();
            ItemsBuffer = null;

            UI.Ins.ShowItems("【物流类建筑】", items);
        }

        private void ConstructIndustryPage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));

            ItemsBuffer = items;
            TryConstructButton<WorkshopOfWoodcutting>();
            TryConstructButton<WorkshopOfMetalSmelting>();
            TryConstructButton<WorkshopOfMetalCasting>();
            TryConstructButton<PowerPlant>();
            TryConstructButton<OilDriller>();
            ItemsBuffer = null;

            UI.Ins.ShowItems("【工业类建筑】", items);
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
                if (IsPassable(Map as StandardMap, Pos) && MoistureType != typeof(MoistureForest)) {
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
            }

            UI.Ins.ShowItems(title, items);
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
                return $"Forest_{tile.GetTileHashCode() % 16}";
            }
            return null;
        }
        public static bool IsSeaLike(StandardMap map, Vector2Int pos) {
            pos = map.Validate(pos);
            return map.AltitudeTypes[pos.x, pos.y] == typeof(AltitudeSea);
        }
        public static bool IsMountainLike(StandardMap map, Vector2Int pos) {
            pos = map.Validate(pos);
            return map.AltitudeTypes[pos.x, pos.y] != typeof(AltitudeSea)
            && (map.TemporatureTypes[pos.x, pos.y] == typeof(TemporatureFreezing)
            || map.AltitudeTypes[pos.x, pos.y] == typeof(AltitudeMountain));
        }
        public static bool IsForestLike(StandardMap map, Vector2Int pos) {
            pos = map.Validate(pos);
            return map.MoistureTypes[pos.x, pos.y] == typeof(MoistureForest);
        }
        public static bool IsPlainLike(StandardMap map, Vector2Int pos) {
            pos = map.Validate(pos);
            return map.AltitudeTypes[pos.x, pos.y] == typeof(AltitudePlain)
                && map.MoistureTypes[pos.x, pos.y] != typeof(MoistureForest);
        }

        public static bool IsPassable(StandardMap standardMap, Vector2Int pos) {
            if (standardMap == null) throw new Exception();
            return !(IsSeaLike(standardMap, pos) || IsMountainLike(standardMap, pos));
        }
    }
}

