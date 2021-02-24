
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    /// <summary>
    /// IRoadlike必须在OnConstruct里创建Refs
    /// </summary>
    [Concept]
    public interface IRoadlike { }
    public interface ILookLikeRoad
    {
        bool LookLikeRoad { get; }
    }

    /// <summary>
    /// 被批量拆除时，会变成什么地块
    /// </summary>
    public interface IDefaultDestruction
    {
        Type DefaultDestruction { get; }
    }


    public interface IRoadDependerUp { }
    public interface IRoadDependerDown { }
    public interface IRoadDependerLeft { }
    public interface IRoadDependerRight { }

    public interface IRoadDependeeUp { }
    public interface IRoadDependeeDown { }
    public interface IRoadDependeeLeft { }
    public interface IRoadDependeeRight { }

    public static class RoadUtility
    {
        private static Vector2Int left = Vector2Int.left;
        private static Vector2Int right = Vector2Int.right;
        private static Vector2Int up = Vector2Int.up;
        private static Vector2Int down = Vector2Int.down;

        /// <summary>
        /// 能否连接道路。带提示
        /// </summary>
        public static bool CanLinkRoad(ITile tile, Action back) {
            if (!CanLinkRoad(tile)) {
                UIPreset.Notify(back, $"四周没找到{Localization.Ins.Get<IRoadlike>()}", "此操作无法进行");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 能否连接道路
        /// </summary>
        public static bool CanLinkRoad(ITile tile) {
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            if (map.Get(pos + left) as IRoadlike != null) return true;
            if (map.Get(pos + right) as IRoadlike != null) return true;
            if (map.Get(pos + up) as IRoadlike != null) return true;
            if (map.Get(pos + down) as IRoadlike != null) return true;
            return false;
        }

        /// <summary>
        /// 连接道路
        /// </summary>
        public static int LinkRoad(ITile tile) {
            if (tile.Refs == null) throw new Exception(tile.GetType().FullName);

            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            int count = 0;
            ITile leftTile = map.Get(pos + left);
            bool leftRoadlike = leftTile as IRoadlike != null;
            if (leftRoadlike) count++;
            ITile rightTile = map.Get(pos + right);
            bool rightRoadlike = rightTile as IRoadlike != null;
            if (rightRoadlike) count++;
            ITile upTile = map.Get(pos + up);
            bool upRoadlike = upTile as IRoadlike != null;
            if (upRoadlike) count++;
            ITile downTile = map.Get(pos + down);
            bool downRoadlike = downTile as IRoadlike != null;
            if (downRoadlike) count++;

            if (leftRoadlike) {
                if (leftTile.Refs == null) throw new Exception(leftTile.GetType().FullName);
                leftTile.Refs.Create<IRoadDependerLeft>();
                tile.Refs.Create<IRoadDependeeLeft>();
            }
            if (rightRoadlike) {
                if (rightTile.Refs == null) throw new Exception(rightTile.GetType().FullName);
                rightTile.Refs.Create<IRoadDependerRight>();
                tile.Refs.Create<IRoadDependeeRight>();
            }
            if (upRoadlike) {
                if (upTile.Refs == null) throw new Exception(upTile.GetType().FullName);
                upTile.Refs.Create<IRoadDependerUp>();
                tile.Refs.Create<IRoadDependeeUp>();
            }
            if (downRoadlike) {
                if (downTile.Refs == null) throw new Exception(downTile.GetType().FullName);
                downTile.Refs.Create<IRoadDependerDown>();
                tile.Refs.Create<IRoadDependeeDown>();
            }
            return count;
        }

        /// <summary>
        /// 能否取消连接道路
        /// </summary>
        //public static bool CanUnlinkRoadlike(ITile tile) {
        //    if (tile.Refs.Has<IRoadDependerLeft>()) return false;
        //    if (tile.Refs.Has<IRoadDependerRight>()) return false;
        //    if (tile.Refs.Has<IRoadDependerUp>()) return false;
        //    if (tile.Refs.Has<IRoadDependerDown>()) return false;
        //    return true;
        //}

        /// <summary>
        /// 取消连接道路
        /// </summary>
        public static void UnlinkRoad(ITile tile) {
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            if (tile.Refs.Has<IRoadDependeeLeft>()) {
                tile.Refs.Remove<IRoadDependeeLeft>();
                map.Get(pos + left).Refs.Remove<IRoadDependerLeft>();
            }
            if (tile.Refs.Has<IRoadDependeeRight>()) {
                tile.Refs.Remove<IRoadDependeeRight>();
                map.Get(pos + right).Refs.Remove<IRoadDependerRight>();
            }
            if (tile.Refs.Has<IRoadDependeeUp>()) {
                tile.Refs.Remove<IRoadDependeeUp>();
                map.Get(pos + up).Refs.Remove<IRoadDependerUp>();
            }
            if (tile.Refs.Has<IRoadDependeeDown>()) {
                tile.Refs.Remove<IRoadDependeeDown>();
                map.Get(pos + down).Refs.Remove<IRoadDependerDown>();
            }
        }


        /// <summary>
        /// 建设道路
        /// </summary>
        /// <typeparam name="T">创建的类型</typeparam>
        /// <param name="map">地图</param>
        /// <param name="pos">位置</param>
        /// <param name="requireAdjacency">是否需要依赖周围其他道路</param>
        /// <param name="back">错误提示返回</param>
        /// <returns></returns>
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
                                map.Get(pos).Refs.Create(dependee); // this
                                otherTile.Refs.Create(depender); // other
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

        private static bool CanBeBuiltOn<T>(IMap map, Vector2Int pos, bool requireAdjacency, out string info, out Type depender, out Type dependee, out ITile otherTile) {
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
                    info = $"建造{Localization.Ins.Get<T>()}时，旁边至少有一个{Localization.Ins.Get<IRoadlike>()}。第一个{Localization.Ins.Get<IRoadlike>()}是{Localization.Ins.Get<SeaHolyShip>()}";
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

        private static bool CanDestructRoad(ITile tile, Action back, out string info) {
            string name = Localization.Ins.Get(tile.GetType());
            if (tile.Refs.Has<IRoadDependerLeft>()) {
                info = $"东边需要{name}";
                return false;
            }
            if (tile.Refs.Has<IRoadDependerRight>()) {
                info = $"西边需要{name}";
                return false;
            }
            if (tile.Refs.Has<IRoadDependerUp>()) {
                info = $"南边需要{name}";
                return false;
            }
            if (tile.Refs.Has<IRoadDependerDown>()) {
                info = $"北边需要{name}";
                return false;
            }
            info = null;
            return true;
        }

        private const int DEPTH = 100;
        /// <summary>
        /// 拆除道路
        /// </summary>
        /// <typeparam name="T">拆除后变成的地块类型</typeparam>
        /// <param name="tile"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        public static UIItem CreateButtonOfDestructingRoad<T>(ITile tile, Action back, bool recursive = false) where T : ITile {
            if (tile as IRoadlike == null) throw new Exception();
            if (typeof(IRoadlike).IsAssignableFrom(typeof(T))) throw new Exception();
            string resursivePrefix = recursive ? "递归" : "";
            int depth = 0;
            return new UIItem {
                Type = IUIItemType.Button,
                Content = $"{resursivePrefix}{Localization.Ins.Get<Destruct>()}{Localization.Ins.Get(tile.GetType())}",
                OnTap = () => DestructRoad(tile, back, ref depth, recursive)
            };
        }
        private static void DestructRoad(ITile tile, Action back, ref int depth, bool recursive = false) {
            IDefaultDestruction destructTo = (tile as IDefaultDestruction);
            if (destructTo == null) throw new Exception(tile.GetType().FullName);
            Type type = destructTo.DefaultDestruction;
            if (!typeof(ITileDefinition).IsAssignableFrom(type)) {
                throw new Exception(type.FullName);
            }

            if (!CanDestructRoad(tile, back, out string info)) {
                UIPreset.Notify(back, info, $"无法拆除{Localization.Ins.Get(tile.GetType())}");
                return;
            }
            if (depth > DEPTH) {
                UIPreset.Notify(back, "达成成就：一次拆除100条道路", "恭喜");
                return;
            }
            depth++;

            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            if (tile.Refs.Has<IRoadDependeeLeft>()) {
                tile.Refs.Remove<IRoadDependeeLeft>();
                ITile other = map.Get(pos + left);
                other.Refs.Remove<IRoadDependerLeft>();
                map.UpdateAt(type, pos);
                TryDestructRoad(other, back, recursive);
                return;
            }
            if (tile.Refs.Has<IRoadDependeeRight>()) {
                tile.Refs.Remove<IRoadDependeeRight>();
                ITile other = map.Get(pos + right);
                other.Refs.Remove<IRoadDependerRight>();
                map.UpdateAt(type, pos);
                TryDestructRoad(other, back, recursive);
                return;
            }
            if (tile.Refs.Has<IRoadDependeeUp>()) {
                tile.Refs.Remove<IRoadDependeeUp>();
                ITile other = map.Get(pos + up);
                other.Refs.Remove<IRoadDependerUp>();
                map.UpdateAt(type, pos);
                TryDestructRoad(other, back, recursive);
                return;
            }
            if (tile.Refs.Has<IRoadDependeeDown>()) {
                tile.Refs.Remove<IRoadDependeeDown>();
                ITile other = map.Get(pos + down);
                other.Refs.Remove<IRoadDependerDown>();
                map.UpdateAt(type, pos);
                TryDestructRoad(other, back, recursive);
                return;
            }
            map.UpdateAt(type, pos);
            UI.Ins.Active = false;
        }

        private static void TryDestructRoad(ITile other, Action back, bool recursive) {
            int depth = 0;
            if (other.CanDestruct()) {
                if (recursive) {
                    if (CanDestructRoad(other, back, out string info)) {
                        DestructRoad(other, back, ref depth, recursive);
                    } else {
                        UI.Ins.Active = false;
                    }
                } else {
                    other.OnTap();
                }
            } else {
                UI.Ins.Active = false;
            }
        }
    }
}

