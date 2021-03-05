
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



    public class TerrainDefault : StandardTile, IPassable, ISealike
    {
        private Type SpriteKeyBaseType;
        private Type SpriteKeyType;

        Type temporatureType;
        Type altitudeType;
        Type moistureType;

        public bool IsLikeSea { get => altitudeType == typeof(AltitudeSea); }
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

        public override void OnTap() {
            ILandable landable = Map as ILandable;
            if (landable == null) {
                throw new Exception();
            }
            var items = new List<IUIItem>();
            string title;
            if (altitudeType == typeof(AltitudeSea)) {
                title = $"海洋 {Longitude()} {Latitude()}";
            } else {
                title = $"{Localization.Ins.Get(temporatureType)}{Localization.Ins.Get(moistureType)}{Localization.Ins.Get(altitudeType)} {Longitude()} {Latitude()}";
            }

            if (!landable.Landable) {
                if (Passable && moistureType != typeof(MoistureForest)) {
                    items.Add(UIItem.CreateMultilineText("这里地势平坦，火箭是否在此着陆"));
                    items.Add(UIItem.CreateButton("就在这里着陆", () => {
                        MainQuest.Ins.CompleteQuest(typeof(Quest_LandRocket));
                        Map.UpdateAt<PlanetLander>(Pos);
                        landable.Land(Pos);
                    }));
                    items.Add(UIItem.CreateButton("换个地方着陆", () => {
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateButton("离开这个星球", () => {
                        GameEntry.Ins.EnterMap(typeof(MainMap));
                        UI.Ins.Active = false;
                    }));
                } else {
                    items.Add(UIItem.CreateMultilineText("火箭只能在空旷的平地着陆"));
                    items.Add(UIItem.CreateButton("继续寻找着陆点", () => {
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateButton("离开这个星球", () => {
                        GameEntry.Ins.EnterMap(typeof(MainMap));
                        UI.Ins.Active = false;
                    }));
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

        
        private void OnTapNearly(List<IUIItem> items) {
            // 山地
            if (altitudeType == typeof(AltitudeMountain)) {
                // MainQuest.Ins.CompleteQuest<SubQuest_ExplorePlanet_Mountain>();
                // items.Add(UIItem.CreateConstructionButton<MountainQuarry>(this));
                // items.Add(UIItem.CreateConstructionButton<MountainMine>(this));
            }
            // 平原，非森林
            else if (altitudeType == typeof(AltitudePlain) && moistureType != typeof(MoistureForest)) {
                // MainQuest.Ins.CompleteQuest<SubQuest_ExplorePlanet_Plain>();
                // 道路
                items.Add(UIItem.CreateConstructionButton<Road>(this, true));
                // 仓库
                items.Add(UIItem.CreateConstructionButton<WareHouse>(this));
                // 村庄
                items.Add(UIItem.CreateConstructionButton<Village>(this));
                // 农场
                items.Add(UIItem.CreateConstructionButton<Farm>(this));
            } 
            // 森林
            else if (altitudeType == typeof(AltitudePlain) && moistureType == typeof(MoistureForest)) {
                // MainQuest.Ins.CompleteQuest<SubQuest_ExplorePlanet_Forest>();
                items.Add(UIItem.CreateConstructionButton<Road>(this, true));
                //// 浆果丛
                //items.Add(UIItem.CreateConstructionButton<BerryBush>(this));
                // 猎场
                items.Add(UIItem.CreateConstructionButton<HuntingGround>(this));
            }
            // 海洋
            else if (altitudeType == typeof(AltitudeSea)) {
                // MainQuest.Ins.CompleteQuest<SubQuest_ExplorePlanet_Sea>();
                // 渔场
                items.Add(UIItem.CreateConstructionButton<SeaFishery>(this));
            }
        }
        public bool Passable {
            get {
                return !(altitudeType == typeof(AltitudeMountain) || altitudeType == typeof(AltitudeSea) || temporatureType == typeof(TemporatureFreezing));
            }
        }


        //public override string SpriteKeyBase {
        //    get {
        //        TryCalcSpriteKey();
        //        return SpriteKeyBaseType?.Name;
        //    }
        //}

        // 优化
        private static string[] spriteKeyBuffer;
        private static void InitSpriteKeyBuffer() {
            spriteKeyBuffer = new string[6 * 8];
            for (int i = 0; i < 6*8; i++) {
                spriteKeyBuffer[i] = "Sea_" + i.ToString();
            }
        }
        public override string SpriteKeyBase {
            get {
                TryCalcSpriteKey();
                if (altitudeType == typeof(AltitudeSea)) {
                    int index = TileUtility.Calculate6x8RuleTileIndex(tile => {
                        Vector2Int pos = tile.GetPos();
                        return (Map as StandardMap).AltitudeTypes[pos.x, pos.y] == typeof(AltitudeSea);
                    }, Map, Pos);
                    if (spriteKeyBuffer == null) InitSpriteKeyBuffer();
                    return spriteKeyBuffer[index];
                }
                return SpriteKeyType?.Name; // 这里产生了很多GCAlloc，
            }
        }

        private void TryCalcSpriteKey() {
            if (SpriteKeyType == null && SpriteKeyBaseType == null) {
                StandardMap standardMap = Map as StandardMap;
                if (standardMap == null) throw new Exception();

                altitudeType = standardMap.AltitudeTypes[Pos.x, Pos.y];
                moistureType = standardMap.MoistureTypes[Pos.x, Pos.y];
                temporatureType = standardMap.TemporatureTypes[Pos.x, Pos.y];

                SpriteKeyType = CalculateTerrain(standardMap, Pos);
            }
        }

        public static Type CalculateTerrain(StandardMap standardMap, Vector2Int pos) {
            Type altitudeType = standardMap.AltitudeTypes[pos.x, pos.y];
            Type moistureType = standardMap.MoistureTypes[pos.x, pos.y];
            Type temporatureType = standardMap.TemporatureTypes[pos.x, pos.y];

            Type result = null;
            if (altitudeType == typeof(AltitudeSea)) {
                // SpriteKeyBaseType = typeof(ColorOfSea);
            } else if (temporatureType == typeof(TemporatureFreezing)) {
                result = typeof(DecorationOfFreezingMountain);
            } else if (altitudeType == typeof(AltitudeMountain)) {
                // SpriteKeyBaseType = typeof(ColorOfTemporateGrassland);
                if (temporatureType == typeof(TemporatureTropical)) {
                    result = typeof(DecorationOfTropicalMountain);
                } else if (temporatureType == typeof(TemporatureTemporate)) {
                    result = typeof(DecorationOfTemporateMountain);
                } else if (temporatureType == typeof(TemporatureCold)) {
                    result = typeof(DecorationOfColdMountain);
                } else if (temporatureType == typeof(TemporatureFreezing)) {
                    result = typeof(DecorationOfFreezingMountain);
                } else {
                    throw new Exception();
                }
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
                        result = typeof(DecorationOfColdDesert);
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
                        result = typeof(DecorationOfFreezingCold);
                    } else {
                        throw new Exception();
                    }
                }
                // 森林
                else if (moistureType == typeof(MoistureForest)) {
                    if (temporatureType == typeof(TemporatureTropical)) {
                        //SpriteKeyBaseType = typeof(ColorOfTropicalForestRainforest);
                        result = typeof(DecorationOfTropicalForest);
                    } else if (temporatureType == typeof(TemporatureTemporate)) {
                        //SpriteKeyBaseType = typeof(ColorOfTemporateForest);
                        result = typeof(DecorationOfTemporateForest);
                    } else if (temporatureType == typeof(TemporatureCold)) {
                        //SpriteKeyBaseType = typeof(ColorOfColdForestConiferousForest);
                        result = typeof(DecorationOfConiferousForest);
                    } else if (temporatureType == typeof(TemporatureFreezing)) {
                        //SpriteKeyBaseType = typeof(ColorOfFreezingCold);
                        result = typeof(DecorationOfFreezingCold);
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

