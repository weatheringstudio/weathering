
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public enum AltitudeType
    {
        Sea, Plain, Mountain
    }

    public enum MoistureType
    {
        Forest, Grassland, Desert
    }

    public enum TemporatureType
    {
        Tropical, Temporate, Cold, Freezing
    }

    public static class GeographyUtility
    {
        public static AltitudeType GetAltitudeType(int altitude) {
            if (altitude > 3000) {
                return AltitudeType.Mountain;
            } else if (altitude > 0) {
                return AltitudeType.Plain;
            } else {
                return AltitudeType.Sea;
            }
        }

        public static MoistureType GetMoistureType(int moisture) {
            if (moisture > 55) {
                return MoistureType.Forest;
            } else if (moisture > 35) {
                return MoistureType.Grassland;
            } else {
                return MoistureType.Desert;
            }
        }

        public static TemporatureType GetTemporatureType(int temporature) {
            if (temporature > 20) {
                return TemporatureType.Tropical;
            } else if (temporature > 0) {
                return TemporatureType.Temporate;
            } else if (temporature > -20) {
                return TemporatureType.Cold;
            } else {
                return TemporatureType.Freezing;
            }
        }
    }
}

