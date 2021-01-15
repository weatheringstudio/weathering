
using System;
using UnityEngine;

namespace Weathering
{
    public class TestMap : StandardMap
    {
        public override int GetHeight() => 40;
        public override int GetWidth() => 40;

        public override void Initialize() {
            base.Initialize();
            if (Values == null) {
                Values = Weathering.Values.Create();
                Values.Get<Food>().Del = 10 * Value.Second;
                Values.Get<Labor>().Del = Value.Second;
                Values.Get<Width>().Max = GetWidth();
                Values.Get<Height>().Max = GetHeight();
            }
        }
        public override void OnConstruct() {
            int width = GetWidth();
            int height = GetHeight();
            int[,] source = new int[width, height];
            MapGenerationUtility.Randomize("TestMap".GetHashCode(), source);
            int[,] copy = new int[width, height];
            MapGenerationUtility.CreateCellularMap(ref source, ref copy);
            MapGenerationUtility.CreateCellularMap(ref source, ref copy);

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {

                    if (source[i, j] == 0) {
                        UpdateAt<Sea>(i, j);
                    } else {
                        const float period = 8f;
                        const float offset = 10000f;
                        float value = Mathf.PerlinNoise((i + Mathf.PI) / period, (j + Mathf.PI) / period);
                        float value2 = Mathf.PerlinNoise(offset + (i + Mathf.PI) / period, (j + Mathf.PI) / period);
                        if (value > 0.5f) {
                            if (value2 > 0.6f) {
                                UpdateAt<Mountain>(i, j);
                            } else {
                                UpdateAt<Forest>(i, j);
                            }
                        } else {
                            UpdateAt<Grassland>(i, j);
                        }
                    }
                }
            }
        }
    }
}

