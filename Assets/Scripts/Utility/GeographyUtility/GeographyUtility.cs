
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public enum AltitudeType { 
		DeepSea, Sea, Plain, Plateau, MountainPeak
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
                return AltitudeType.MountainPeak;
            } else if (altitude > 1500) {
                return AltitudeType.Plateau;
            } else if (altitude > 0) {
                return AltitudeType.Plain;
            } else if (altitude > -1000) {
                return AltitudeType.Sea;
            } else {
                return AltitudeType.DeepSea;
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

