
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Weathering
//{

//    [Concept]
//    public class GrasslandRoad : StandardTile, IRoadlike, IDefaultDestruction
//    {
//        public override string SpriteKey {
//            get {
//                Func<ITile, bool> predicate = tile => {
//                    if (tile as IRoadlike != null) {
//                        return true;
//                    }
//                    if (tile as ILookLikeRoad != null && (tile as ILookLikeRoad).LookLikeRoad) {
//                        return true;
//                    }
//                    return false;
//                };
//                int index = TileUtility.Calculate4x4RuleTileIndex(predicate, Map, Pos);
//                return $"StoneRoad_{index}";
//            }
//        }

//        public override string SpriteLeft {
//            get {
//                return Refs.Has<IRoadDependeeLeft>() ? "GrasslandEnemy0" :  null;
//            }
//        }
//        public override string SpriteRight {
//            get {
//                return Refs.Has<IRoadDependeeRight>() ? "GrasslandEnemy0" : null;
//            }
//        }
//        public override string SpriteUp {
//            get {
//                return Refs.Has<IRoadDependeeUp>() ? "GrasslandEnemy0" : null;
//            }
//        }
//        public override string SpriteDown {
//            get {
//                return Refs.Has<IRoadDependerDown>() ? "GrasslandEnemy0" : null;
//            }
//        }

//        public Type DefaultDestruction => typeof(Grassland);

//        public override void OnConstruct() {
//            base.OnConstruct();
//            Refs = Weathering.Refs.GetOne();
//        }

//        public override void OnTap() {
//            var items = new List<IUIItem>();

//            items.Add(UIItem.CreateMultilineText("【在目前游戏版本中, 道路暂时没有作用。在以后的版本中, 建筑需要贴近道路才能自动化, 进行物流】"));

//            items.Add(RoadUtility.CreateButtonOfDestructingRoad<Grassland>(this, OnTap));
//            items.Add(RoadUtility.CreateButtonOfDestructingRoad<Grassland>(this, OnTap, true));

//            UI.Ins.ShowItems(Localization.Ins.Get<GrasslandRoad>(), items);
//        }
//    }
//}

