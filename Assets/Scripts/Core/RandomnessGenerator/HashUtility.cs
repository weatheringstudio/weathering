
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public static class HashUtility
    {

        public static uint Hash(string a) {
            uint result = 7;
            foreach (char c in a) {
                result += c;
                result = Hash(result);
            }
            return result;
        }

        /// 4-byte Integer Hashing

        /// The hashes on this page (with the possible exception of HashMap.java's) are all public domain. 
        /// So are the ones on Thomas Wang's page. Thomas recommends citing the author and page when using them.
        /// http://www.burtleburtle.net/bob/hash/integer.html

        /// Thomas Wang has an integer hash using multiplication that's faster than any of mine on my Core 2 duo using gcc -O3, 
        /// and it passes my favorite sanity tests well. 
        /// I've had reports it doesn't do well with integer sequences with a multiple of 34.
        public static uint Hash(uint a) {
            a = (a ^ 61) ^ (a >> 16);
            a = a + (a << 3);
            a = a ^ (a >> 4);
            a = a * 0x27d4eb2d;
            a = a ^ (a >> 15);
            return a;
        }
        public static uint Hashed(ref uint x) {
            x = HashUtility.Hash(x);
            return x;
        }
        public static uint AddSalt(uint a, uint salt) {
            return Hash(a + salt);
        }

        private const uint hashDivider = uint.MaxValue / 1024;
        public static float ToFloat(uint hash) {
            return (float)(hash / 1024) / hashDivider;
        }

        public static Vector2 RandomVec2(int i, int j, int width, int height) {
            if (i < 0) i += width;
            if (j < 0) j += height;
            if (i > width) j -= width;
            if (j > height) j -= height;
            return new Vector2(
                ToFloat(Hash(i, j, width, height)),
                ToFloat(Hash(i, j, width, height, width * height))
            );
        }

        public static Vector2 RandomVec2Simple(int i, int j, int width, int height, int offset = 0) {
            i = i % width;
            if (i < 0) i += width;
            j = j % height;
            if (j < 0) j += height;
            uint hash = Hash(i, j, width, height, offset);
            switch (hash % 4) {
                case 0:
                    return Vector2.left + Vector2.up;
                case 1:
                    return Vector2.left + Vector2.down;
                case 2:
                    return Vector2.right + Vector2.up;
                case 3:
                    return Vector2.right + Vector2.down;
                default:
                    throw new Exception();
            }
        }

        public static uint Hash(int i, int j, int width, int height, int offset = 0) {
            return unchecked(Hash((uint)(offset * width + height + i + j * width)));
        }

        public static uint Hash(Vector2Int pos, Vector2Int size) {
            return Hash((uint)(pos.x + pos.y + size.x));
        }


        public static float SimpleValueNoise(float x) {
            float x0 = (float)(Hash((uint)(x))) / uint.MaxValue;
            float x1 = (float)(Hash((uint)(x + 1))) / uint.MaxValue;
            float result = Mathf.Lerp(x0, x1, EaseFuncUtility.EaseInOutCubic(x - (uint)x));
            return result;
        }
        public static double SimpleValueNoise(double x) {
            double x0 = (double)(Hash((uint)(x))) / uint.MaxValue;
            double x1 = (double)(Hash((uint)(x + 1))) / uint.MaxValue;
            double t = EaseFuncUtility.EaseInOutCubic(x - (uint)x);
            double result = x0 + (x1 - x0) * t;
            // Debug.LogWarning($"{x} @ {x0} @ {x1} @ {t} @ {result}");
            return result;
        }


        public static float PerlinNoise(float x, float y, int width, int height, int layer = 0) {
            int p0x = (int)(x); // P0坐标
            int p0y = (int)(y);
            int p1x = p0x;      // P1坐标
            int p1y = p0y + 1;
            int p2x = p0x + 1;  // P2坐标
            int p2y = p0y + 1;
            int p3x = p0x + 1;  // P3坐标
            int p3y = p0y;

            Vector2 g0 = RandomVec2Simple(p0x, p0y, width, height, layer);
            float g0x = g0.x;  // P0点的梯度
            float g0y = g0.y;
            Vector2 g1 = RandomVec2Simple(p1x, p1y, width, height, layer);
            float g1x = g1.x;  // P1点的梯度
            float g1y = g1.y;
            Vector2 g2 = RandomVec2Simple(p2x, p2y, width, height, layer);
            float g2x = g2.x;  // P2点的梯度
            float g2y = g2.y;
            Vector2 g3 = RandomVec2Simple(p3x, p3y, width, height, layer);
            float g3x = g3.x;  // P3点的梯度
            float g3y = g3.y;

            float v0x = x - p0x;  // P0点的方向向量
            float v0y = y - p0y;
            float v1x = x - p1x;  // P1点的方向向量
            float v1y = y - p1y;
            float v2x = x - p2x;  // P2点的方向向量
            float v2y = y - p2y;
            float v3x = x - p3x;  // P3点的方向向量
            float v3y = y - p3y;

            float product0 = g0x * v0x + g0y * v0y;  // P0点梯度向量和方向向量的点乘
            float product1 = g1x * v1x + g1y * v1y;  // P1点梯度向量和方向向量的点乘
            float product2 = g2x * v2x + g2y * v2y;  // P2点梯度向量和方向向量的点乘
            float product3 = g3x * v3x + g3y * v3y;  // P3点梯度向量和方向向量的点乘

            // P1和P2的插值
            float d0 = x - p0x;
            d0 = d0 * d0 * d0 * (d0 * (d0 * 6 - 15) + 10);

            float n0 = product1 * (1.0f - d0) + product2 * d0;
            // P0和P3的插值
            float n1 = product0 * (1.0f - d0) + product3 * d0;

            // P点的插值
            float d1 = y - p0y;
            d1 = d1 * d1 * d1 * (d1 * (d1 * 6 - 15) + 10);

            float result = n1 * (1.0f - d1) + n0 * d1;
            return result;
        }




        /// <summary>
        /// Thomas has 64-bit integer hashes too. I don't have any of those yet.
        /// Here's a way to do it in 6 shifts:
        /// </summary>
        public static uint Hash0(uint a) {
            a = (a + 0x7ed55d16) + (a << 12);
            a = (a ^ 0xc761c23c) ^ (a >> 19);
            a = (a + 0x165667b1) + (a << 5);
            a = (a + 0xd3a2646c) ^ (a << 9);
            a = (a + 0xfd7046c5) + (a << 3);
            a = (a ^ 0xb55a4f09) ^ (a >> 16);
            return a;
        }

        /// <summary>
        /// Or 7 shifts, if you don't like adding those big magic constants:
        /// </summary>
        public static uint Hash1(uint a) {
            a -= (a << 6);
            a ^= (a >> 17);
            a -= (a << 9);
            a ^= (a << 4);
            a -= (a << 3);
            a ^= (a << 10);
            a ^= (a >> 15);
            return a;
        }

        /// <summary>
        /// Thomas Wang has a function that does it in 6 shifts (provided you use the low bits, hash & (SIZE-1), 
        /// rather than the high bits if you can't use the whole value):
        /// </summary>
        public static uint Hash2(uint a) {
            a += ~(a << 15);
            a ^= (a >> 10);
            a += (a << 3);
            a ^= (a >> 6);
            a += ~(a << 11);
            a ^= (a >> 16);
            return a;
        }

        /// <summary>
        /// Here's a 5-shift one where you have to use the high bits, hash >> (32-logSize), because the low bits are hardly mixed at all:
        /// </summary>
        public static uint Hash3(uint a) {
            a = (a + 0x479ab41d) + (a << 8);
            a = (a ^ 0xe4aa10ce) ^ (a >> 5);
            a = (a + 0x9942f0a6) - (a << 14);
            a = (a ^ 0x5aedd67d) ^ (a >> 3);
            a = (a + 0x17bea992) + (a << 7);
            return a;
        }

        /// <summary>
        /// Here's one that takes 4 shifts. You need to use the bottom bits, and you need to use at least the bottom 11 bits. 
        /// It doesn't achieve avalanche at the high or the low end. It does pass my integer sequences tests, 
        /// and all settings of any set of 4 bits usually maps to 16 distinct values in bottom 11 bits.
        /// </summary>
        public static uint Hash4(uint a) {
            a = (a ^ 0xdeadbeef) + (a << 4);
            a = a ^ (a >> 10);
            a = a + (a << 7);
            a = a ^ (a >> 13);
            return a;
        }

        /// <summary>
        /// And this one isn't too bad, provided you promise to use at least the 17 lowest bits. Passes the integer sequence and 4-bit tests.
        /// </summary>
        public static uint Hash5(uint a) {
            a = a ^ (a >> 4);
            a = (a ^ 0xdeadbeef) + (a << 5);
            a = a ^ (a >> 11);
            return a;
        }

        public static uint Hash6(uint h) {
            // This function ensures that hashCodes that differ only by
            // constant multiples at each bit position have a bounded
            // number of collisions (approximately 8 at default load factor).
            h ^= (h >> 20) ^ (h >> 12);
            return h ^ (h >> 7) ^ (h >> 4);
        }
    }
}

