

using UnityEngine;

namespace Weathering
{
    public class RandomNames : MonoBehaviour
    {
        public static RandomNames Ins { get; private set; }

        public TextAsset Head;
        public TextAsset Tail;

        private void Awake() {
            if (Ins != null) throw new System.Exception();
            Ins = this;
        }
    }
}
