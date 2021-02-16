
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class ColorRainForest { }
    public class ColorGrassland { }
    public class ColorTundra { }
    public class ColorIceland { }
    public class TerrainDefault : StandardTile
    {
        private int temporature;
        private Type TerrainType;
        public override string SpriteKey {
            get {
                if (TerrainType == null) {
                    StandardMap standardMap = Map as StandardMap;
                    temporature = standardMap.Temporatures[Pos.x, Pos.y];
                    if (temporature > 20) {
                        TerrainType = typeof(ColorRainForest);
                    } else if (temporature > 0) {
                        TerrainType = typeof(ColorGrassland);
                    } else if (temporature > -10) {
                        TerrainType = typeof(ColorTundra);
                    } else {
                        TerrainType = typeof(ColorIceland);
                    }
                }
                return TerrainType.Name;
            }
        }

        public override void OnTap() {
            UI.Ins.ShowItems($"纬度{Mathf.Lerp(-90, 90, 1f * Pos.y / Map.Height)} 温度{temporature}", null, null);
        }
    }
}

