
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    #region road
    [Concept]
    public interface IRoadlike
    {
    }

    public interface IRoadDependerUp { }
    public interface IRoadDependerDown { }
    public interface IRoadDependerLeft { }
    public interface IRoadDependerRight { }

    public interface IRoadDependeeUp { }
    public interface IRoadDependeeDown { }
    public interface IRoadDependeeLeft { }
    public interface IRoadDependeeRight { }

    public class Road
    {
        private Road() { }

        private static Vector2Int left = Vector2Int.left;
        private static Vector2Int right = Vector2Int.right;
        private static Vector2Int up = Vector2Int.up;
        private static Vector2Int down = Vector2Int.down;


        public static UIItem CreateButtonOfConstructingRoad<T>(IMap map, Vector2Int pos, bool requireAdjacency, Action back) where T : IRoadlike, ITile {
            if (back == null) throw new Exception();
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Construct>()}{Localization.Ins.Get(typeof(T))}",
                OnTap = () => {
                    if (CanBeBuiltOn<T>(map, pos, requireAdjacency, out string info, out Type depender, out Type dependee, out ITile otherTile)) {
                        if (otherTile != null) {
                            // 不是第一块地，自己添加dependee，另一块地的refs添加depender
                            if (map.UpdateAt<T>(pos)) {
                                if (otherTile.Refs == null) throw new Exception();
                                otherTile.Refs.Create(depender); // other
                                map.Get(pos).Refs.Create(dependee); // this
                                UI.Ins.Active = false;
                            } else {
                                throw new Exception();
                            }
                        } else {
                            map.UpdateAt(typeof(T), pos);
                            UI.Ins.Active = false;
                        }
                    } else {
                        UIPreset.Notify(back, info, $"此处无法建造{Localization.Ins.Get<T>()}");
                    }
                }
            };
        }

        public static bool CanBeBuiltOn<T>(IMap map, Vector2Int pos, bool requireAdjacency, out string info, out Type depender, out Type dependee, out ITile otherTile) {
            int count = 0;
            info = null;
            dependee = null;
            depender = null;
            otherTile = null;
            ITile tile;

            tile = map.Get(pos + left);
            if (tile as IRoadlike != null) {
                count++;
                dependee = typeof(IRoadDependeeLeft);
                depender = typeof(IRoadDependerLeft);
                otherTile = tile;
            }

            tile = map.Get(pos + right);
            if (tile as IRoadlike != null) {
                count++;
                dependee = typeof(IRoadDependeeRight);
                depender = typeof(IRoadDependerRight);
                otherTile = tile;
            }

            tile = map.Get(pos + up);
            if (tile as IRoadlike != null) {
                count++;
                dependee = typeof(IRoadDependeeUp);
                depender = typeof(IRoadDependerUp);
                otherTile = tile;
            }

            tile = map.Get(pos + down);
            if (tile as IRoadlike != null) {
                count++;
                dependee = typeof(IRoadDependeeDown);
                depender = typeof(IRoadDependerDown);
                otherTile = tile;
            }

            if (count == 0) {
                if (requireAdjacency) {
                    info = $"建造{Localization.Ins.Get<T>()}时，旁边需要有一个{Localization.Ins.Get<IRoadlike>()}";
                    return false;
                } else {
                    return true;
                }
            } else if (count == 1) {
                return true;
            } else {
                info = $"建造{Localization.Ins.Get<T>()}时，旁边最多有一个{Localization.Ins.Get<IRoadlike>()}";
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">拆除后变成的地块类型</typeparam>
        /// <param name="tile"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        public static UIItem CreateButtonOfDestructingRoad<T>(ITile tile, Action back) where T : ITile {
            if (tile as IRoadlike == null) throw new Exception();
            if (typeof(IRoadlike).IsAssignableFrom(typeof(T))) throw new Exception();
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Destruct>()}{Localization.Ins.Get(tile.GetType())}",
                OnTap = () => {
                    if (tile.Refs.Has<IRoadDependerLeft>()) {
                        UIPreset.Notify(back, "西边需要这条道路", "无法拆除道路");
                        return;
                    }
                    if (tile.Refs.Has<IRoadDependerRight>()) {
                        UIPreset.Notify(back, "东边需要这条道路", "无法拆除道路");
                        return;
                    }
                    if (tile.Refs.Has<IRoadDependerUp>()) {
                        UIPreset.Notify(back, "北边需要这条道路", "无法拆除道路");
                        return;
                    }
                    if (tile.Refs.Has<IRoadDependerDown>()) {
                        UIPreset.Notify(back, "南边需要这条道路", "无法拆除道路");
                        return;
                    }
                    IMap map = tile.GetMap();
                    Vector2Int pos = tile.GetPos();
                    if (tile.Refs.Has<IRoadDependeeLeft>()) {
                        tile.Refs.Remove<IRoadDependeeLeft>();
                        ITile other = map.Get(pos + left);
                        other.Refs.Remove<IRoadDependerLeft>();
                        map.UpdateAt<T>(pos);
                        other.OnTap();
                        return;
                    }
                    if (tile.Refs.Has<IRoadDependeeRight>()) {
                        tile.Refs.Remove<IRoadDependeeRight>();
                        ITile other = map.Get(pos + right);
                        other.Refs.Remove<IRoadDependerRight>();
                        map.UpdateAt<T>(pos);
                        other.OnTap();
                        return;
                    }
                    if (tile.Refs.Has<IRoadDependeeUp>()) {
                        tile.Refs.Remove<IRoadDependeeUp>();
                        ITile other = map.Get(pos + up);
                        other.Refs.Remove<IRoadDependerUp>();
                        map.UpdateAt<T>(pos);
                        other.OnTap();
                        return;
                    }
                    if (tile.Refs.Has<IRoadDependeeDown>()) {
                        tile.Refs.Remove<IRoadDependeeDown>();
                        ITile other = map.Get(pos + down);
                        other.Refs.Remove<IRoadDependerDown>();
                        map.UpdateAt<T>(pos);
                        other.OnTap();
                        return;
                    }
                    Debug.LogWarning("initial");
                    map.UpdateAt<T>(pos);
                    UI.Ins.Active = false;
                }
            };
        }





    }
    #endregion road
}

