
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    // 热带沙漠 热带草原 热带雨林
    // 温带沙漠 温带草原 温带森林
    // 寒带沙漠 寒带冻土 寒带针叶林
    // 海洋 平原 丘陵 山脉

    public class ColorOfTropicalForestRainforest { }
    public class ColorOfTropicalGrasslandSavanna { }
    public class ColorOfTropicalDesert { }

    public class ColorOfTemporateForest { }
    public class ColorOfTemporateGrassland { }
    public class ColorOfTemporateDesert { }

    public class ColorOfColdForestConiferousForest { }
    public class ColorOfColdGrasslandTundra { }
    public class ColorOfColdDesert { }

    public class ColorOfSea { }
    public class ColorOfFreezingCold { }
    public class ColorOfMountainPeak { }

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


    public class TerrainDefault : StandardTile, IPassable
    {
        private int temporature;
        private int altitude;
        private int moisture;

        private Type SpriteKeyBaseType;
        private Type SpriteKeyType;

        TemporatureType temporatureType;
        AltitudeType altitudeType;
        MoistureType moistureType;

        public bool Passable {
            get {
                return !(altitudeType == AltitudeType.Mountain || altitudeType == AltitudeType.Sea);
            }
        }

        public override string SpriteKeyBase {
            get {
                TryCalcSpriteKey();
                return SpriteKeyBaseType?.Name;
            }
        }
        public override string SpriteKey {
            get {
                TryCalcSpriteKey();
                if (SpriteKeyBaseType == typeof(ColorOfSea)) {
                    int index = TileUtility.Calculate6x8RuleTileIndex(tile => {
                        TerrainDefault terrainDefault = tile as TerrainDefault;
                        if (terrainDefault == null) return false;
                        if (terrainDefault.altitudeType == AltitudeType.Sea) {
                            return true;
                        }
                        return false;
                    }, Map, Pos);
                    return "Sea_" + index.ToString();
                }
                return SpriteKeyType?.Name;
            }
        }
        private void TryCalcSpriteKey() {
            if (SpriteKeyType == null && SpriteKeyBaseType == null) {
                StandardMap standardMap = Map as StandardMap;
                if (standardMap == null) throw new Exception();
                temporature = standardMap.Temporatures[Pos.x, Pos.y];
                altitude = standardMap.Altitudes[Pos.x, Pos.y];
                moisture = standardMap.Moistures[Pos.x, Pos.y];

                altitudeType = standardMap.AltitudeTypes[Pos.x, Pos.y];
                moistureType = standardMap.MoistureTypes[Pos.x, Pos.y];
                temporatureType = standardMap.TemporatureTypes[Pos.x, Pos.y];

                if (altitudeType == AltitudeType.Sea) {
                    SpriteKeyBaseType = typeof(ColorOfSea);
                } else if (altitudeType == AltitudeType.Mountain) {
                    SpriteKeyBaseType = typeof(ColorOfTemporateGrassland);
                    if (temporatureType == TemporatureType.Tropical) {
                        SpriteKeyType = typeof(DecorationOfTropicalMountain);
                    } else if (temporatureType == TemporatureType.Temporate) {
                        SpriteKeyType = typeof(DecorationOfTemporateMountain);
                    } else if (temporatureType == TemporatureType.Cold) {
                        SpriteKeyType = typeof(DecorationOfColdMountain);
                    } else if (temporatureType == TemporatureType.Freezing) {
                        SpriteKeyType = typeof(DecorationOfFreezingMountain);
                    } else {
                        throw new Exception();
                    }
                } else if (altitudeType == AltitudeType.Plain) {
                    // 沙漠
                    if (moistureType == MoistureType.Desert) {
                        if (temporatureType == TemporatureType.Tropical) {
                            SpriteKeyBaseType = typeof(ColorOfTropicalDesert);
                            SpriteKeyType = typeof(DecorationOfDesert);
                        } else if (temporatureType == TemporatureType.Temporate) {
                            SpriteKeyBaseType = typeof(ColorOfTemporateDesert);
                            SpriteKeyType = typeof(DecorationOfDesert);
                        } else if (temporatureType == TemporatureType.Cold) {
                            SpriteKeyBaseType = typeof(ColorOfColdDesert);
                            SpriteKeyType = typeof(DecorationOfColdDesert);
                        } else if (temporatureType == TemporatureType.Freezing) {
                            SpriteKeyBaseType = typeof(ColorOfFreezingCold);
                            SpriteKeyType = typeof(DecorationOfColdDesert);
                        } else {
                            throw new Exception();
                        }
                    }
                    // 草原
                    else if (moistureType == MoistureType.Grassland) {
                        if (temporatureType == TemporatureType.Tropical) {
                            SpriteKeyBaseType = typeof(ColorOfTropicalGrasslandSavanna);
                            SpriteKeyType = typeof(DecorationOfTropicalGrasslandSavanna);
                        } else if (temporatureType == TemporatureType.Temporate) {
                            SpriteKeyBaseType = typeof(ColorOfTemporateGrassland);
                            SpriteKeyType = typeof(DecorationOfTemporateGrassland);
                        } else if (temporatureType == TemporatureType.Cold) {
                            SpriteKeyBaseType = typeof(ColorOfColdGrasslandTundra);
                            SpriteKeyType = typeof(DecorationOfColdGrasslandTundra);
                        } else if (temporatureType == TemporatureType.Freezing) {
                            SpriteKeyBaseType = typeof(ColorOfFreezingCold);
                            SpriteKeyType = typeof(DecorationOfFreezingCold);
                        } else {
                            throw new Exception();
                        }
                    }
                    // 森林
                    else if (moistureType == MoistureType.Forest) {
                        if (temporatureType == TemporatureType.Tropical) {
                            SpriteKeyBaseType = typeof(ColorOfTropicalForestRainforest);
                            SpriteKeyType = typeof(DecorationOfTropicalForest);
                        } else if (temporatureType == TemporatureType.Temporate) {
                            SpriteKeyBaseType = typeof(ColorOfTemporateForest);
                            SpriteKeyType = typeof(DecorationOfTemporateForest);
                        } else if (temporatureType == TemporatureType.Cold) {
                            SpriteKeyBaseType = typeof(ColorOfColdForestConiferousForest);
                            SpriteKeyType = typeof(DecorationOfConiferousForest);
                        } else if (temporatureType == TemporatureType.Freezing) {
                            SpriteKeyBaseType = typeof(ColorOfFreezingCold);
                            SpriteKeyType = typeof(DecorationOfFreezingCold);
                        } else {
                            throw new Exception();
                        }
                    }
                } else {
                    throw new Exception();
                }
            }
        }

        public override void OnTap() {
            // UI.Ins.ShowItems($"纬度{Mathf.Lerp(-90, 90, 1f * Pos.y / Map.Height)} 温度{temporature} 海拔{altitude}", null, null);
            UI.Ins.ShowItems($"坐标{Pos} 温度{temporatureType} 湿度{moistureType} 地势{altitudeType}", null, null);
        }
    }
}

