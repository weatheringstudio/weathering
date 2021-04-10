
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CharacterView : MonoBehaviour
    {
        public Sprite DefaultSprite;

        public Transform FlashLight;

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

        public void SetCharacterSprite(Vector2Int direction, bool moving) {

            bool needUpdateFlashLight = moving != movingLast || direction != directionLast;

            if (moving || needUpdateFlashLight) {
                int index;

                if (direction == Vector2Int.down) {
                    if (needUpdateFlashLight) FlashLight.rotation = Quaternion.Euler(0, 0, 180);
                    index = 0;
                } else if (direction == Vector2Int.left) {
                    if (needUpdateFlashLight) FlashLight.rotation = Quaternion.Euler(0, 0, 90);
                    index = 1;
                } else if (direction == Vector2Int.right) {
                    if (needUpdateFlashLight) FlashLight.rotation = Quaternion.Euler(0, 0, 270);
                    index = 2;
                } else if (direction == Vector2Int.up) {
                    if (needUpdateFlashLight) FlashLight.rotation = Quaternion.Euler(0, 0, 0);
                    index = 3;
                } else {
                    index = 0;
                }

                if (needUpdateFlashLight) {
                    Vector3 position = ((Vector2)direction) * 0.2f;
                    FlashLight.localPosition = position;
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

