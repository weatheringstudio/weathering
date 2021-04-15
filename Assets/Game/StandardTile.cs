
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weathering
{
    /// <summary>
    /// StandardTile功能
    /// 1. ISavable, Values, Refs, Inventory
    /// 2. Map, Pos, HashCode
    /// 3. SpriteKey
    /// 4. Construct, Destruct, Enable
    /// </summary>
    public abstract class StandardTile : ITileDefinition
    {
        public bool NeedUpdateSpriteKeys { get; set; } = true;
        public int NeedUpdateSpriteKeysPositionX { get; set; }
        public int NeedUpdateSpriteKeysPositionY { get; set; }

        public IValues Values { get; protected set; } = null;
        public void SetValues(IValues values) => Values = values;
        public IRefs Refs { get; set; } = null;
        public void SetRefs(IRefs refs) => Refs = refs;
        public IInventory Inventory { get; protected set; }
        public void SetInventory(IInventory inventory) => Inventory = inventory;
        public IInventory InventoryOfSupply { get; protected set; }
        public void SetInventoryOfSupply(IInventory inventory) => InventoryOfSupply = inventory;


        public IMap Map { get; set; }
        public Vector2Int Pos { get; set; }
        public IMap GetMap() => Map;
        public Vector2Int GetPos() => Pos;
        public uint TileHashCode { get; set; }
        public uint GetTileHashCode() => TileHashCode;


        /// <summary>
        /// SpriteKeyBackground和SpriteKeyBase都是Map定义的
        /// </summary>
        protected virtual bool PreserveLandscape => true;
        public virtual string SpriteKeyBedrock => Map.GetSpriteKeyBedrock(Pos);
        public virtual string SpriteKeyWater => Map.GetSpriteKeyWater(Pos);
        public virtual string SpriteKeyGrass => PreserveLandscape ? Map.GetSpriteKeyGrass(Pos) : null;
        public virtual string SpriteKeyTree => PreserveLandscape ? Map.GetSpriteKeyTree(Pos) : null;
        public virtual string SpriteKeyHill => PreserveLandscape ? Map.GetSpriteKeyHill(Pos) : null;

        public virtual string SpriteKeyRoad { get => null; }
        public virtual string SpriteKey { get => null; } // 
        public virtual string SpriteKeyOverlay { get => null; } // 用于指示标记

        public virtual string SpriteLeft { get => null; }
        public virtual string SpriteRight { get => null; }
        public virtual string SpriteUp { get => null; }
        public virtual string SpriteDown { get => null; }

        public Tile TileSpriteKeyBedrockBuffer { get; set; }
        public Tile TileSpriteKeyWaterBuffer { get; set; }
        public Tile TileSpriteKeyGrassBuffer { get; set; }
        public Tile TileSpriteKeyTreeBuffer { get; set; }
        public Tile TileSpriteKeyHillBuffer { get; set; }
        public Tile TileSpriteKeyRoadBuffer { get; set; }
        public Tile TileSpriteKeyLeftBuffer { get; set; }
        public Tile TileSpriteKeyRightBuffer { get; set; }
        public Tile TileSpriteKeyUpBuffer { get; set; }
        public Tile TileSpriteKeyDownBuffer { get; set; }
        public Tile TileSpriteKeyBuffer { get; set; }
        public Tile TileSpriteKeyOverlayBuffer { get; set; }


        public virtual bool CanConstruct() => true;
        public virtual bool CanDestruct() => false;

        public virtual void OnEnable() { }
        public virtual void OnConstruct(ITile oldTile) { }
        public virtual void OnDestruct(ITile newTile) { }
        public abstract void OnTap();
        public virtual void OnTapPlaySound() {
            Sound.Ins.PlayDefaultSound();
        }

    }
}

