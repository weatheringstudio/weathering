
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Weathering
//{
//    public class RandomnessGenerator : MonoBehaviour
//    {
//        public static RandomnessGenerator Ins { get; private set; }

//        private void Awake() {
//            if (Ins != null) throw new Exception();
//            Ins = this;

//            GameEntry.BeforeGameStart += Initialize;
//        }

//        public TextAsset RandomPlaceHead;
//        public TextAsset RandomPlaceTail;

//        public List<string> ReadConfigList(TextAsset text) {
//            List<string> result = new List<string>();
//            foreach (var line in text.text.Split()) {
//                if (line.Length == 0 || line.StartsWith("# ")) {
//                    continue;
//                }
//                result.Add(line);
//            }
//            return result;
//        }

//        private void Initialize() {
//            PlaceHead = ReadConfigList(RandomPlaceHead);
//            PlaceTail = ReadConfigList(RandomPlaceTail);
//        }

//        private List<string> PlaceHead;
//        private List<string> PlaceTail;

//        public string GetTileName(int i, int j, IMap map) {
//            unchecked {
//                int hashCode = (i.ToString() + j.ToString() + map.GetType().Name).GetHashCode();
//                hashCode = Hash(hashCode);
//                int a = Hash(hashCode + i);
//                int b = Hash(a + j);
//                int c = Hash(b) % 5;
//                string head = c < 3 ? PlaceHead[a % PlaceHead.Count] : PlaceTail[a % PlaceHead.Count];
//                string tail = PlaceTail[b % PlaceTail.Count];
//                if (head.Equals(tail)) {
//                    b = Hash(a + i + j) % PlaceTail.Count;
//                    tail = PlaceTail[b];
//                }
//                string result = c % 8 == 0 ? tail + head : head + tail;
//                // Debug.Log($"{result} - {hashCode} - ({i},{j}) - ({a},{b}");
//                return result;
//            }
//        }
//        private const int hashA = 378551;
//        private const int hashB = 63689;
//        private static int Hash(int x) {
//            x = x * hashA + hashB;
//            if (x < 0) x = -x;
//            return x;
//        }
//    }
//}

