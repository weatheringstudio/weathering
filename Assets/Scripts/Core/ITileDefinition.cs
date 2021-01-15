
using System;
using System.Collections.Generic;

namespace Weathering
{
    public interface ITile {
        IValues Values { get; }
        // IRefs Refs { get; }
        void OnTap();
        bool CanConstruct();
        bool CanDestruct();
    }

    public interface ITileDefinition : ITile
    {
        void SetValues(IValues values);
        // void SetRefs(IRefs refs);

        IMap Map { get; set; }
        UnityEngine.Vector2Int Pos { get; set; }

        void OnConstruct();
        void OnDestruct();

        void Initialize();

        string SpriteKey { get; }
    }

}








