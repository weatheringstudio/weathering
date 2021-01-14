
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("草地", "BCFF45")]
    public class Grassland : ITileDefinition
    {
        public string SpriteKey => typeof(Grassland).Name;

        public IValues Values { get; private set; } = null;
        public void SetValues(IValues values) => Values = values;

        public IMap Map { get; set; }
        public UnityEngine.Vector2Int Pos { get; set; }

        public bool CanConstruct() => true;

        public bool CanDestruct() => true;

        private IValue laborValue;
        private IValue foodValue;
        public void Initialize() {
            laborValue = Map.Values.Get<Labor>();
            foodValue = Map.Values.Get<Food>();
        }

        public void OnConstruct() {
        }

        public void OnDestruct() {
        }


        public void OnTap() {
            string foodColoredName = Concept.Ins.ColoredNameOf<Food>();
            string grasslandColoredName = Concept.Ins.ColoredNameOf<Grassland>();
            string baseColoredName = Concept.Ins.ColoredNameOf<Base>();
            if (Map.Values.Get<BaseCount>().Max == 0) {
                UI.Ins.UIItems(grasslandColoredName, new List<IUIItem> {
                    new UIItem {
                        Type = IUIItemType.MultilineText,
                        Content = "一片" + grasslandColoredName+"，适合建立" + baseColoredName,
                    },
                    new UIItem {
                        Type = IUIItemType.Button,
                        Content = "建立" + baseColoredName,
                        OnTap = () => {
                            Map.UpdateAt<Base>(Pos);
                            UI.Ins.Active = false;
                        },
                    },
                }) ;
            } else {
                IValue mapFood = Map.Values.Get<Food>();
                UI.Ins.UIItems(grasslandColoredName, new List<IUIItem> {
                    new UIItem {
                        Type = IUIItemType.MultilineText,
                        Content = $"一片{grasslandColoredName}，气候适宜",
                    },
                    new UIItem {
                        Type = IUIItemType.Button,
                        Content = $"采集浆果。{Concept.Ins.Val<Labor>(-gatherFruitLaborCost)}{Concept.Ins.Val<Food>(gatherFruitFoodRevenue)}",
                        OnTap = GatherFruit,
                        CanTap = CanGatherFruit,
                    },
                    new UIItem {
                        Type = IUIItemType.Button,
                        Content = "播种浆果" + BerryBush.ConsturctionDescription,
                        OnTap = () => {
                            if (Map.UpdateAt<BerryBush>(Pos)) {
                                Map.Get(Pos).OnTap();
                            }
                        },
                        CanTap = () => BerryBush.CanBeBuiltOn(Map, Pos),
                    },
                    new UIItem {
                        Type = IUIItemType.Button,
                        Content = "建造小屋" + Residence.ConsturctionDescription,
                        OnTap = () => {
                            if (Map.UpdateAt<Residence>(Pos)) {
                                Map.Get(Pos).OnTap();
                            }
                        },
                        CanTap = () => Residence.CanBeBuiltOn(Map, Pos),
                    },
                    new UIItem {
                        Content = Concept.Ins.ColoredNameOf<Labor>(),
                        Type = IUIItemType.ValueProgress,
                        Value = Map.Values.Get<Labor>()
                    },
                    new UIItem {
                        Content = foodColoredName,
                        Type = IUIItemType.ValueProgress,
                        Value = Map.Values.Get<Food>()
                    },
                });
            }
        }

        private readonly long gatherFruitLaborCost = 1;
        private readonly long gatherFruitFoodRevenue = 1;
        private void GatherFruit() {
            foodValue.Val += gatherFruitFoodRevenue;
            laborValue.Val -= gatherFruitLaborCost;
        }

        private bool CanGatherFruit() {
            return laborValue.Val >= gatherFruitLaborCost;
        }


    }
}

