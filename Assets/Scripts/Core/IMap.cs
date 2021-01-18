
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public interface IMap
    {
        int Width { get; }

        int Height { get; }

        ITile Get(int i, int j);
        ITile Get(Vector2Int pos);
        bool UpdateAt<T>(int i, int j) where T : ITile;
        bool UpdateAt<T>(Vector2Int pos) where T : ITile;

        Type Generate(Vector2Int pos);

        IValues Values { get; }

        IRefs Refs { get; }
    }

    public interface IMapDefinition : IMap
    {
        void OnEnable();
        // void OnDisable();
        void OnConstruct();
        void SetValues(IValues values);
        void SetRefs(IRefs refs);
        void SetTile(Vector2Int pos, ITileDefinition tile);
    }
}

