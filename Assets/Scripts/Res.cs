
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weathering
{
    public interface IRes
    {
        Tile GetTile(string name);
    }

    public class Res : MonoBehaviour, IRes
    {
        public static IRes Ins;

        private Dictionary<string, Tile> staticTiles = new Dictionary<string, Tile>();
        public Tile GetTile(string name) {
            if (staticTiles.TryGetValue(name, out Tile result)) {
                return result;
            }
            throw new Exception("No Tile called: " + name + ".  Total: " + staticTiles.Count);
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
            }
        }
    }
}
