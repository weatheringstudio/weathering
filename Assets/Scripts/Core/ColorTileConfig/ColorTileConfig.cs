
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public interface IColorTileConfig
    {
        Type Find(Color color);
    }

	public class ColorTileConfig : MonoBehaviour, IColorTileConfig
	{
        [Serializable]
        public struct Pair
        {
            public Color Color;
            public string Tile; 
        }
        public List<Pair> Pairs;

		public static IColorTileConfig Ins;

        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;

            if (Pairs.Count == 0) {
                throw new Exception();
            }
            foreach (var pair in Pairs) {
                Type type = Type.GetType(pair.Tile);
                if (type == null) throw new Exception(pair.Tile);
                if (type.IsAssignableFrom(typeof(ITileDefinition))) {
                    throw new Exception();
                }
            }
        }

        public Type Find(Color color) {
            string resultString = null;
            float distanceSquaredRecorded = float.MaxValue;
            foreach (var pair in Pairs) {
                float distanceSquared = DistanceSquaredOfColor(color - pair.Color);
                if (distanceSquared < distanceSquaredRecorded) {
                    resultString = pair.Tile;
                    distanceSquaredRecorded = distanceSquared;
                }
            }
            return Type.GetType(resultString);
        }

        private float DistanceSquaredOfColor(Color color) {
            return color.r * color.r + color.g * color.g + color.b * color.b;
        }
    }
}

