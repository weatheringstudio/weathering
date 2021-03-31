
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public static class EaseFuncUtility
    {
        public static float ShrinkOnHalf(float x, float dx) {
            if (x < dx) return 0;
            else if (x > 1 - dx) return 1;
            else {
                return (x - 0.5f) / (1 - 2 * dx) + 0.5f;
            }
        }

        public static float Linear(float x) {
            return x;
        }
        public static float EaseIn(float x) {
            return 1 + Mathf.Sin((x - 1) * Mathf.PI / 2);
        }
        public static float EaseOut(float x) {
            return Mathf.Sin(x * Mathf.PI / 2);
        }
        public static float EaseInCubic(float x) {
            return x * x;
        }

        public static float EaseInOutCubic(float x) {
            return x < 0.5f ? 2 * x * x : (-2 * x + 4) * x - 1;
        }

        public static float EaseInOutBack(float x) {
            return x * (x * x - 0.3f * Mathf.Sin(x * Mathf.PI));
        }
    }
}

