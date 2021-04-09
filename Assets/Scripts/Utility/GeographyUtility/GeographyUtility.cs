
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



    //private string Longitude() {
    //    if (Pos.x < Map.Width / 2) {
    //        if (Pos.x == 0) {
    //            return "经线180°";
    //        }
    //        return $"西经{(int)((Map.Width / 2 - Pos.x) * 360f / Map.Width)}°";
    //    } else if (Pos.x > Map.Width / 2) {
    //        return $"东经{(int)((Pos.x - Map.Width / 2) * 360f / Map.Width)}°";
    //    } else {
    //        return "经线180°";
    //    }
    //}
    //private string Latitude() {
    //    if (Pos.y < Map.Width / 2) {
    //        if (Pos.x == 0) {
    //            return "极地90°";
    //        }
    //        return $"南纬{(int)((Map.Width / 2 - Pos.y) * 180f / Map.Width)}°";
    //    } else if (Pos.x > Map.Width / 2) {
    //        return $"北纬{(int)((Pos.y - Map.Width / 2) * 180f / Map.Width)}°";
    //    } else {
    //        return "极地90°";
    //    }
    //}


    public static class GeographyUtility
    {
        public static Type GetAltitudeType(int altitude) {
            if (altitude > 2500) {
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

