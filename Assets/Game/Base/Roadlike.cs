
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    #region road
    public interface IRoadLike
    {
        bool IsRoadLike();
    }

    public interface IRoadUpDepender { }
    public interface IRoadDownDepender { }
    public interface IRoadLeftDepender { }
    public interface IRoadRightDepender { }

    public interface IRoadUpDependee { }
    public interface IRoadDownDependee { }
    public interface IRoadLeftDependee { }
    public interface IRoadRightDependee { }

    public class Road
    {
        private Road() { }
        public static UIItem CreateConstructRoadButton<T>(IMap map, Vector2Int pos, Action back) where T : IRoadLike, ITile {
            if (back == null) throw new Exception();
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Construct>()}{Localization.Ins.Get(typeof(T))}",
                OnTap = () => {
                    //if (CanBeBuiltOn(map, pos, out string info, out Type depender, out Type dependee, out ITile otherTile)) {
                    //    if (otherTile != null) {
                    //        // 不是第一块地，自己添加dependee，另一块地的refs添加depender
                    //        if (map.UpdateAt<T>(pos)) {
                    //            otherTile.Refs.Create(depender); // other
                    //            map.Get(pos).Refs.Create(dependee); // this
                    //            UI.Ins.Active = false;
                    //        }
                    //    } else {
                    //        // 第一块地
                    //        map.UpdateAt(typeof(T), pos);
                    //        UI.Ins.Active = false;
                    //    }
                    //} else {
                    //    UIPreset.Notify(back, info, "不能在此建造道路");
                    //}
                    throw new NotImplementedException();
                }
            };
        }


        public static void ConstructOn(IMap map, Vector2Int pos) {
            map.Values.Get<IRoadLike>().Max++;
        }
        public static void DestructOn(IMap map, Vector2Int pos) {
            map.Values.Get<IRoadLike>().Max--;
        }
        public static bool CanBeBuiltOn(IMap map, Vector2Int pos, out string info, out Type depender, out Type dependee, out ITile otherTile) {
            int count = 0;
            info = null;
            dependee = null;
            depender = null;
            otherTile = null;
            ITile tile;

            tile = map.Get(pos + Vector2Int.left);
            IRoadLike left = tile as IRoadLike;
            if (left != null && left.IsRoadLike()) {
                count++;
                dependee = typeof(IRoadLeftDependee);
                depender = typeof(IRoadLeftDepender);
                otherTile = tile;
            }

            tile = map.Get(pos + Vector2Int.right);
            IRoadLike right = tile as IRoadLike;
            if (right != null && right.IsRoadLike()) {
                count++;
                dependee = typeof(IRoadRightDependee);
                depender = typeof(IRoadRightDepender);
                otherTile = tile;
            }

            tile = map.Get(pos + Vector2Int.up);
            IRoadLike up = tile as IRoadLike;
            if (up != null && up.IsRoadLike()) {
                count++;
                dependee = typeof(IRoadUpDependee);
                depender = typeof(IRoadUpDepender);
                otherTile = tile;
            }

            tile = map.Get(pos + Vector2Int.down);
            IRoadLike down = tile as IRoadLike;
            if (down != null && down.IsRoadLike()) {
                count++;
                dependee = typeof(IRoadDownDependee);
                depender = typeof(IRoadDownDepender);
                otherTile = tile;
            }

            if (count == 0) {
                if (!map.Values.Has<IRoadLike>()) {
                    map.Values.Create<IRoadLike>();
                }
                if (map.Values.Get<IRoadLike>().Max == 0) {
                    return true;
                } else {
                    info = "道路需要和道路相邻";
                    return false;
                }
            } else if (count == 1) {
                return true;
            } else {
                info = "道路不能形成环状结构";
                return false;
            }
        }
    }
    #endregion road
}

