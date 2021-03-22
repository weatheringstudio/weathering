
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weathering
{
    public abstract class StandardTile : ITileDefinition
    {
        public bool NeedUpdateSpriteKeys { get; set; } = true;
        public int NeedUpdateSpriteKeysPositionX { get; set; }
        public int NeedUpdateSpriteKeysPositionY { get; set; }

        public IValues Values { get; protected set; } = null;
        public void SetValues(IValues values) => Values = values;
        public IRefs Refs { get; protected set; } = null;
        public void SetRefs(IRefs refs) => Refs = refs;
        public IInventory Inventory { get; protected set; }
        public void SetInventory(IInventory inventory) => Inventory = inventory;

        public IMap Map { get; set; }
        public Vector2Int Pos { get; set; }
        public IMap GetMap() => Map;
        public Vector2Int GetPos() => Pos;


        public uint HashCode { get; set; }

        public virtual string SpriteKeyBackground {
            get {
                StandardMap standardMap = Map as StandardMap;
                return standardMap.GetSpriteKeyBackground(HashCode);
            }
        }
        public virtual string SpriteKeyBase { get => null; }
        public virtual string SpriteKeyRoad { get => null; }
        public virtual string SpriteKey { get => null; }
        public virtual string SpriteKeyOverlay { get => null; }
        public virtual string SpriteLeft { get => null; }
        public virtual string SpriteRight { get => null; }
        public virtual string SpriteUp { get => null; }
        public virtual string SpriteDown { get => null; }

        public Tile TileSpriteKeyBackgroundBuffer { get; set; }
        public Tile TileSpriteKeyBaseBuffer { get; set; }
        public Tile TileSpriteKeyRoadBuffer { get; set; }
        public Tile TileSpriteKeyLeftBuffer { get; set; }
        public Tile TileSpriteKeyRightBuffer { get; set; }
        public Tile TileSpriteKeyUpBuffer { get; set; }
        public Tile TileSpriteKeyDownBuffer { get; set; }
        public Tile TileSpriteKeyBuffer { get; set; }
        public Tile TileSpriteKeyOverlayBuffer { get; set; }


        public virtual bool CanConstruct() => true;
        //public virtual bool CanDestruct() => true;

        public virtual void OnEnable() { }
        public virtual void OnConstruct() { }
        // public virtual void OnDestruct() { }
        public abstract void OnTap();
        public virtual void OnTapPlaySound() {
            Sound.Ins.PlayDefaultSound();
        }

        public uint GetTileHashCode() => HashCode;
    }
}

