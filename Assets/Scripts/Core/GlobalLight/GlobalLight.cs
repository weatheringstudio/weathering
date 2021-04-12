

using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Weathering
{
    /// <summary>
    /// 全局光管理
    /// 1. 全局光
    /// 2. 角色光
    /// </summary>
    public class GlobalLight : MonoBehaviour
    {
        public static GlobalLight Ins { get; private set; }

        [SerializeField]
        private Transform TheOnlyGlobalLightTransform;
        [SerializeField]
        private Light2D TheOnlyGlobalLight;

        [SerializeField]
        public void SyncCharacterLightPosition(Material mat) {
            Vector3 position = CharacterLightPosition;
            mat.SetFloat("_PlayerLightPosX", position.x);
            mat.SetFloat("_PlayerLightPosY", position.y);
        }

        [SerializeField]
        private Transform TheOnlyCharacterLightTransform;
        [SerializeField]
        private Light2D TheOnlyCharacterLight;

        public Quaternion GlobalLightRotation { 
            get => TheOnlyGlobalLightTransform.rotation; 
            set => TheOnlyGlobalLightTransform.rotation = value; 
        }

        public float GlobalLightIntensity { set => TheOnlyGlobalLight.intensity = value; }
        public Color GlobalLightColor { set => TheOnlyGlobalLight.color = value; }

        public Vector3 CharacterLightPosition { get => TheOnlyCharacterLightTransform.position; set => TheOnlyCharacterLightTransform.position = value; }

        public float CharacterLightIntensity { get => TheOnlyCharacterLight.intensity; set => TheOnlyCharacterLight.intensity = value; }

        private void Awake() {
            if (Ins != null) throw new System.Exception();
            Ins = this;
        }
    }
}
