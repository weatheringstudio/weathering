
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Concept("草地", "BCFF45")]
    public class Grassland : StandardTile
    {
        public override string SpriteKey => typeof(Grassland).Name;

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
        }
    }

    //[Concept("草地", "BCFF45")]
    //public class Grassland : StandardTile
    //{
    //    public override string SpriteKey => typeof(Grassland).Name;

    //    private IValue laborValue;
    //    private IValue foodValue;
    //    public override void OnEnable() {
    //        laborValue = Map.Values.Get<Labor>();
    //        foodValue = Map.Values.Get<Food>();
    //    }

    //    public override void OnConstruct() {
    //    }

    //    public override void OnDestruct() {
    //    }

    //    public override void OnTap() {
    //        string foodColoredName = Concept.Ins.ColoredNameOf<Food>();
    //        string grasslandColoredName = Concept.Ins.ColoredNameOf<Grassland>();
    //        string baseColoredName = Concept.Ins.ColoredNameOf<Base>();
    //        if (Map.Values.Get<BaseCount>().Max == 0) {
    //            UI.Ins.UIItems(grasslandColoredName, new List<IUIItem> {
    //                new UIItem {
    //                    Type = IUIItemType.MultilineText,
    //                    Content = "一片" + grasslandColoredName+"，适合建立" + baseColoredName,
    //                },
    //                new UIItem {
    //                    Type = IUIItemType.Button,
    //                    Content = "建立" + baseColoredName,
    //                    OnTap = () => {
    //                        Map.UpdateAt<Base>(Pos);
    //                        UI.Ins.Active = false;
    //                    },
    //                },
    //            }) ;
    //        } else {
    //            IValue mapFood = Map.Values.Get<Food>();
    //            UI.Ins.UIItems(grasslandColoredName, new List<IUIItem> {
    //                new UIItem {
    //                    Type = IUIItemType.MultilineText,
    //                    Content = $"一片{grasslandColoredName}，气候适宜",
    //                },
    //                new UIItem {
    //                    Type = IUIItemType.Image,
    //                    Content = "GrasslandBanner",
    //                    LeftPadding = 0,
    //                },
    //                new UIItem {
    //                    Type = IUIItemType.Button,
    //                    Content = $"采集浆果。{Concept.Ins.Val<Labor>(-gatherFruitLaborCost)}{Concept.Ins.Val<Food>(gatherFruitFoodRevenue)}",
    //                    OnTap = GatherFruit,
    //                    CanTap = CanGatherFruit,
    //                },
    //                new UIItem {
    //                    Type = IUIItemType.Button,
    //                    Content = "播种浆果" + BerryBush.ConsturctionDescription,
    //                    OnTap = () => {
    //                        if (Map.UpdateAt<BerryBush>(Pos)) {
    //                            Map.Get(Pos).OnTap();
    //                        }
    //                    },
    //                    CanTap = () => BerryBush.CanBeBuiltOn(Map, Pos),
    //                },
    //                new UIItem {
    //                    Type = IUIItemType.Button,
    //                    Content = "建造小屋" + Residence.ConsturctionDescription,
    //                    OnTap = () => {
    //                        if (Map.UpdateAt<Residence>(Pos)) {
    //                            Map.Get(Pos).OnTap();
    //                        }
    //                    },
    //                    CanTap = () => Residence.CanBeBuiltOn(Map, Pos),
    //                },
    //                new UIItem {
    //                    Content = Concept.Ins.ColoredNameOf<Labor>(),
    //                    Type = IUIItemType.ValueProgress,
    //                    Value = Map.Values.Get<Labor>()
    //                },
    //                new UIItem {
    //                    Content = foodColoredName,
    //                    Type = IUIItemType.ValueProgress,
    //                    Value = Map.Values.Get<Food>()
    //                },
    //            });
    //        }
    //    }

    //    private readonly long gatherFruitLaborCost = 1;
    //    private readonly long gatherFruitFoodRevenue = 1;
    //    private void GatherFruit() {
    //        foodValue.Val += gatherFruitFoodRevenue;
    //        laborValue.Val -= gatherFruitLaborCost;
    //    }

    //    private bool CanGatherFruit() {
    //        return laborValue.Val >= gatherFruitLaborCost;
    //    }

    //}
}

