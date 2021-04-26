
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    //using ComponentData = Dictionary<string, string>;
    //using EntityData = Dictionary<string, Dictionary<string, string>>;
    //using MapData = Dictionary<string, Dictionary<string, Dictionary<string, string>>>;

    //public interface ISerializable
    //{
    //    void Serialize(ComponentData data);
    //    void Deserialize(ComponentData data);
    //}

    public static class TimeUtility
    {

        public static long GetTicks() {
            // will be replaced by in-game ticks
            return DateTime.Now.Ticks;
        }
        public static long GetMiniSeconds() {
            return GetTicks() / Value.MiniSecond;
        }
        public static long GetSeconds() {
            return GetTicks() / Value.Second;
        }

        public static double GetSecondsInDouble() {
            return (double)GetTicks() / Value.Second;
        }

        public static double GetMiniSecondsInDouble() {
            return (double)GetTicks() / Value.MiniSecond;
        }

        public static int GetFrame(float framerate, int spriteCount) {
            return (int)(GetTicks() / ((long)(Value.Second * framerate)) % spriteCount);
        }

        public static int GetSimpleFrame(float framerate, int spriteCount) {
            return (int)(Time.time / framerate) % spriteCount;
        }
    }
}

