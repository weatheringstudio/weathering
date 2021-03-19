
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    // 热带沙漠 热带草原 热带雨林
    // 温带沙漠 温带草原 温带森林
    // 寒带沙漠 寒带冻土 寒带针叶林
    // 海洋 平原 丘陵 山脉

    //public class ColorOfTropicalForestRainforest { }
    //public class ColorOfTropicalGrasslandSavanna { }
    //public class ColorOfTropicalDesert { }

    //public class ColorOfTemporateForest { }
    //public class ColorOfTemporateGrassland { }
    //public class ColorOfTemporateDesert { }

    //public class ColorOfColdForestConiferousForest { }
    //public class ColorOfColdGrasslandTundra { }
    //public class ColorOfColdDesert { }

    //public class ColorOfSea { }
    //public class ColorOfFreezingCold { }
    //public class ColorOfMountainPeak { }

    public class DecorationOfDesert { }
    public class DecorationOfColdDesert { }
    public class DecorationOfTropicalForest { }
    public class DecorationOfTemporateForest { }
    public class DecorationOfConiferousForest { }
    public class DecorationOfTropicalGrasslandSavanna { }
    public class DecorationOfTemporateGrassland { }
    public class DecorationOfColdGrasslandTundra { }
    public class DecorationOfMountainPeak { }
    public class DecorationOfFreezingCold { }

    public class DecorationOfTropicalMountain { }
    public class DecorationOfTemporateMountain { }
    public class DecorationOfColdMountain { }
    public class DecorationOfFreezingMountain { }


    public class TerrainDefault : StandardTile
    {
        private void OnTapNearly(List<IUIItem> items) {
            MainQuest quest = MainQuest.Ins;
            // 山地
            if (IsMountainLike(Map as StandardMap, Pos)) {
                items.Add(UIItem.CreateConstructionButton<MountainQuarry>(this));
                items.Add(UIItem.CreateConstructionButton<MountainMine>(this));
            }
            // 平原，非森林
            else if (AltitudeType == typeof(AltitudePlain) && MoistureType != typeof(MoistureForest)) {
                // MainQuest.Ins.CompleteQuest<SubQuest_ExplorePlanet_Plain>();
                if (quest.IsUnlocked<Quest_CollectFood_Hunting>()) {
                    // 仓库
                    items.Add(UIItem.CreateConstructionButton<WareHouse>(this));
                }
                if (Road.CanBeBuiltOn(this)) {
                    // 道路
                    items.Add(UIItem.CreateConstructionButton<Road>(this, true));
                }

                if (quest.IsUnlocked<Quest_ProduceMetal_Smelting>()) {
                    // 运输站
                    items.Add(UIItem.CreateConstructionButton<TransportStation>(this));
                    // 运输站终点
                    items.Add(UIItem.CreateConstructionButton<TransportStationDest>(this));
                }

                if (quest.IsUnlocked<Quest_HavePopulation_Settlement>()) {
                    // 村庄
                    items.Add(UIItem.CreateConstructionButton<Village>(this));
                }

                if (quest.IsUnlocked<Quest_CollectFood_Algriculture>()) {
                    // 农场
                    items.Add(UIItem.CreateConstructionButton<Farm>(this));
                }
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
            }
            // 森林
            else if (AltitudeType == typeof(AltitudePlain) && MoistureType == typeof(MoistureForest)) {
                if (quest.IsUnlocked<Quest_CollectFood_Hunting>()) {
                    //// 浆果丛
                    //items.Add(UIItem.CreateConstructionButton<BerryBush>(this));
                    // 猎场
                    items.Add(UIItem.CreateConstructionButton<HuntingGround>(this));
                }
                if (Road.CanBeBuiltOn(this)) {
                    // 道路
                    items.Add(UIItem.CreateConstructionButton<Road>(this, true));
                }
                if (quest.IsUnlocked<Quest_CollectWood_Woodcutting>()) {
                    // 木场
                    items.Add(UIItem.CreateConstructionButton<ForestLoggingCamp>(this));
                }
            }
            // 海洋
            else if (AltitudeType == typeof(AltitudeSea)) {
                //// MainQuest.Ins.CompleteQuest<SubQuest_ExplorePlanet_Sea>();
                //// 渔场
                //items.Add(UIItem.CreateConstructionButton<SeaFishery>(this));
            }

            if (quest.IsUnlocked<Quest_CollectFood_Algriculture>()) {
                items.Add(UIItem.CreateConstructionButton<AESReward>(this));
            }
        }

        // --------------------------------------------------

        private Type SpriteKeyBaseType;
        private Type SpriteKeyType;

        public Type TemporatureType { get; private set; }
        public Type AltitudeType { get; private set; }
        public Type MoistureType { get; private set; }

        public bool IsLikeSea { get => AltitudeType == typeof(AltitudeSea); }
        public bool IsLikeMountain { get => SpriteKeyType == typeof(DecorationOfFreezingCold); }
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

        private void Test() {
            string source = AESPack.CorrectAnswer;
            string answerKey = "!!!!!!";
            string encrypted = AESUtility.Encrypt(source, answerKey);
            string decrypted = AESUtility.Decrypt(encrypted, answerKey);
            Debug.LogWarning($" {source} {encrypted} {decrypted}");
        }

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


        //public bool Passable {
        //    get {
        //        return !(AltitudeType == typeof(AltitudeMountain) || AltitudeType == typeof(AltitudeSea) || TemporatureType == typeof(TemporatureFreezing));
        //    }
        //}

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
                //// 海洋的特殊ruletile
                //if (AltitudeType == typeof(AltitudeSea)) {
                //    int index = TileUtility.Calculate6x8RuleTileIndex(tile => {
                //        Vector2Int pos = tile.GetPos();
                //        return (Map as StandardMap).AltitudeTypes[pos.x, pos.y] == typeof(AltitudeSea);
                //    }, Map, Pos);
                //    if (spriteKeyBuffer == null) InitSpriteKeyBuffer();
                //    return spriteKeyBuffer[index];
                //}
                //// 山地的特殊ruletile
                //if (SpriteKeyType == DecorationOfMountain) {
                //    int index = TileUtility.Calculate6x8RuleTileIndex(tile => {
                //        TerrainDefault terrainDefault = tile as TerrainDefault;
                //        if (terrainDefault == null) return false;
                //        terrainDefault.TryCalcSpriteKey();
                //        return (terrainDefault).SpriteKeyType == DecorationOfMountain;
                //    }, Map, Pos);
                //    return "MountainSea_" + index.ToString();
                //}
                //if (SpriteKeyType == typeof(DecorationOfTemporateForest)) {
                //    return $"Forest_{HashCode % 16}";
                //}
                //return SpriteKeyType?.Name; // 这里产生了很多GCAlloc，
            }
        }

        private void TryCalcSpriteKey() {
            if (SpriteKeyType == null && SpriteKeyBaseType == null) {
                StandardMap standardMap = Map as StandardMap;
                if (standardMap == null) throw new Exception();

                AltitudeType = standardMap.AltitudeTypes[Pos.x, Pos.y];
                MoistureType = standardMap.MoistureTypes[Pos.x, Pos.y];
                TemporatureType = standardMap.TemporatureTypes[Pos.x, Pos.y];

                SpriteKeyType = CalculateTerrain(standardMap, Pos);
            }
        }

        public static string CalculateTerrainName(StandardMap standardMap, Vector2Int pos) {
            ITile tile = standardMap.Get(pos);
            Type calculatedTerrainType = CalculateTerrain(standardMap, pos);
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
            if (calculatedTerrainType == typeof(DecorationOfTemporateForest)) {
                return $"Forest_{tile.GetTileHashCode() % 16}";
            }
            return calculatedTerrainType.Name;
        }
        public static bool IsSeaLike(StandardMap standardMap, Vector2Int pos) {
            pos = standardMap.Validate(pos);
            return standardMap.AltitudeTypes[pos.x, pos.y] == typeof(AltitudeSea);
        }
        public static bool IsMountainLike(StandardMap standardMap, Vector2Int pos) {
            pos = standardMap.Validate(pos);
            return standardMap.AltitudeTypes[pos.x, pos.y] != typeof(AltitudeSea)
            && (standardMap.TemporatureTypes[pos.x, pos.y] == typeof(TemporatureFreezing)
            || standardMap.AltitudeTypes[pos.x, pos.y] == typeof(AltitudeMountain));
        }
        public static bool IsPassable(StandardMap standardMap, Vector2Int pos) {
            if (standardMap == null) throw new Exception();
            return !(IsSeaLike(standardMap, pos) || IsMountainLike(standardMap, pos));
        }

        private static Type CalculateTerrain(StandardMap standardMap, Vector2Int pos) {
            Type altitudeType = standardMap.AltitudeTypes[pos.x, pos.y];
            Type moistureType = standardMap.MoistureTypes[pos.x, pos.y];
            Type temporatureType = standardMap.TemporatureTypes[pos.x, pos.y];

            Type result = null;
            if (altitudeType == typeof(AltitudeSea)) {
                // SpriteKeyBaseType = typeof(ColorOfSea);
            } else if (temporatureType == typeof(TemporatureFreezing)) {
            } else if (altitudeType == typeof(AltitudeMountain)) {
                //// SpriteKeyBaseType = typeof(ColorOfTemporateGrassland);
                //if (temporatureType == typeof(TemporatureTropical)) {
                //    result = DecorationOfMountain; // typeof(DecorationOfTropicalMountain);
                //} else if (temporatureType == typeof(TemporatureTemporate)) {
                //    result = DecorationOfMountain; // typeof(DecorationOfTemporateMountain);
                //} else if (temporatureType == typeof(TemporatureCold)) {
                //    result = DecorationOfMountain; // typeof(DecorationOfColdMountain);
                //} else if (temporatureType == typeof(TemporatureFreezing)) {
                //    result = DecorationOfMountain; // typeof(DecorationOfFreezingMountain);
                //} else {
                //    throw new Exception();
                //}
            } else if (altitudeType == typeof(AltitudePlain)) {
                // 沙漠
                if (moistureType == typeof(MoistureDesert)) {
                    if (temporatureType == typeof(TemporatureTropical)) {
                        //SpriteKeyBaseType = typeof(ColorOfTropicalDesert);
                        result = typeof(DecorationOfDesert);
                    } else if (temporatureType == typeof(TemporatureTemporate)) {
                        //SpriteKeyBaseType = typeof(ColorOfTemporateDesert);
                        result = typeof(DecorationOfDesert);
                    } else if (temporatureType == typeof(TemporatureCold)) {
                        //SpriteKeyBaseType = typeof(ColorOfColdDesert);
                        result = typeof(DecorationOfColdDesert);
                    } else if (temporatureType == typeof(TemporatureFreezing)) {
                        //SpriteKeyBaseType = typeof(ColorOfFreezingCold);
                    } else {
                        throw new Exception();
                    }
                }
                // 草原
                else if (moistureType == typeof(MoistureGrassland)) {
                    if (temporatureType == typeof(TemporatureTropical)) {
                        //SpriteKeyBaseType = typeof(ColorOfTropicalGrasslandSavanna);
                        result = typeof(DecorationOfTropicalGrasslandSavanna);
                    } else if (temporatureType == typeof(TemporatureTemporate)) {
                        //SpriteKeyBaseType = typeof(ColorOfTemporateGrassland);
                        result = typeof(DecorationOfTemporateGrassland);
                    } else if (temporatureType == typeof(TemporatureCold)) {
                        //SpriteKeyBaseType = typeof(ColorOfColdGrasslandTundra);
                        result = typeof(DecorationOfColdGrasslandTundra);
                    } else if (temporatureType == typeof(TemporatureFreezing)) {
                        //SpriteKeyBaseType = typeof(ColorOfFreezingCold);
                    } else {
                        throw new Exception();
                    }
                }
                // 森林
                else if (moistureType == typeof(MoistureForest)) {
                    if (temporatureType == typeof(TemporatureTropical)) {
                        //SpriteKeyBaseType = typeof(ColorOfTropicalForestRainforest);
                        result = typeof(DecorationOfTemporateForest); //; typeof(DecorationOfTropicalForest);
                    } else if (temporatureType == typeof(TemporatureTemporate)) {
                        //SpriteKeyBaseType = typeof(ColorOfTemporateForest);
                        result = typeof(DecorationOfTemporateForest);
                    } else if (temporatureType == typeof(TemporatureCold)) {
                        //SpriteKeyBaseType = typeof(ColorOfColdForestConiferousForest);
                        result = typeof(DecorationOfTemporateForest); // typeof(DecorationOfConiferousForest);
                    } else if (temporatureType == typeof(TemporatureFreezing)) {
                        //SpriteKeyBaseType = typeof(ColorOfFreezingCold);
                    } else {
                        throw new Exception();
                    }
                }
            } else {
                throw new Exception();
            }
            return result;
        }
    }
}

