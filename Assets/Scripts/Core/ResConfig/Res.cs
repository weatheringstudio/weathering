
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weathering
{
    public interface IRes
    {
        // Tile GetTile(string name);
        bool TryGetTile(string name, out Tile result); // 这种形式便于判断
        // Sprite GetSprite(string name);
        Sprite TryGetSprite(string name); // 这种形式便于直接返回null
    }

    public class Res : MonoBehaviour, IRes
    {
        public static IRes Ins;

        [SerializeField]
        private Tile EmptyTilePrefab;

        private Dictionary<string, Sprite> staticSprites = new Dictionary<string, Sprite>();
        private Dictionary<string, Tile> staticTiles = new Dictionary<string, Tile>();
        //public Tile GetTile(string name) {
        //    if (staticTiles.TryGetValue(name, out Tile result)) {
        //        return result;
        //    }
        //    throw new Exception("No Tile called: " + name + ".  Total: " + staticTiles.Count);
        //}
        public bool TryGetTile(string name, out Tile result) {
            if (staticTiles.TryGetValue(name, out Tile result2)) {
                result = result2;
                return true;
            } else if (staticSprites.TryGetValue(name, out Sprite result3)) {
                Tile tile = Instantiate(EmptyTilePrefab);
                tile.sprite = result3;
                result = tile;

                staticTiles.Add(name, tile);
                return true;
            }
            result = null;
            return false;
        }

        //public Sprite GetSprite(string name) {
        //    if (staticSprites.TryGetValue(name, out Sprite result)) {
        //        return result;
        //    }
        //    throw new Exception("No Sprite called: " + name);
        //}
        public Sprite TryGetSprite(string name) {
            if (staticSprites.TryGetValue(name, out Sprite result)) {
                return result;
            }
            return null;
        }

        private void Awake() {
            if (Ins != null) throw new System.Exception();
            Ins = this;
            foreach (Transform item in transform) {
                ProcessObject(item);
                foreach (Transform item2 in item) {
                    ProcessObject(item2);
                    foreach (Transform item3 in item2) {
                        ProcessObject(item3);
                    }
                }
            }
        }

        private void ProcessObject(Transform trans) {
            SpriteResContainer spriteContainer = trans.GetComponent<SpriteResContainer>();
            if (spriteContainer != null) {
                if (spriteContainer.Sprites == null) {
                    throw new Exception($"{spriteContainer.name} 没用配置内容");
                }
                foreach (var sprite in spriteContainer.Sprites) {
                    staticSprites.Add(sprite.name, sprite);
                }
            }
        }
    }
}
