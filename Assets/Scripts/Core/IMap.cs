
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface ISavable
    {
        IValues Values { get; }
        IRefs Refs { get; }
        IInventory Inventory { get; }
    }

    public interface ISavableDefinition : ISavable
    {
        void SetValues(IValues values);
        void SetRefs(IRefs refs);
        void SetInventory(IInventory inventory);
    }

    public interface IMap : ISavable
    {
        int Width { get; }
        int Height { get; }
        bool ControlCharacter { get; }

        ITile Get(int i, int j);
        ITile Get(Vector2Int pos);
        T UpdateAt<T>(int i, int j) where T : class, ITile;
        ITile UpdateAt(Type type, int i, int j);
        T UpdateAt<T>(Vector2Int pos) where T : class, ITile;
        ITile UpdateAt(Type type, Vector2Int pos);
    }

    public interface IMapDefinition : IMap, ISavableDefinition
    {
        // 目前有两种方案定义DefaultTileType, 目前采用DefaultTileType够用
        Type GenerateTileType(Vector2Int pos);
        Type DefaultTileType { get; }

        void Update();
        int HashCode { get; }
        void SetTile(Vector2Int pos, ITileDefinition tile, bool inConstruction=false);
        void OnEnable();
        void OnDisable();
        void OnConstruct();

        void AfterGeneration();
    }
}

