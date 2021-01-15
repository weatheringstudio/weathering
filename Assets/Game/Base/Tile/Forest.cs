
using System;
using System.Collections.Generic;

namespace Weathering
{
    public class Forest : StandardTile
    {
        public override string SpriteKey => typeof(Forest).Name;

        public override bool CanConstruct() => true;

        public override bool CanDestruct() => true;

        private IValue laborValue;
        private IValue woodValue;
        public override void OnEnable() {
            laborValue = Map.Values.Get<Labor>();
            woodValue = Map.Values.Get<Wood>();
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {

            if (Map.Values.Get<BaseCount>().Max == 0) {
                UI.Ins.UIItems("森林", new List<IUIItem> {
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "一片森林，地貌复杂，不适合建造基地",
                },
                new UIItem {
                    Content = "ForestBanner",
                    Type = IUIItemType.Image,
                    LeftPadding = 0
                },
            });
            } else {
                string woodColoredName = Concept.Ins.ColoredNameOf<Wood>();
                UI.Ins.UIItems("森林", new List<IUIItem> {
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "一片森林，可以开采木材",
                },

                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"采集木材。{Concept.Ins.Val<Labor>(-gatherWoodLaborCost)}{Concept.Ins.Val<Wood>(gatherWoodWoodRevenue)}",
                    OnTap = GatherWood,
                    CanTap = CanGatherWood,
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
                },

                new UIItem {
                    Content = "ForestBanner",
                    Type = IUIItemType.Image,
                    LeftPadding = 0
                },
            });
            }
        }

        private long gatherWoodLaborCost = 10;
        private long gatherWoodWoodRevenue = 1;

        private void GatherWood() {
            laborValue.Val -= gatherWoodLaborCost;
            woodValue.Val += gatherWoodWoodRevenue;
        }

        private bool CanGatherWood() {
            return laborValue.Val >= gatherWoodLaborCost;
        }
    }
}

