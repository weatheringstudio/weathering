
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Depend]
    [Concept]
    public class TerrainAttribute { }

    [Depend(typeof(TerrainAttribute))]
    [Concept]
    public class Altitude { }
    [Depend(typeof(TerrainAttribute))]
    [Concept]
    public class Moisture { }
    [Depend(typeof(TerrainAttribute))]
    [Concept]
    public class Temporature { }



    [Concept]
    [Depend(typeof(Altitude))]
    public class AltitudeSea { }

    [Concept]
    [Depend(typeof(Altitude))] // a > 0
    public class AltitudePlain { }

    [Concept]
    [Depend(typeof(Altitude))] // a > 3000
    public class AltitudeMountain { }



    [Concept]
    [Depend(typeof(Moisture))]
    public class MoistureDesert { }
    [Concept]
    [Depend(typeof(Moisture))] // a > 35
    public class MoistureGrassland { }
    [Concept]
    [Depend(typeof(Moisture))] // a > 55
    public class MoistureForest { }



    [Concept]
    [Depend(typeof(Temporature))]
    public class TemporatureTropical { }
    [Concept]
    [Depend(typeof(Temporature))]
    public class TemporatureTemporate { }
    [Concept]
    [Depend(typeof(Temporature))]
    public class TemporatureCold { }
    [Concept]
    [Depend(typeof(Temporature))]
    public class TemporatureFreezing { }







    public static class GeographyUtility
    {

        private static string Longitude(Vector2Int Pos, int width) {
            if (Pos.x < width / 2) {
                if (Pos.x == 0) {
                    return "经线180°";
                }
                return $"西经{(int)((width / 2 - Pos.x) * 360f / width)}°";
            } else if (Pos.x > width / 2) {
                return $"东经{(int)((Pos.x - width / 2) * 360f / width)}°";
            } else {
                return "经线180°";
            }
        }
        private static string Latitude(Vector2Int Pos, int height) {
            if (Pos.y < height / 2) {
                if (Pos.x == 0) {
                    return "极地90°";
                }
                return $"南纬{(int)((height / 2 - Pos.y) * 180f / height)}°";
            } else if (Pos.x > height / 2) {
                return $"北纬{(int)((Pos.y - height / 2) * 180f / height)}°";
            } else {
                return "极地90°";
            }
        }


        public static string DayTimeDescription(double x) {
            double delta = 0.125;
            if (x < delta) {
                return "早晨";
            } else if (x < 2 * delta) {
                return "上午";
            } else if (x < 3 * delta) {
                return "午后";
            } else if (x < 4 * delta) {
                return "黄昏";
            } else if (x < 5 * delta) {
                return "傍晚";
            } else if (x < 6 * delta) {
                return "晚上";
            } else if (x < 7 * delta) {
                return "午夜";
            } else {
                return "黎明";
            }
        }

        public static string DateDescription(int x) {
            switch (x) {
                case 1:
                    return "初一";
                case 2:
                    return "初二";
                case 3:
                    return "初三";
                case 4:
                    return "初四";
                case 5:
                    return "初五";
                case 6:
                    return "初六";
                case 7:
                    return "初七";
                case 8:
                    return "初八";
                case 9:
                    return "初九";
                case 10:
                    return "初十";
                case 11:
                    return "十一";
                case 12:
                    return "十二";
                case 13:
                    return "十三";
                case 14:
                    return "十四";
                case 15:
                    return "十五";
                case 16:
                    return "十六";
                case 17:
                    return "十七";
                case 18:
                    return "十八";
                case 19:
                    return "十九";
                case 20:
                    return "二十";
                case 21:
                    return "廿一";
                case 22:
                    return "廿二";
                case 23:
                    return "廿三";
                case 24:
                    return "廿四";
                case 25:
                    return "廿五";
                case 26:
                    return "廿六";
                case 27:
                    return "廿七";
                case 28:
                    return "廿八";
                case 29:
                    return "廿九";
                case 30:
                    return "三十";
                case 31:
                    return "三一";
                case 32:
                    return "三二";
                case 33:
                    return "三三";
                case 34:
                    return "三四";
                case 35:
                    return "三五";
                case 36:
                    return "三六";
                case 37:
                    return "三七";
                case 38:
                    return "三八";
                case 39:
                    return "三九";
                default:
                    return "零日";
            }
        }

        public static string MonthTimeDescription(int x) {
            switch (x) {
                case 1:
                    return "正月";
                case 2:
                    return "如月";
                case 3:
                    return "弥生";
                case 4:
                    return "清明";
                case 5:
                    return "皋月";
                case 6:
                    return "荷月";
                case 7:
                    return "文月";
                case 8:
                    return "叶月";
                case 9:
                    return "长月";
                case 10:
                    return "露月";
                case 11:
                    return "霜月";
                case 12:
                    return "腊月";
                default:
                    return "无月";
            }
        }

        public static string HourDescription(int x) {
            x = (x / 2) % 12;
            switch (x) {
                case 0:
                    return "子时";
                case 1:
                    return "丑时";
                case 2:
                    return "寅时";
                case 3:
                    return "卯时";
                case 4:
                    return "辰时";
                case 5:
                    return "巳时";
                case 6:
                    return "午时";
                case 7:
                    return "未时";
                case 8:
                    return "申时";
                case 9:
                    return "酉时";
                case 10:
                    return "戌时";
                case 11:
                    return "亥时";

                default:
                    return "无时";
            }
        }


        public static Type GetAltitudeType(int altitude) {
            if (altitude > 3000) {
                return typeof(AltitudeMountain);
            } else
            if (altitude > 0) {
                return typeof(AltitudePlain);
            } else {
                return typeof(AltitudeSea);
            }
        }

        public static Type GetMoistureType(int moisture) {
            if (moisture > 55) {
                return typeof(MoistureForest);
            } else if (moisture > 35) {
                return typeof(MoistureGrassland);
            } else {
                return typeof(MoistureDesert);
            }
        }

        public static Type GetTemporatureType(int temporature) {
            if (temporature > 20) {
                return typeof(TemporatureTemporate);
            } else if (temporature > 0) {
                return typeof(TemporatureTemporate);
            } else if (temporature > -20) {
                return typeof(TemporatureCold);
            } else {
                return typeof(TemporatureCold);
            }
        }
    }
}

