
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IReplacable
    {
        bool CanBeReplacedWith(Type type);
    }

    public class TerrainDefault : StandardTile, IReplacable
    {
        public bool CanBeReplacedWith(Type type) {
            StandardMap map = Map as StandardMap;
            if (map == null) throw new Exception();
            if (false) {

            } else if (IsPlainLike(map, Pos) && BuildingsOnPlain.Contains(type)) {
                return true;
            } else if (IsForestLike(map, Pos) && BuildingsOnForest.Contains(type)) {
                return true;
            } else if (IsMountainLike(map, Pos) && BuildingsOnMountain.Contains(type)) {
                return true;
            } else if (IsSeaLike(map, Pos) && BuildingsOnSea.Contains(type)) {
                return true;
            }
            return false;
        }
        private readonly HashSet<Type> BuildingsOnMountain = new HashSet<Type> {
            typeof(MountainMine),
            typeof(MineOfCoal),
            typeof(MineOfCopper),
            typeof(MountainQuarry),
        };
        private readonly HashSet<Type> BuildingsOnForest = new HashSet<Type> {
            typeof(Road),

            typeof(HuntingGround),
            typeof(ForestLoggingCamp),
        };
        private readonly HashSet<Type> BuildingsOnSea = new HashSet<Type> {

        };
        private readonly HashSet<Type> BuildingsOnPlain = new HashSet<Type> {
            typeof(Road),
            typeof(WareHouse),
            typeof(TransportStation),
            typeof(TransportStationDest),
            typeof(Village),
            typeof(Farm),

            typeof(WorkshopOfWoodcutting),
            typeof(WorkshopOfMetalSmelting),
            typeof(WorkshopOfMetalCasting),

            typeof(PowerPlant),
        };

        private void OnTapNearly(List<IUIItem> items) {

            StandardMap map = Map as StandardMap;
            if (map == null) throw new Exception();

            // 快捷方式建造
            if (UIItem.ShortcutMap == Map) { // 上次建造的建筑和自己处于同一个地图。standardtile
                Type shortcutType = UIItem.ShortcutType;
                if (CanBeReplacedWith(shortcutType)) {
                    items.Add(UIItem.CreateConstructionButton(shortcutType, this));
                }
            }

            // 其他建造方法

            MainQuest quest = MainQuest.Ins;
            // 山地
            if (IsMountainLike(Map as StandardMap, Pos)) {
                if (quest.IsUnlocked<Quest_CollectMetalOre_Mining>()) {
                    // items.Add(UIItem.CreateConstructionButton<MountainQuarry>(this));
                    items.Add(UIItem.CreateConstructionButton<MineOfCopper>(this));
                    items.Add(UIItem.CreateConstructionButton<MineOfCoal>(this));
                }
            }
            // 平原，非森林
            else if (IsPlainLike(Map as StandardMap, Pos)) {

                items.Add(UIItem.CreateButton("建造【物流类建筑】", ConstructLogisticsPage));

                if (quest.IsUnlocked<Quest_HavePopulation_Settlement>()) {
                    // 村庄
                    items.Add(UIItem.CreateConstructionButton<Village>(this));
                }

                if (quest.IsUnlocked<Quest_CollectFood_Algriculture>()) {
                    // 农场
                    items.Add(UIItem.CreateConstructionButton<Farm>(this));
                }

                items.Add(UIItem.CreateButton("建造【工业类建筑】", ConstructIndustryPage));

                if (quest.IsUnlocked<Quest_CollectFood_Algriculture>()) {
                    items.Add(UIItem.CreateConstructionButton<AESReward>(this));
                }
            }
            // 森林
            else if (IsForestLike(map, Pos)) {
                if (quest.IsUnlocked<Quest_CollectFood_Hunting>()) {
                    //// 浆果丛
                    //items.Add(UIItem.CreateConstructionButton<BerryBush>(this));
                    // 猎场
                    items.Add(UIItem.CreateConstructionButton<HuntingGround>(this));
                }
                if (Road.CanBeBuiltOn(this)) {
                    // 道路
                    items.Add(UIItem.CreateConstructionButton<Road>(this));
                }
                if (quest.IsUnlocked<Quest_CollectWood_Woodcutting>()) {
                    // 木场
                    items.Add(UIItem.CreateConstructionButton<ForestLoggingCamp>(this));
                }
            }
            // 海洋
            else if (IsSeaLike(map, Pos)) {
                //// MainQuest.Ins.CompleteQuest<SubQuest_ExplorePlanet_Sea>();
                //// 渔场
                //items.Add(UIItem.CreateConstructionButton<SeaFishery>(this));
            }
        }

        private void ConstructLogisticsPage() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateReturnButton(OnTap));

            MainQuest quest = MainQuest.Ins;

            if (quest.IsUnlocked<Quest_CollectFood_Hunting>()) {
                // 仓库
                items.Add(UIItem.CreateConstructionButton<WareHouse>(this));
            }
            if (Road.CanBeBuiltOn(this)) {
                // 道路
                items.Add(UIItem.CreateConstructionButton<Road>(this));
            }

            if (quest.IsUnlocked<Quest_ProduceMetal_Smelting>()) {
                // 运输站
                items.Add(UIItem.CreateConstructionButton<TransportStation>(this));
                // 运输站终点
                items.Add(UIItem.CreateConstructionButton<TransportStationDest>(this));
            }

            UI.Ins.ShowItems("【物流类建筑】", items);
        }

        private void ConstructIndustryPage() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateReturnButton(OnTap));

            MainQuest quest = MainQuest.Ins;

            if (quest.IsUnlocked<Quest_ProduceWoodProduct_WoodProcessing>()) {
                // 锯木厂
                items.Add(UIItem.CreateConstructionButton<WorkshopOfWoodcutting>(this));
            }
            if (quest.IsUnlocked<Quest_ProduceMetal_Smelting>()) {
                // 冶炼厂
                items.Add(UIItem.CreateConstructionButton<WorkshopOfMetalSmelting>(this));
            }
            if (quest.IsUnlocked<Quest_ProduceMetalProduct_Casting>()) {
                // 铸造厂
                items.Add(UIItem.CreateConstructionButton<WorkshopOfMetalCasting>(this));
            }

            items.Add(UIItem.CreateConstructionButton<PowerPlant>(this));

            UI.Ins.ShowItems("【工业类建筑】", items);
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

