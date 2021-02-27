

using System;
using UnityEngine;

namespace Weathering
{
    public static class TileUtility
    {
        public static int Distance(Vector2Int lhs, Vector2Int rhs, int Width, int Height) {
            int x = DistanceOneDimension(lhs.x, rhs.x, Width);
            int y = DistanceOneDimension(lhs.y, rhs.y, Height);
            return x + y;
        }
        private static int DistanceOneDimension(int x1, int x2, int size) {
            int maxX;
            int minX;
            if (x1 > x2) {
                maxX = x1;
                minX = x2;
            } else {
                maxX = x2;
                minX = x1;
            }
            return Max(Abs(maxX - minX), (minX - maxX - size));
        }
        private static int Abs(int x) {
            return x >= 0 ? x : -x;
        }
        private static int Max(int x, int y) {
            return x > y ? x : y;
        }
        private static int Min(int x, int y) {
            return x < y ? x : y;
        }

        public static Vector3 PixelPerfect(Vector3 pos) {
            pos.x = ((int)(pos.x * 16f)) / 16f;
            pos.y = ((int)(pos.y * 16f)) / 16f;
            return pos;
        }

        public static Vector2Int DeltaDirection(Vector2Int vec0, Vector2Int vec1, int width, int height) {
            Vector2Int result = vec0 - vec1;
            if (vec0.x != vec1.x) {
                if (vec0.y != vec1.y) throw new Exception();
                
            }
            return result;
        }

        public static int Calculate4x4RuleTileIndex(Func<ITile, Type, bool> predicate, IMap map, Vector2Int context) {
            //bool left = map.Get(context.x - 1, context.y).GetType() == type;
            //bool right = map.Get(context.x + 1, context.y).GetType() == type;
            //bool up = map.Get(context.x, context.y + 1).GetType() == type;
            //bool down = map.Get(context.x, context.y - 1).GetType() == type;

            //bool left = Tag.Ins.HasTag(map.Get(context.x - 1, context.y).GetType(), type);
            //bool right = Tag.Ins.HasTag(map.Get(context.x + 1, context.y).GetType(), type);
            //bool up = Tag.Ins.HasTag(map.Get(context.x, context.y + 1).GetType(), type);
            //bool down = Tag.Ins.HasTag(map.Get(context.x, context.y - 1).GetType(), type);

            bool left = predicate(map.Get(context.x - 1, context.y), typeof(ILeft));
            bool right = predicate(map.Get(context.x + 1, context.y), typeof(IRight));
            bool up = predicate(map.Get(context.x, context.y + 1), typeof(IUp));
            bool down = predicate(map.Get(context.x, context.y - 1), typeof(IDown));

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

        public static int Calculate6x8RuleTileIndex(Func<ITile, bool> predicate, IMap map, Vector2Int context) {
            //bool left = map.Get(context.x - 1, context.y).GetType() == type;
            //bool right = map.Get(context.x + 1, context.y).GetType() == type;
            //bool up = map.Get(context.x, context.y + 1).GetType() == type;
            //bool down = map.Get(context.x, context.y - 1).GetType() == type;
            //bool upLeft = map.Get(context.x - 1, context.y + 1).GetType() == type;
            //bool upRight = map.Get(context.x + 1, context.y + 1).GetType() == type;
            //bool downLeft = map.Get(context.x - 1, context.y - 1).GetType() == type;
            //bool downRight = map.Get(context.x + 1, context.y - 1).GetType() == type;

            bool left = predicate(map.Get(context.x - 1, context.y));  // map.Get(context.x - 1, context.y).GetType() == type; 
            bool right = predicate(map.Get(context.x + 1, context.y)); // map.Get(context.x + 1, context.y).GetType() == type;
            bool up = predicate(map.Get(context.x, context.y + 1)); // map.Get(context.x, context.y + 1).GetType() == type;
            bool down = predicate(map.Get(context.x, context.y - 1)); // map.Get(context.x, context.y - 1).GetType() == type;
            bool upLeft = predicate(map.Get(context.x - 1, context.y + 1)); // map.Get(context.x - 1, context.y + 1).GetType() == type;
            bool upRight = predicate(map.Get(context.x + 1, context.y + 1)); // map.Get(context.x + 1, context.y + 1).GetType() == type;
            bool downLeft = predicate(map.Get(context.x - 1, context.y - 1)); // map.Get(context.x - 1, context.y - 1).GetType() == type;
            bool downRight = predicate(map.Get(context.x + 1, context.y - 1)); // map.Get(context.x + 1, context.y - 1).GetType() == type;


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

