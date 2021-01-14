
using System;
using System.Collections.Generic;

namespace Weathering
{
    public class Forest : ITileDefinition
    {
        public string SpriteKey => typeof(Forest).Name;

        public IValues Values { get; private set; } = null;
        public void SetValues(IValues values) => Values = values;

        public IMap Map { get; set; }
        public UnityEngine.Vector2Int Pos { get; set; }

        public bool CanConstruct() => true;

        public bool CanDestruct() => true;

        private IValue laborValue;
        private IValue woodValue;
        public void Initialize() {
            //if (Values == null) {
            //    Weathering.Values.Create();
            //}
            laborValue = Map.Values.Get<Labor>();
            woodValue = Map.Values.Get<Wood>();
        }

        public void OnConstruct() {
        }

        public void OnDestruct() {
        }

        public void OnTap() {

            if (Map.Values.Get<BaseCount>().Max == 0) {
                UI.Ins.UIItems("森林", new List<IUIItem> {
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "一片森林，地貌复杂，不适合建造基地",
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
                    Value = Map.Values.Get<Wood>()
                },
                new UIItem {
                    Content = woodColoredName,
                    Type = IUIItemType.TimeProgress,
                    Value = Map.Values.Get<Wood>()
                },
                new UIItem {
                    Content = woodColoredName,
                    Type = IUIItemType.DelProgress,
                    Value = Map.Values.Get<Wood>()
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

