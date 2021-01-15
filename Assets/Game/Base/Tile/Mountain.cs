
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Mountain : StandardTile
    {
        public override string SpriteKey => typeof(Mountain).Name;

        public override bool CanConstruct() => true;

        public override bool CanDestruct() => true;

        private IValue laborValue;
        private IValue foodValue;
        private IValue woodValue;
        private IValue stoneValue;
        public override void OnEnable() {
            laborValue = Map.Values.Get<Labor>();
            foodValue = Map.Values.Get<Food>();
            woodValue = Map.Values.Get<Wood>();
            stoneValue = Map.Values.Get<Stone>();
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            if (Map.Values.Get<BaseCount>().Max == 0) {
                UI.Ins.UIItems("高山", new List<IUIItem>() {
                    new UIItem {
                        Content = "地势崎岖，不适合建造基地",
                        Type = IUIItemType.MultilineText,
                    },
                    new UIItem {
                        Content = "MountainBanner",
                        Type = IUIItemType.Image,
                        LeftPadding = 0
                    }
                });
            } else {
                string stoneColoredName = Concept.Ins.ColoredNameOf<Stone>();
                UI.Ins.UIItems("高山", new List<IUIItem>() {
                    new UIItem {
                        Content = "哇！好多的矿石",
                        Type = IUIItemType.MultilineText,
                    },
                    new UIItem {
                        Type = IUIItemType.Button,
                        Content = $"采集石材。{Concept.Ins.Val<Labor>(-gatherStoneLaborCost)}{Concept.Ins.Val<Food>(-gatherStoneFoodCost)}{Concept.Ins.Val<Wood>(-gatherStoneWoodCost)}{Concept.Ins.Val<Stone>(gatherStoneStoneRevenue)}",
                        OnTap = () => {
                            laborValue.Val -= gatherStoneLaborCost;
                            foodValue.Val -= gatherStoneFoodCost;
                            woodValue.Val -= gatherStoneWoodCost;
                            stoneValue.Val += gatherStoneStoneRevenue;
                        },
                        CanTap = () =>
                        laborValue.Val >= gatherStoneLaborCost
                        && foodValue.Val >= gatherStoneFoodCost
                        && woodValue.Val >= gatherStoneWoodCost,
                    },
                    new UIItem {
                        Content = stoneColoredName,
                        Type = IUIItemType.ValueProgress,
                        Value = stoneValue
                    },
                    new UIItem {
                        Content = stoneColoredName,
                        Type = IUIItemType.TimeProgress,
                        Value = stoneValue
                    },
                    new UIItem {
                        Content = stoneColoredName,
                        Type = IUIItemType.DelProgress,
                        Value = stoneValue
                    },
                    new UIItem {
                        Content = "MountainBanner",
                        Type = IUIItemType.Image,
                        LeftPadding = 0
                    },
                });
            }
        }

        private long gatherStoneLaborCost = 10;
        private long gatherStoneFoodCost = 10;
        private long gatherStoneWoodCost = 10;
        private long gatherStoneStoneRevenue = 1;
    }
}

