
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

    public class ColorOfDeepSea { }
    public class ColorOfSea { }
    public class ColorOfFreezingCold { }
    public class ColorOfMountainPeak { }

    public class DecorationOfDesert { }
    public class DecorationOfTropicalForest { }
    public class DecorationOfTemporateForest { }
    public class DecorationOfConiferousForest { }
    public class DecorationOfTropicalGrasslandSavanna { }
    public class DecorationOfTemporateGrassland { }
    public class DecorationOfColdGrasslandTundra { }
    public class DecorationOfMountainPeak { }


    public class TerrainDefault : StandardTile
    {
        private int temporature;
        private int altitude;
        private int moisture;

        private Type SpriteKeyBaseType;
        private Type SpriteKeyType;

        TemporatureType temporatureType;
        AltitudeType altitudeType;
        MoistureType moistureType;

        public override string SpriteKeyBase {
            get {
                TryCalcSpriteKey();
                return SpriteKeyBaseType?.Name;
            }
        }
        public override string SpriteKey {
            get {
                TryCalcSpriteKey();
                if (SpriteKeyBaseType == typeof(ColorOfSea) || SpriteKeyBaseType == typeof(ColorOfDeepSea)) {
                    int index = TileUtility.Calculate6x8RuleTileIndex(tile => {
                        TerrainDefault terrainDefault = tile as TerrainDefault;
                        if (terrainDefault == null) return false;
                        if (terrainDefault.altitudeType == AltitudeType.DeepSea || terrainDefault.altitudeType == AltitudeType.Sea) {
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

                temporatureType = GeographyUtility.GetTemporatureType(temporature);
                altitudeType = GeographyUtility.GetAltitudeType(altitude);
                moistureType = GeographyUtility.GetMoistureType(moisture);

                if (altitudeType == AltitudeType.DeepSea) {
                    SpriteKeyBaseType = typeof(ColorOfDeepSea);
                } else if (altitudeType == AltitudeType.Sea) {
                    SpriteKeyBaseType = typeof(ColorOfSea);
                } else if (altitudeType == AltitudeType.MountainPeak) {
                    SpriteKeyType = typeof(DecorationOfMountainPeak);
                    SpriteKeyBaseType = typeof(ColorOfMountainPeak);
                } else if (altitudeType == AltitudeType.Plain || altitudeType == AltitudeType.Plateau) {
                    // 沙漠
                    if (moistureType == MoistureType.Desert) {
                        SpriteKeyType = typeof(DecorationOfDesert);
                        if (temporatureType == TemporatureType.Tropical) {
                            SpriteKeyBaseType = typeof(ColorOfTropicalDesert);
                        } else if (temporatureType == TemporatureType.Temporate) {
                            SpriteKeyBaseType = typeof(ColorOfTemporateDesert);
                        } else if (temporatureType == TemporatureType.Cold) {
                            SpriteKeyBaseType = typeof(ColorOfColdDesert);
                        } else if (temporatureType == TemporatureType.Freezing) {
                            SpriteKeyBaseType = typeof(ColorOfFreezingCold);
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
                        } else {
                            throw new Exception();
                        }
                    }
                    // 森林
                    else if (moistureType == MoistureType.Forest) {
                        if (temporatureType == TemporatureType.Tropical) {
                            SpriteKeyType = typeof(DecorationOfTropicalForest);
                            SpriteKeyBaseType = typeof(ColorOfTropicalForestRainforest);
                        } else if (temporatureType == TemporatureType.Temporate) {
                            SpriteKeyType = typeof(DecorationOfTemporateForest);
                            SpriteKeyBaseType = typeof(ColorOfTemporateForest);
                        } else if (temporatureType == TemporatureType.Cold) {
                            SpriteKeyType = typeof(DecorationOfConiferousForest);
                            SpriteKeyBaseType = typeof(ColorOfColdForestConiferousForest);
                        } else if (temporatureType == TemporatureType.Freezing) {
                            SpriteKeyBaseType = typeof(ColorOfFreezingCold);
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

