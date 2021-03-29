
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
    [Depend(typeof(Temporature))] // a > 20
    public class TemporatureTropical { }
    [Concept]
    [Depend(typeof(Temporature))] // a > 0
    public class TemporatureTemporate { }
    [Concept]
    [Depend(typeof(Temporature))] // a > -20
    public class TemporatureCold { }
    [Concept]
    [Depend(typeof(Temporature))] // a <= -20
    public class TemporatureFreezing { }



    



    public static class GeographyUtility
    {
        public static Type GetAltitudeType(int altitude) {
            if (altitude > 2500) {
                return typeof(AltitudeMountain);
            } else if (altitude > 0) {
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
                return typeof(TemporatureTropical);
            } else if (temporature > 0) {
                return typeof(TemporatureTemporate);
            } else if (temporature > -20) {
                return typeof(TemporatureCold);
            } else {
                return typeof(TemporatureFreezing);
            }
        }
    }
}

