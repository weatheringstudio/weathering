

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
                if (whiteBalance == null && Profile.TryGet<UnityEngine.Rendering.Universal.WhiteBalance>(out whiteBalance)) {
                }
                if (whiteBalance == null) {
                    throw new System.Exception();
                }
                return whiteBalance;

            }
        }

        private void Awake() {
            if (Ins != null) throw new System.Exception();
            Ins = this;

            
        }
    }
}
