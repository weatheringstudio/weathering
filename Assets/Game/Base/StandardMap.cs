﻿

using System;
using UnityEngine;

namespace Weathering
{
    public abstract class StandardMap : IMapDefinition
    {
        public abstract int Width { get; }

        public abstract int Height { get; }

        public virtual void OnEnable() {
            if (Values == null) {
                int width = Width;
                int height = Height;
                Tiles = new ITileDefinition[width, height];
                Values = Weathering.Values.Create();
            }
            if (Refs == null) {
                Refs = Weathering.Refs.Create();
            }

            Vector2 cameraPos = Vector2.zero;
            cameraPos.x = Values.Get<CameraX>().Max / cameraFactor;
            cameraPos.y = Values.Get<CameraY>().Max / cameraFactor;
            MapView.Ins.CameraPosition = cameraPos;
        }
        private const float cameraFactor = 1024f;
        public void OnDisable() {
            Vector2 cameraPos = MapView.Ins.CameraPosition;
            Values.Get<CameraX>().Max = (long)(cameraPos.x * cameraFactor);
            Values.Get<CameraY>().Max = (long)(cameraPos.y * cameraFactor);
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


        /// <summary>
        /// 标准地图抽象类。功能：
        /// 1. 自动将width和height写入map.Values，便于保存
        /// 2. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
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
                    tile.OnEnable();
                    tile.OnConstruct();
                    return true;
                }
            }
            return false;
        }

        private void Validate(ref int i, ref int j) {
            int width = Width;
            int height = Height;
            i %= width;
            while (i < 0) i += width;
            j %= height;
            while (j < 0) j += height;
        }

        // modify
        public void SetTile(Vector2Int pos, ITileDefinition tile) {
            if (Tiles == null) {
                Tiles = new ITileDefinition[Width, Height];
            }
            Tiles[pos.x, pos.y] = tile;
        }
    }
}

