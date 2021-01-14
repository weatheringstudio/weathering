
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public interface IMap
    {
        ITile Get(int i, int j);
        ITile Get(Vector2Int pos);
        bool UpdateAt<T>(int i, int j) where T : ITile;
        bool UpdateAt<T>(Vector2Int pos) where T : ITile;

        IValues Values { get; }
    }

    public interface IMapDefinition : IMap
    {
        void Initialize();
        void SetValues(IValues values);
        void SetTile(Vector2Int pos, ITileDefinition tile);
    }
}

