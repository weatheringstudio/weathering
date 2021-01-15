
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("伐木场", "D2A064")]
    public class ForestLoggingCamp : StandardTile
    {
        public override string SpriteKey => "ForestLoggingCamp";

        public override void OnConstruct() {
            worker.Max = 0;
        }

        public override void OnDestruct() {
        }

        private IValue laborValue;
        private IValue foodValue;
        private IValue woodValue;
        private IValue worker;
        public override void OnEnable() {
            laborValue = Map.Values.Get<Labor>();
            foodValue = Map.Values.Get<Food>();
            woodValue = Map.Values.Get<Wood>();
            worker = Map.Values.Get<Worker>();
        }

        public const long LaborDec = 1;
        public const long WoodInc = 1;

        public override void OnTap() {
            string woodColoredName = Concept.Ins.ColoredNameOf<Wood>();
            UI.Ins.UIItems(Concept.Ins.ColoredNameOf<ForestLoggingCamp>(), new List<IUIItem> {
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "伐木场，工人可以在此生产木材",
                },
                worker.Max == 1 ? null : new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"派遣伐木工",
                    OnTap = () => {
                        laborValue.Dec += LaborDec;
                        woodValue.Inc += WoodInc;
                        worker.Max = 1;
                        OnTap();
                    },
                    CanTap = () => laborValue.Sur >= 1,
                },
                worker.Max == 0 ? null : new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"解雇伐木工",
                    OnTap = () => {
                        laborValue.Dec -= LaborDec;
                        woodValue.Inc -= WoodInc;
                        worker.Max = 0;
                        OnTap();
                    },
                },
                new UIItem {
                    Content = woodColoredName,
                    Type = IUIItemType.ValueProgress,
                    Value = woodValue
                },
                new UIItem {
                    Content = woodColoredName,
                    Type = IUIItemType.TimeProgress,
                    Value = woodValue
                },
                new UIItem {
                    Content = woodColoredName,
                    Type = IUIItemType.DelProgress,
                    Value = woodValue
                }
            }) ;
        }
    }
}

