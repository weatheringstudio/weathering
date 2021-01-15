
using System;
using System.Collections.Generic;

namespace Weathering
{
    public class Forest : StandardTile
    {
        public override string SpriteKey => typeof(Forest).Name;

        private IValue laborValue;
        private IValue foodValue;
        private IValue woodValue;
        public override void OnEnable() {
            laborValue = Map.Values.Get<Labor>();
            foodValue = Map.Values.Get<Food>();
            woodValue = Map.Values.Get<Wood>();
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
                    Content = $"采集木材。{Concept.Ins.Val<Labor>(-gatherWoodLaborCost)}{Concept.Ins.Val<Food>(-gatherWoodFoodCost)}{Concept.Ins.Val<Wood>(gatherWoodWoodRevenue)}",
                    OnTap = GatherWood,
                    CanTap = () => laborValue.Val >= gatherWoodLaborCost
                    && foodValue.Val >= gatherWoodFoodCost,
                },
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"建造伐木场。{Concept.Ins.Val<Labor>(-buildLoggingCampLaborCost)}{Concept.Ins.Val<Food>(-buildLoggingCampFoodCost)}{Concept.Ins.Val<ForestLoggingCamp>(gatherWoodWoodRevenue)}",
                    OnTap = BuildLoggingCamp,
                    CanTap = CanBuildLoggingCamp
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

        private const long gatherWoodLaborCost = 10;
        private const long gatherWoodFoodCost = 10;
        private const long gatherWoodWoodRevenue = 1;

        private void GatherWood() {
            laborValue.Val -= gatherWoodLaborCost;
            foodValue.Val -= gatherWoodFoodCost;
            woodValue.Val += gatherWoodWoodRevenue;
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        private const long buildLoggingCampLaborCost = 1;
        private const long buildLoggingCampFoodCost = 1;
        private void BuildLoggingCamp() {
            laborValue.Val -= buildLoggingCampLaborCost;
            foodValue.Val -= buildLoggingCampFoodCost;
            Map.UpdateAt<ForestLoggingCamp>(Pos);
            Map.Get(Pos).OnTap();
        }
        private bool CanBuildLoggingCamp() {
            return laborValue.Val >= buildLoggingCampLaborCost
                && foodValue.Val >= buildLoggingCampFoodCost;
        }
    }
}

