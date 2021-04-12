
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Weathering
{
    public class CharacterView : MonoBehaviour
    {
        public Sprite DefaultSprite;

        //public Transform FlashLightTransform;

        //public Light2D FlashLight;

        public Sprite[] TestSprites;

        public CharacterView Ins { get; private set; }

        private SpriteRenderer sr;
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;

            sr = GetComponent<SpriteRenderer>();
            if (sr == null) throw new Exception();
        }

        private bool movingLast = false;
        private Vector2Int directionLast = Vector2Int.zero;

        private Vector3 lightVelocity = Vector3.zero;

        public float Distance = 2f;
        public void SetCharacterSprite(Vector2Int direction, bool moving) {

            bool needUpdateFlashLight = moving != movingLast || direction != directionLast;

            if (moving || needUpdateFlashLight) {
                int index;

                if (direction == Vector2Int.down) {
                    index = 0;
                } else if (direction == Vector2Int.left) {
                    index = 1;
                } else if (direction == Vector2Int.right) {
                    index = 2;
                } else if (direction == Vector2Int.up) {
                    index = 3;
                } else {
                    index = 0;
                }

                index *= 4;

                if (moving) index += TimeUtility.GetSimpleFrame(0.125f, 4);

                directionLast = direction;
                movingLast = moving;

                sr.sprite = TestSprites[index];
            }

        }
    }
}

