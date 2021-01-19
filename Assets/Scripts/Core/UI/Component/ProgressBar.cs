
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class ProgressBar : MonoBehaviour
    {
        public UnityEngine.UI.Slider Slider;

        public UnityEngine.UI.Image Background;
        public UnityEngine.UI.Button Button;

        public UnityEngine.UI.Image Foreground;
        public UnityEngine.UI.Text Text;
        public UnityEngine.UI.Image SliderRaycast;

        private Action onTap = null;
        public Action OnTap {
            set {
                onTap = value;
            }
        }
        public void Tap() {
            onTap?.Invoke();
        }

        private float velocity = 0;
        private float targetValue = 0;
        public void DampTo(float value) {
            targetValue = value;
            if (Mathf.Abs(value - Slider.value) > 0.5f) {
                SetTo(value);
            }
        }

        private void Awake() {
            SetTo(0);
        }
        public void SetTo(float value) {
            Slider.value = value;
            targetValue = value;
            velocity = 0;
        }
        public float Dampping = 0.05f;
        private void Update() {
            if (Slider.enabled && !Slider.interactable) {
                Slider.value = Mathf.SmoothDamp(Slider.value, targetValue, ref velocity, Dampping);
            }
        }
    }
}

