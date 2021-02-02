

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public static class TileUtility
    {
        public static int Calculate4x4RuleTileIndex(Func<ITile, bool> predicate, IMap map, Vector2Int context) {
            //bool left = map.Get(context.x - 1, context.y).GetType() == type;
            //bool right = map.Get(context.x + 1, context.y).GetType() == type;
            //bool up = map.Get(context.x, context.y + 1).GetType() == type;
            //bool down = map.Get(context.x, context.y - 1).GetType() == type;

            //bool left = Tag.Ins.HasTag(map.Get(context.x - 1, context.y).GetType(), type);
            //bool right = Tag.Ins.HasTag(map.Get(context.x + 1, context.y).GetType(), type);
            //bool up = Tag.Ins.HasTag(map.Get(context.x, context.y + 1).GetType(), type);
            //bool down = Tag.Ins.HasTag(map.Get(context.x, context.y - 1).GetType(), type);

            bool left = predicate(map.Get(context.x - 1, context.y));
            bool right = predicate(map.Get(context.x + 1, context.y));
            bool up = predicate(map.Get(context.x, context.y + 1));
            bool down = predicate(map.Get(context.x, context.y - 1));

            int index = Calculate4x4RuleTileIndex(left, right, up, down);
            return index;
        }

        public static int Calculate4x4RuleTileIndex(bool left, bool right, bool up, bool down) {
            if (left) {
                if (right) {
                    if (up) {
                        // ^ v < >
                        if (down) return 1 * 4 + 1;
                        return 2 * 4 + 1;
                    } else {
                        //   v < >
                        if (down) return 0 * 4 + 1;
                        return 3 * 4 + 1;
                    }
                } else {
                    if (up) {
                        // ^ v <  
                        if (down) return 1 * 4 + 2;
                        return 2 * 4 + 2;
                    } else {
                        //   v <  
                        if (down) return 0 * 4 + 2;
                        return 3 * 4 + 2;
                    }
                }
            } else {
                if (right) {
                    if (up) {
                        // ^ v   >
                        if (down) return 1 * 4 + 0;
                        return 2 * 4 + 0;
                    } else {
                        //   v   >
                        if (down) return 0 * 4 + 0;
                        return 3 * 4 + 0;
                    }
                } else {
                    if (up) {
                        // ^ v    
                        if (down) return 1 * 4 + 3;
                        return 2 * 4 + 3;
                    } else {
                        //   v    
                        if (down) return 0 * 4 + 3;
                        return 3 * 4 + 3;
                    }
                }
            }
            throw new Exception();
        }

        public static int Calculate6x8RuleTileIndex(Type type, IMap map, Vector2Int context) {
            bool left = map.Get(context.x - 1, context.y).GetType() == type;
            bool right = map.Get(context.x + 1, context.y).GetType() == type;
            bool up = map.Get(context.x, context.y + 1).GetType() == type;
            bool down = map.Get(context.x, context.y - 1).GetType() == type;
            bool upLeft = map.Get(context.x - 1, context.y + 1).GetType() == type;
            bool upRight = map.Get(context.x + 1, context.y + 1).GetType() == type;
            bool downLeft = map.Get(context.x - 1, context.y - 1).GetType() == type;
            bool downRight = map.Get(context.x + 1, context.y - 1).GetType() == type;
            int index = Calculate6x8RuleTileIndex(left, right, up, down, upLeft, upRight, downLeft, downRight);
            return index;
        }
        public static int Calculate6x8RuleTileIndex(bool left, bool right,
            bool up, bool down, bool upLeft, bool upRight,
            bool downLeft, bool downRight) {

            if (left) {
                if (right) {
                    if (up) {
                        if (down) {
                            if (upLeft && upRight && downLeft && downRight) return 1 * 8 + 1;

                            if (!upLeft && upRight && downLeft && downRight) return 5 * 8 + 2;
                            if (upLeft && !upRight && downLeft && downRight) return 5 * 8 + 0;
                            if (upLeft && upRight && !downLeft && downRight) return 3 * 8 + 2;
                            if (upLeft && upRight && downLeft && !downRight) return 3 * 8 + 0;

                            if (upLeft && upRight && !downLeft && !downRight) return 3 * 8 + 1;
                            if (upLeft && !upRight && downLeft && !downRight) return 4 * 8 + 0;
                            if (upLeft && !upRight && !downLeft && downRight) return 0 * 8 + 7;
                            if (!upLeft && upRight && downLeft && !downRight) return 0 * 8 + 6;
                            if (!upLeft && upRight && !downLeft && downRight) return 4 * 8 + 2;
                            if (!upLeft && !upRight && downLeft && downRight) return 5 * 8 + 1;

                            if (upLeft && !upRight && !downLeft && !downRight) return 2 * 8 + 7;
                            if (!upLeft && upRight && !downLeft && !downRight) return 2 * 8 + 6;
                            if (!upLeft && !upRight && downLeft && !downRight) return 1 * 8 + 7;
                            if (!upLeft && !upRight && !downLeft && downRight) return 1 * 8 + 6;

                            if (!upLeft && !upRight && !downLeft && !downRight) return 1 * 8 + 4;

                            throw new Exception();
                        } else {
                            // < > ^  
                            if (upLeft && upRight) return 2 * 8 + 1;
                            if (!upLeft && upRight) return 4 * 8 + 6;
                            if (upLeft && !upRight) return 4 * 8 + 7;
                            if (!upLeft && !upRight) return 2 * 8 + 4;

                            throw new Exception();
                        }
                    } else {
                        if (down) {
                            // < >   v
                            if (downLeft && downRight) return 0 * 8 + 1;
                            if (!downLeft && downRight) return 3 * 8 + 6;
                            if (downLeft && !downRight) return 3 * 8 + 7;
                            if (!downLeft && !downRight) return 0 * 8 + 4;

                        } else {
                            // < >   
                            return 5 * 8 + 5;
                        }
                    }
                } else {
                    if (up) {
                        if (down) {
                            // <   ^ v
                            if (upLeft && downLeft) return 1 * 8 + 2;
                            if (!upLeft && downLeft) return 3 * 8 + 5;
                            if (upLeft && !downLeft) return 4 * 8 + 5;
                            if (!upLeft && !downLeft) return 1 * 8 + 5;

                            throw new Exception();
                        } else {
                            // <   ^  
                            if (upLeft) return 2 * 8 + 2;
                            return 2 * 8 + 5;
                        }
                    } else {
                        if (down) {
                            // <     v
                            if (downLeft) return 0 * 8 + 2;
                            return 0 * 8 + 5;
                        } else {
                            // <    
                            return 5 * 8 + 6;
                        }
                    }
                }
            } else {
                if (right) {
                    if (up) {
                        if (down) {
                            //   > ^ v
                            if (upRight && downRight) return 1 * 8 + 0;
                            if (!upRight && downRight) return 3 * 8 + 4;
                            if (upRight && !downRight) return 4 * 8 + 4;
                            if (!upRight && !downRight) return 1 * 8 + 3;

                            throw new Exception();
                        } else {
                            //   > ^
                            if (upRight) return 2 * 8 + 0;
                            return 2 * 8 + 3;
                        }
                    } else {
                        if (down) {
                            //   >   v
                            if (downRight) return 0 * 8 + 0;
                            return 0 * 8 + 3;
                        } else {
                            //   >    
                            return 5 * 8 + 4;
                        }
                    }
                } else {
                    if (up) {
                        if (down) {
                            //     ^ v
                            return 4 * 8 + 3;
                        } else {
                            //     ^  
                            return 5 * 8 + 3;
                        }
                    } else {
                        if (down) {
                            //       v
                            return 3 * 8 + 3;
                        } else {
                            //
                            return 5 * 8 + 7;
                        }
                    }
                }
            }
            return 4 * 8 + 1;
        }
    }
}

