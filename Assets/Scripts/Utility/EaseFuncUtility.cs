
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public static class EaseFuncUtility
    {
        public static float EaseIn(float x) {
            return 1 + Mathf.Sin((x - 1) * Mathf.PI / 2);
        }
        public static float EaseOut(float x) {
            return Mathf.Sin(x * Mathf.PI / 2);
        }
        public static float EaseInCubic(float x) {
            return x * x;
        }

        public static float EaseInOutBack(float x) {
            return x * (x * x - 0.3f * Mathf.Sin(x * Mathf.PI));
        }
    }
}

