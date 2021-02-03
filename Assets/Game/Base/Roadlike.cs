
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    #region road

    /// <summary>
    /// IRoadlike必须在OnConstruct里创建Refs
    /// </summary>
    [Concept]
    public interface IRoadlike { }

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

        public static bool TryDependOn(ITile tile, Action back) {
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            int count = 0;
            ITile leftTile = map.Get(pos + left);
            bool leftRoadlike = leftTile as IRoadlike == null;
            if (leftRoadlike) { count++; }
            ITile rightTile = map.Get(pos + right);
            bool rightRoadlike = leftTile as IRoadlike == null;
            if (rightRoadlike) { count++; }
            ITile upTile = map.Get(pos + up);
            bool upRoadlike = leftTile as IRoadlike == null;
            if (upRoadlike) { count++; }
            ITile downTile = map.Get(pos + down);
            bool downRoadlike = leftTile as IRoadlike == null;
            if (downRoadlike) { count++; }

            if (count == 0) {
                UIPreset.Notify(back, $"需要在{Localization.Ins.Get<IRoadlike>()}旁进行此操作");
                return false;
            } else if (count == 1) {
                if (leftRoadlike) {
                    leftTile.Refs.Create<IRoadDependerLeft>();
                    tile.Refs.Create<IRoadDependeeLeft>();
                } else if (rightRoadlike) {
                    rightTile.Refs.Create<IRoadDependerRight>();
                    tile.Refs.Create<IRoadDependeeRight>();
                } else if (upRoadlike) {
                    rightTile.Refs.Create<IRoadDependerUp>();
                    tile.Refs.Create<IRoadDependeeUp>();
                } else if (downRoadlike) {
                    rightTile.Refs.Create<IRoadDependerDown>();
                    tile.Refs.Create<IRoadDependeeDown>();
                } else {
                    throw new Exception();
                }
                return true;
            } else {
                throw new NotImplementedException();
            }
        }

        public static void TryReleaseDependOn(ITile tile, Action back) {
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            int count = 0;
            ITile leftTile = map.Get(pos + left);
            bool leftRoadlike = leftTile as IRoadlike == null;
            if (leftRoadlike) { count++; }
            ITile rightTile = map.Get(pos + right);
            bool rightRoadlike = leftTile as IRoadlike == null;
            if (rightRoadlike) { count++; }
            ITile upTile = map.Get(pos + up);
            bool upRoadlike = leftTile as IRoadlike == null;
            if (upRoadlike) { count++; }
            ITile downTile = map.Get(pos + down);
            bool downRoadlike = leftTile as IRoadlike == null;
            if (downRoadlike) { count++; }

            throw new Exception();
        }

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
        /// 
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
                OnTap = () => DestructRoad<T>(tile, back, ref depth, recursive)
            };
        }
        private static void DestructRoad<T>(ITile tile, Action back, ref int depth, bool recursive = false) where T : ITile {
            if (depth > DEPTH) {
                UIPreset.Notify(back, "达成成就：一次拆除100条道路", "恭喜");
            }
            depth++;
            if (!CanDestructRoad(tile, back, out string info)) {
                UIPreset.Notify(back, info, $"无法拆除{Localization.Ins.Get(tile.GetType())}");
                return;
            }
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            if (tile.Refs.Has<IRoadDependeeLeft>()) {
                tile.Refs.Remove<IRoadDependeeLeft>();
                ITile other = map.Get(pos + left);
                other.Refs.Remove<IRoadDependerLeft>();
                map.UpdateAt<T>(pos);
                TryTapOtherTile<T>(other, back, recursive);
                return;
            }
            if (tile.Refs.Has<IRoadDependeeRight>()) {
                tile.Refs.Remove<IRoadDependeeRight>();
                ITile other = map.Get(pos + right);
                other.Refs.Remove<IRoadDependerRight>();
                map.UpdateAt<T>(pos);
                TryTapOtherTile<T>(other, back, recursive);
                return;
            }
            if (tile.Refs.Has<IRoadDependeeUp>()) {
                tile.Refs.Remove<IRoadDependeeUp>();
                ITile other = map.Get(pos + up);
                other.Refs.Remove<IRoadDependerUp>();
                map.UpdateAt<T>(pos);
                TryTapOtherTile<T>(other, back, recursive);
                return;
            }
            if (tile.Refs.Has<IRoadDependeeDown>()) {
                tile.Refs.Remove<IRoadDependeeDown>();
                ITile other = map.Get(pos + down);
                other.Refs.Remove<IRoadDependerDown>();
                map.UpdateAt<T>(pos);
                TryTapOtherTile<T>(other, back, recursive);
                return;
            }
            map.UpdateAt<T>(pos);
            UI.Ins.Active = false;
        }
        private static void TryTapOtherTile<T>(ITile other, Action back, bool recursive) where T : ITile {
            int depth = 0;
            if (other.CanDestruct()) {
                if (recursive) {
                    if (CanDestructRoad(other, back, out string info)) {
                        DestructRoad<T>(other, back, ref depth, recursive);
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
    #endregion road
}

