
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public abstract class StandardTile : ITileDefinition
    {
        public IValues Values { get; protected set; } = null;
        public void SetValues(IValues values) => Values = values;
        public IRefs Refs { get; protected set; } = null;
        public void SetRefs(IRefs refs) => Refs = refs;

        public IMap Map { get; set; }
        public Vector2Int Pos { get; set; }

        public uint HashCode { get; set; }


        public abstract string SpriteKey { get; }
        public virtual string SpriteOverlayKey { get => null; }
        public virtual string SpriteLeft { get => null; }
        public virtual string SpriteRight { get => null; }
        public virtual string SpriteUp { get => null; }
        public virtual string SpriteDown { get => null; }


        public virtual bool CanConstruct() => true;
        public virtual bool CanDestruct() => true;

        public virtual void OnEnable() {
            // TileName = Localization.Ins.Get(GetType());
        }
        // public string TileName { get; private set; }
        public virtual void OnConstruct() { 
        
        }
        public virtual void OnDestruct() { }
        public abstract void OnTap();
        public virtual void OnTapPlaySound() {
            Sound.Ins.PlayDefaultSound();
        }

        public IInventory Inventory { get; protected set; }



        public void SetInventory(IInventory inventory) => Inventory = inventory;

        public IMap GetMap() => Map;

        public Vector2Int GetPos() => Pos;
    }
}

