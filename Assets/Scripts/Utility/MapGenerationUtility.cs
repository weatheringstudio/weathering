
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public static class MapGenerationUtility
    {
        public static void Randomize(int seed, int[,] map) {
            System.Random random = new System.Random(seed);

            int width = map.GetLength(0);
            int height = map.GetLength(1);
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    map[i, j] = random.Next(0, 2);
                }
            }
        }

        public static void CreateCellularMap(ref int[,] source, ref int[,] copy) {
            int width = source.GetLength(0);
            int height = source.GetLength(1);
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    int count = NeighborCount(i, j, source, width, height);
                    if (count < 4) {
                        copy[i, j] = 0;
                    }
                    else if (count > 4) {
                        copy[i, j] = 1;
                    }
                }
            }
            int[,] t = source;
            source = copy;
            copy = t;
        }

        private static int NeighborCount(int i, int j, int[,] map, int width, int height) {
            int result = 0;
            result += Get(CorrectPosition(new Vector2Int(i, j) + Vector2Int.left, width, height), map);
            result += Get(CorrectPosition(new Vector2Int(i, j) + Vector2Int.right, width, height), map);
            result += Get(CorrectPosition(new Vector2Int(i, j) + Vector2Int.up, width, height), map);
            result += Get(CorrectPosition(new Vector2Int(i, j) + Vector2Int.down, width, height), map);
            result += Get(CorrectPosition(new Vector2Int(i, j) + Vector2Int.left + Vector2Int.up, width, height), map);
            result += Get(CorrectPosition(new Vector2Int(i, j) + Vector2Int.left + Vector2Int.down, width, height), map);
            result += Get(CorrectPosition(new Vector2Int(i, j) + Vector2Int.right + Vector2Int.up, width, height), map);
            result += Get(CorrectPosition(new Vector2Int(i, j) + Vector2Int.right + Vector2Int.down, width, height), map);
            return result;
        }
        private static int Get(Vector2Int pos, int[,] map) {
            return map[pos.x, pos.y];
        }

        private static Vector2Int CorrectPosition(Vector2Int pos, int width, int height) {
            Vector2Int result = pos;
            if (result.x < 0) result.x += width;
            if (result.y < 0) result.y += height;
            if (result.x >= width) result.x -= width;
            if (result.y >= height) result.y -= height;
            return result;
        }
    }
}

