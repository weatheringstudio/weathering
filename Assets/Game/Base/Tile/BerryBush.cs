﻿
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept("浆果丛", "D95763")]
    public class BerryBush : StandardTile
    {
        private readonly string SpriteKeyBase = typeof(BerryBush).Name;
        private readonly string SpriteKeyGrowing = typeof(BerryBush).Name + "Growing";
        public override string SpriteKey {
            get {
                if (Matured) {
                    return SpriteKeyBase;
                } else if (Values.Get<Growing>().Maxed) {
                    return Utility.GetFrame(0.1f, 2) == 0 ? SpriteKeyBase : SpriteKeyGrowing;
                } else {
                    return SpriteKeyGrowing;
                }
            }
        }

        private IValue mapFood;
        private IValue mapLabor;
        public override void OnEnable() {
            if (Values == null) {
                Values = Weathering.Values.Create();
            }
            mapFood = Map.Values.Get<Food>();
            mapLabor = Map.Values.Get<Labor>();
        }
        public override void OnTap() {
            if (!Matured) {
                string berrybushColoredName = Concept.Ins.ColoredNameOf<BerryBush>();
                UI.Ins.UIItems(berrybushColoredName, new List<IUIItem>() {
                    new UIItem {
                        Content = Values.Get<Growing>().Maxed ? $"种下的{berrybushColoredName}已经成熟" :$"种下的{berrybushColoredName}正在茁壮生长",
                        Type = IUIItemType.MultilineText,

                    },
                    new UIItem {
                        Content = "生长中",
                        Type = IUIItemType.TimeProgress,
                        Value = Values.Get<Growing>()
                    },
                    new UIItem {
                        Content = $"收获",
                        Type = IUIItemType.Button,
                        OnTap = Mature,
                        CanTap = Values.Get<Growing>().IsMaxed
                    },
                    new UIItem {
                        Content = $"{Concept.Ins.ColoredNameOf<Destruct>()}{berrybushColoredName}",
                        Type = IUIItemType.Button,
                        OnTap = () => Map.UpdateAt<Grassland>(Pos)
                    },
                });
            } else {
                string berrybushColoredName = Concept.Ins.ColoredNameOf<BerryBush>();
                string foodColoredName = Concept.Ins.ColoredNameOf<Food>();
                UI.Ins.UIItems(berrybushColoredName, new List<IUIItem>() {
                    new UIItem {
                        Content = $"一大片{berrybushColoredName}，持续生产着{Concept.Ins.Inc<Food>(FoodIncRevenue) }",
                        Type = IUIItemType.MultilineText,
                    },
                    new UIItem {
                        Content = foodColoredName,
                        Type = IUIItemType.ValueProgress,
                        Value = mapFood
                    },
                    new UIItem {
                        Content = foodColoredName,
                        Type = IUIItemType.TimeProgress,
                        Value = mapFood
                    },
                    new UIItem {
                        Content = $"{Concept.Ins.ColoredNameOf<Destruct>()}{berrybushColoredName}",
                        Type = IUIItemType.Button,
                        OnTap = () => Map.UpdateAt<Grassland>(Pos)
                    },
                });
            }

        }

        public static string ConsturctionDescription {
            get {
                return $"{ Concept.Ins.Val<Labor>(-LaborValCost) }{ Concept.Ins.Val<Food>(-FoodValCost) }{Concept.Ins.Val<BerryBush>(1)}";
            }
        }
        public static bool CanBeBuiltOn(IMap map, UnityEngine.Vector2Int vec) {
            return map.Values.Get<Food>().Val >= FoodValCost && map.Values.Get<Labor>().Val >= LaborValCost;
        }


        public override bool CanConstruct() {
            return Map.Values.Get<Food>().Val >= FoodValCost
                && Map.Values.Get<Labor>().Val >= LaborValCost;
        }


        public const long FoodValCost = 10;
        public const long LaborValCost = 10;
        public const long FoodIncRevenue = 1;
        public const long BerryBushGrowingTimeInSeconds = 60;

        public override bool CanDestruct() {
            return Map.Values.Get<Food>().Inc >= FoodIncRevenue;
        }

        public override void OnConstruct() {
            mapFood.Val -= FoodValCost;
            mapLabor.Val -= LaborValCost;

            IValue growing = Values.Get<Growing>();
            growing.Del = BerryBushGrowingTimeInSeconds * Value.Second;
            growing.Inc = 1;
            growing.Max = 1;
        }

        private bool Matured {
            get { return Values.Get<Growing>().Max == 0; }
            set { Values.Get<Growing>().Max = 0; } // only mature once 
        }
        private void Mature() {
            mapFood.Inc += FoodIncRevenue;
            Matured = true;
            OnTap();
        }

        public override void OnDestruct() {
            if (Matured) {
                mapFood.Inc -= FoodIncRevenue;
            }
        }

    }
}

