

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CameraX { }
    public class CameraY { }

    public class ClearColorR { }
    public class ClearColorG { }
    public class ClearColorB { }


    public abstract class StandardMap : IMapDefinition
    {
        public abstract int Width { get; }

        public abstract int Height { get; }

        public virtual void OnEnable() {
            if (Values == null) {
                Tiles = new ITileDefinition[Width, Height];
                Values = Weathering.Values.GetOne();
            }
            if (Refs == null) {
                Refs = Weathering.Refs.GetOne();
            }
            if (Inventory == null) {
                Inventory = Weathering.Inventory.GetOne();
            }
            Vector2 cameraPos = Vector2.zero;
            cameraPos.x = Values.GetOrCreate<CameraX>().Max / factor;
            cameraPos.y = Values.GetOrCreate<CameraY>().Max / factor;
            MapView.Ins.CameraPosition = cameraPos;

            Color color = Color.black;
            color.r = Values.GetOrCreate<ClearColorR>().Max / factor;
            color.g = Values.GetOrCreate<ClearColorG>().Max / factor;
            color.b = Values.GetOrCreate<ClearColorB>().Max / factor;
            MapView.Ins.ClearColor = color;
        }
        private const float factor = 1024f;
        public void OnDisable() {
            Vector2 cameraPos = MapView.Ins.CameraPosition;
            Values.Get<CameraX>().Max = (long)(cameraPos.x * factor);
            Values.Get<CameraY>().Max = (long)(cameraPos.y * factor);

            Color clearColor = MapView.Ins.ClearColor;
            Values.Get<ClearColorR>().Max = (long)(clearColor.r * factor);
            Values.Get<ClearColorG>().Max = (long)(clearColor.g * factor);
            Values.Get<ClearColorB>().Max = (long)(clearColor.b * factor);
        }

        public abstract void OnConstruct();
        public IValues Values { get; protected set; }
        public void SetValues(IValues values) => Values = values;
        public IRefs Refs { get; protected set; }
        //public void SetRefs(IRefs refs) {
        //    Refs = refs;
        //}
        public void SetRefs(IRefs refs) => Refs = refs;
        public IInventory Inventory { get; protected set; }
        public void SetInventory(IInventory inventory) => Inventory = inventory;

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
        public void SetTile(Vector2Int pos, ITileDefinition tile) {
            if (Tiles == null) {
                Tiles = new ITileDefinition[Width, Height];
            }
            Tiles[pos.x, pos.y] = tile;
        }

        public abstract Type Generate(Vector2Int pos);
        public virtual void AfterGeneration() { }
    }
}

