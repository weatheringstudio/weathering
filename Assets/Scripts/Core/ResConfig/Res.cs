
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weathering
{
    public interface IRes
    {
        Tile GetTile(string name);
        bool TryGetTile(string name, out Tile result); // 这种形式便于判断
        Sprite GetSprite(string name);
        Sprite TryGetSprite(string name); // 这种形式便于直接返回null
    }

    public class Res : MonoBehaviour, IRes
    {
        public static IRes Ins;

        private Dictionary<string, Sprite> staticSprites = new Dictionary<string, Sprite>();
        private Dictionary<string, Tile> staticTiles = new Dictionary<string, Tile>();
        public Tile GetTile(string name) {
            if (staticTiles.TryGetValue(name, out Tile result)) {
                return result;
            }
            throw new Exception("No Tile called: " + name + ".  Total: " + staticTiles.Count);
        }
        public bool TryGetTile(string name, out Tile result) {
            if (staticTiles.TryGetValue(name, out Tile result2)) {
                result = result2;
                return true;
            }
            result = null;
            return false;
        }

        public Sprite GetSprite(string name) {
            if (staticSprites.TryGetValue(name, out Sprite result)) {
                return result;
            }
            throw new Exception("No Sprite called: " + name);
        }
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
            }
        }

        private void ProcessObject(Transform trans) {
            TileResContainer staticTile = trans.GetComponent<TileResContainer>();
            if (staticTile != null) {
                foreach (var tile in staticTile.Tiles) {
                    staticTiles.Add(tile.name, tile);
                }
                if (staticTile.AlsoAsSprite) {
                    foreach (var tile in staticTile.Tiles) {
                        staticSprites.Add(tile.name, tile.sprite);
                    }
                }
            }
            SpriteResContainer staticSprite = trans.GetComponent<SpriteResContainer>();
            if (staticSprite != null) {
                foreach (var sprite in staticSprite.Sprites) {
                    staticSprites.Add(sprite.name, sprite);
                }
            }
        }
    }
}
