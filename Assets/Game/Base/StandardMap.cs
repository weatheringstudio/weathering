
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public abstract class StandardMap : IMapDefinition
	{
        public abstract int GetWidth();
        public abstract int GetHeight();

        public virtual void Initialize() {
            if (Values == null) {
                int width = GetWidth();
                int height = GetHeight();
                Tiles = new ITileDefinition[width, height];
                Values = Weathering.Values.Create();
                if (Values.Has<Width>() ) {
                    throw new Exception();
                }
                else {
                    Values.Get<Width>().Max = width;
                }
                if (Values.Has<Height>()) {
                    throw new Exception();
                }
                else {
                    Values.Get<Height>().Max = height;
                }
            }
        }

        public abstract void OnConstruct();

        public IValues Values { get; protected set; }
        public void SetValues(IValues values) => Values = values;

        public IRefs Refs { get; protected set; }
        public void SetRefs(IRefs refs) => Refs = refs;

        protected ITileDefinition[,] Tiles;

        public ITile Get(int i, int j) {
            Validate(ref i, ref j);
            ITile result = Tiles[i, j];
            return result;
        }

        public ITile Get(Vector2Int pos) {
            return Get(pos.x, pos.y);
        }

        public bool UpdateAt<T>(Vector2Int pos) where T : ITile {
            return UpdateAt<T>(pos.x, pos.y);
        }

        public bool UpdateAt<T>(int i, int j) where T : ITile {
            ITileDefinition tile = (Activator.CreateInstance<T>() as ITileDefinition);
            if (tile == null) throw new Exception();

            Validate(ref i, ref j);
            tile.Map = this;
            tile.Pos = new Vector2Int(i, j);
            if (tile.CanConstruct()) {
                ITileDefinition formerTile = Get(i, j) as ITileDefinition;
                if (formerTile == null || formerTile.CanDestruct()) {
                    if (formerTile != null) {
                        formerTile.OnDestruct();
                    }
                    Tiles[i, j] = tile;
                    tile.Initialize();
                    tile.OnConstruct();
                    return true;
                }
            }
            return false;
        }

        private void Validate(ref int i, ref int j) {
            int width = GetWidth();
            int height = GetHeight();
            i %= width;
            while (i < 0) i += width;
            j %= height;
            while (j < 0) j += height;
        }

        public void SetTile(Vector2Int pos, ITileDefinition tile) {
            if (Tiles == null) {
                Tiles = new ITileDefinition[GetWidth(), GetHeight()];
            }
            Tiles[pos.x, pos.y] = tile;
        }
    }
}

