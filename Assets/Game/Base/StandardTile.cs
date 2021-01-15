
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

        public abstract string SpriteKey { get; }

        public abstract bool CanConstruct();
        public abstract bool CanDestruct();
        public abstract void OnEnable();
        public abstract void OnConstruct();
        public abstract void OnDestruct();
        public abstract void OnTap();
    }
}

