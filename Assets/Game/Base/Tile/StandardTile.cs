
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public abstract class StandardTile : ITileDefinition
    {
        public IValues Values { get; private set; } = null;
        public void SetValues(IValues values) => Values = values;
        public IMap Map { get; set; }
        public UnityEngine.Vector2Int Pos { get; set; }

        public abstract string SpriteKey {
            get;
        }


        public abstract bool CanConstruct();
        public abstract bool CanDestruct();
        public abstract void Initialize();

        public abstract void OnConstruct();

        public abstract void OnDestruct();

        public abstract void OnTap();
    }
}

