
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Mountain : ITileDefinition
    {
        public string SpriteKey => typeof(Mountain).Name;

        public IValues Values { get; private set; } = null;
        public void SetValues(IValues values) => Values = values;

        public IMap Map { get; set; }
        public UnityEngine.Vector2Int Pos { get; set; }

        public bool CanConstruct() => true;

        public bool CanDestruct() => true;

        private IValue laborValue;
        private IValue woodValue;
        private IValue stoneValue;
        public void Initialize() {
            laborValue = Map.Values.Get<Labor>();
            woodValue = Map.Values.Get<Wood>();
            stoneValue = Map.Values.Get<Stone>();
        }

        public void OnConstruct() {
        }

        public void OnDestruct() {
        }

        public void OnTap() {
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
            }
            else {
                string stoneColoredName = Concept.Ins.ColoredNameOf<Stone>();
                UI.Ins.UIItems("高山", new List<IUIItem>() {
                    new UIItem {
                        Content = "哇！好多的矿石",
                        Type = IUIItemType.MultilineText,
                    },
                    new UIItem {
                        Type = IUIItemType.Button,
                        Content = $"采集石材。{Concept.Ins.Val<Labor>(-gatherStoneLaborCost)}{Concept.Ins.Val<Stone>(gatherStoneStoneRevenue)}",
                        OnTap = GatherStone,
                        CanTap = CanGatherStone,
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
        private long gatherStoneStoneRevenue = 1;

        private void GatherStone() {
            laborValue.Val -= gatherStoneLaborCost;
            woodValue.Val += gatherStoneStoneRevenue;
        }

        private bool CanGatherStone() {
            return laborValue.Val >= gatherStoneLaborCost;
        }
    }
}

