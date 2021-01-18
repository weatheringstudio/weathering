

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public abstract class StandardMap : IMapDefinition
    {
        //// "脏"数据保存，只需要保存有变化的数据。边长20
        //public const int DirtyRange = 20;
        //public bool AllDirty = false;
        //private List<Vector2Int> dirtyParts = new List<Vector2Int>(16);
        //public void SetDirty(int i, int j) {
        //    int dirtyI = i % DirtyRange;
        //    int dirtyJ = j % DirtyRange;
        //    Vector2Int dirtyPos = new Vector2Int(dirtyI, dirtyJ);
        //    if (dirtyParts.Contains(dirtyPos)) {

        //    }
        //    else {

        //    }
        //}
        //public void ClearDirty() {
        //    dirtyParts.Clear();
        //}

        public abstract int Width { get; }

        public abstract int Height { get; }

        public virtual void OnEnable() {
            if (Values == null) {
                Tiles = new ITileDefinition[Width, Height];
                Values = Weathering.Values.Create();
            }
            if (Refs == null) {
                Refs = Weathering.Refs.Create();
            }

            //Vector2 cameraPos = Vector2.zero;
            //cameraPos.x = Values.Get<CameraX>().Max / cameraFactor;
            //cameraPos.y = Values.Get<CameraY>().Max / cameraFactor;
            //MapView.Ins.CameraPosition = cameraPos;
        }
        //private const float cameraFactor = 1024f;
        public void OnDisable() {
            //Vector2 cameraPos = MapView.Ins.CameraPosition;
            //Values.Get<CameraX>().Max = (long)(cameraPos.x * cameraFactor);
            //Values.Get<CameraY>().Max = (long)(cameraPos.y * cameraFactor);
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
        /// 2. 获取地块位置时，矫正i和j，循环
        /// 3. 建立和替换地块时，自动检查调用旧地块CanDestruct和OnDestruct，调用新地块CanConstruct，OnEnable和OnConstruct
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
            i %= Width;
            while (i < 0) i += Width;
            j %= Height;
            while (j < 0) j += Height;
        }

        // modify
        public void SetTile(UnityEngine.Vector2Int pos, ITileDefinition tile) {
            if (Tiles == null) {
                Tiles = new ITileDefinition[Width, Height];
            }
            Tiles[pos.x, pos.y] = tile;
        }
    }
}

