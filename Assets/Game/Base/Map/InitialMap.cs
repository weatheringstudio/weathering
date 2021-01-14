
using System;
using UnityEngine;

namespace Weathering
{
    public class InitialMap : IMapDefinition {

        private const int width = 25;
        private const int height = 25;

        public void Initialize() {
            if (Values == null) {
                Values = Weathering.Values.Create();
                Values.Get<Food>().Del = 10 * Value.Second;
                Values.Get<Labor>().Del = Value.Second;
                Values.Get<Width>().Max = width;
                Values.Get<Height>().Max = height;
            }
            Tiles = new ITileDefinition[width, height];
        }
        public void OnConstruct() {
            int[,] source = new int[width, height];
            MapGenerationUtility.Randomize("litan".GetHashCode(), source);
            int[,] copy = new int[width, height];
            MapGenerationUtility.CreateCellularMap(ref source, ref copy);
            MapGenerationUtility.CreateCellularMap(ref source, ref copy);
            // MapGenerationUtility.CreateCellularMap(ref source, ref copy);

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

        public IValues Values { get; private set; }
        public void SetValues(IValues values) => Values = values;

        private ITileDefinition[,] Tiles;

        public ITile Get(int i, int j) {
            Validate(ref i, ref j);
            ITile result = Tiles[i, j];
            return result;
        }

        public ITile Get(Vector2Int pos) {
            return Get(pos.x, pos.y);
        }

        public bool UpdateAt<T>(Vector2Int pos) where T : ITile {
            return UpdateAt<T>(pos.x, pos.y);
        }

        public bool UpdateAt<T>(int i, int j) where T : ITile {
            ITileDefinition tile = (Activator.CreateInstance<T>() as ITileDefinition);
            if (tile == null) throw new Exception();

            Validate(ref i, ref j);
            tile.Map = this;
            tile.Pos = new Vector2Int(i, j);
            if (tile.CanConstruct()) {
                ITileDefinition formerTile = Get(i, j) as ITileDefinition;
                if (formerTile == null || formerTile.CanDestruct()) {
                    if (formerTile != null) {
                        formerTile.OnDestruct();
                    }
                    Tiles[i, j] = tile;
                    tile.Initialize();
                    tile.OnConstruct();
                    return true;
                }
            }
            return false;
        }

        private void Validate(ref int i, ref int j) {
            i %= width;
            while (i < 0) i += width;
            j %= height;
            while (j < 0) j += height;
        }

        public void SetTile(Vector2Int pos, ITileDefinition tile) {
            Tiles[pos.x, pos.y] = tile;
        }
    }
}

