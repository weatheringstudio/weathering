

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
        public static string Decorated(string name) => $"{name}_Working";

        public static GlobalLight Ins { get; private set; }

        [SerializeField]
        private Transform TheOnlyGlobalLightTransform;
        [SerializeField]
        private Light2D TheOnlyGlobalLight;

        [SerializeField]
        private Transform TheOnlyStarLightTransform;
        [SerializeField]
        private Light2D TheOnlyStarLight;


        public bool UseDayNightCycle { set {
                TheOnlyStarLight.enabled = value;
                TheOnlyGlobalLight.enabled = !value;
            } 
        }
        public bool UseCharacterLight {
            set {
                TheOnlyCharacterLight.enabled = value;
            }
        }

        public long SecondsForADay { get; set; }




        [SerializeField]
        public void SyncCharacterLightPosition(Material mat) {
            if (GameMenu.LightEnabled) {
                Vector3 position = TheOnlyCharacterLightTransform.position;
                mat.SetFloat("_PlayerLightPosX", position.x);
                mat.SetFloat("_PlayerLightPosY", position.y);
                mat.SetFloat("PlayerLightAlpha", 1);
            }
            else {
                mat.SetFloat("PlayerLightAlpha", 0);
            }

        }

        [SerializeField]
        private Transform TheOnlyCharacterLightTransform;
        [SerializeField]
        private Light2D TheOnlyCharacterLight;

        public Quaternion StarLightRotation {
            get => TheOnlyStarLightTransform.rotation;
            set => TheOnlyStarLightTransform.rotation = value;
        }

        public float StarLightIntensity { set => TheOnlyStarLight.intensity = value; }
        public Color StarLightColor { set => TheOnlyStarLight.color = value; }

        public Vector3 StarLightPosition { set => TheOnlyStarLightTransform.position = value; }

        public Vector3 CharacterLightPosition { get => TheOnlyCharacterLightTransform.position; set => TheOnlyCharacterLightTransform.position = value; }

        public float CharacterLightIntensity { get => TheOnlyCharacterLight.intensity; set => TheOnlyCharacterLight.intensity = value; }

        private void Awake() {
            if (Ins != null) throw new System.Exception();
            Ins = this;
        }

    }
}
