

using UnityEngine;

namespace Weathering
{
    public class SimpleHash : MonoBehaviour
    {
        public string Argument;
        [ContextMenu("生成hashcode")]
        public void GenerateHashCode() {
            if (Argument == null) throw new System.Exception();
            Debug.LogWarning(HashUtility.Hash(Argument));
        }
    }
}
