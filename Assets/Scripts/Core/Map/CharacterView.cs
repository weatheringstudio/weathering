
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public class CharacterView : MonoBehaviour
	{
        public Sprite DefaultSprite;

        public Sprite[] TestSprites;

		public CharacterView Ins { get; private set; }

        private SpriteRenderer sr;
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;

            sr = GetComponent<SpriteRenderer>();
            if (sr == null) throw new Exception();
        }

        public void SetCharacterSprite(Vector2Int direction, bool moving) {
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
            if (moving) {
                index += TimeUtility.GetSimpleFrame(0.125f, 4);
            }
            sr.sprite = TestSprites[index];
        }
    }
}

