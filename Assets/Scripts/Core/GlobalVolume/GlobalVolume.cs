

using UnityEngine;

namespace Weathering
{
    public class GlobalVolume : MonoBehaviour
    {
        public static GlobalVolume Ins { get; private set; }

        [SerializeField]
        private UnityEngine.Rendering.Volume volume;
        public UnityEngine.Rendering.Volume Volume { get => volume; }

        public UnityEngine.Rendering.VolumeProfile Profile { get => volume.sharedProfile; }


        private UnityEngine.Rendering.Universal.WhiteBalance whiteBalance = null;
        public UnityEngine.Rendering.Universal.WhiteBalance WhiteBalance {
            get {
                if (whiteBalance == null && Profile.TryGet(out whiteBalance)) {
                }
                if (whiteBalance == null) {
                    throw new System.Exception();
                }
                return whiteBalance;
            }
        }

        private UnityEngine.Rendering.Universal.Tonemapping tonemapping = null;
        public UnityEngine.Rendering.Universal.Tonemapping Tonemapping {
            get {
                if (tonemapping == null && Profile.TryGet(out tonemapping)) {
                }
                if (tonemapping == null) {
                    throw new System.Exception();
                }
                return tonemapping;
            }
        }

        private void Awake() {
            if (Ins != null) throw new System.Exception();
            Ins = this;
        }
    }
}
