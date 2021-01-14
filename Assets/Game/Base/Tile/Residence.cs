
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("住房", "AB3600")]
    public class Residence : ITileDefinition
    {
        public string SpriteKey => typeof(Residence).Name;

        public IValues Values { get; private set; } = null;
        public void SetValues(IValues values) => Values = values;

        public void OnTap() {
            string coloredName = Concept.Ins.ColoredNameOf<Residence>();
            string foodColoredName = Concept.Ins.ColoredNameOf<Food>();
            string laborColoredName = Concept.Ins.ColoredNameOf<Labor>();

            IValue level = Values.Get<Level>();

            UI.Ins.UIItems(coloredName, new List<IUIItem>() {
                new UIItem {
                    Content = $"吃着{foodColoredName}，增加{laborColoredName}。每级{Concept.Ins.Inc<Labor>(1)}{Concept.Ins.Inc<Food>(-2)}",
                    Type = IUIItemType.MultilineText,
                },
                new UIItem {
                    Type = IUIItemType.OnelineDynamicText,
                    DynamicContent = () => {
                        return "当前入住人数：" + Level.ToString() + (Level==0 ? "" : $"{Concept.Ins.Inc<Labor>(1*Level)}{Concept.Ins.Inc<Food>(-2*Level)}");
                    }
                },

                new UIItem {
                    Content = $"请人入住。{Concept.Ins.Val<Food>(-100)}{Concept.Ins.Inc<Labor>(LaborIncRevenuePerLevel)}",
                    Type = IUIItemType.Button,
                    OnTap = () => {
                        Level++;
                        mapFood.Dec += FoodIncCostPerLevel;
                        mapLabor.Inc += LaborIncRevenuePerLevel;
                        mapLabor.Max += LaborMaxPerLevel;
                    },
                    CanTap = () => {
                        if (mapFood.Val < FoodValCostLevelingUp) return false;
                        return mapFood.Inc >= FoodIncCostPerLevel;
                    }
                },
                 new UIItem {
                    Content = $"请人离开。{Concept.Ins.Inc<Labor>(-LaborIncRevenuePerLevel)}",
                    Type = IUIItemType.Button,
                    OnTap = () => {
                        Level--;
                        mapFood.Val -= FoodValCostLevelingUp;
                        mapFood.Dec -= FoodIncCostPerLevel;
                        mapLabor.Inc -= LaborIncRevenuePerLevel;
                        mapLabor.Max += LaborMaxPerLevel;
                    },
                    CanTap = () => {
                        if (Level == 0) return false;
                        if (mapLabor.Max < LaborMaxPerLevel) return false;
                        return mapLabor.Inc >= LaborIncRevenuePerLevel;
                    }
                },

                new UIItem {
                    Content = laborColoredName,
                    Type = IUIItemType.ValueProgress,
                    Value = mapLabor
                },
                new UIItem {
                    Content = laborColoredName,
                    Type = IUIItemType.TimeProgress,
                    Value = mapLabor
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
                    Content = $"{Concept.Ins.ColoredNameOf<Destruct>()}{coloredName}",
                    Type = IUIItemType.Button,
                    OnTap = () => {
                        Map.UpdateAt<Grassland>(Pos);
                        UI.Ins.Active = false;
                    }
                },
            }); ;
        }

        public static string ConsturctionDescription {
            get {
                return $"{ Concept.Ins.Val<Labor>(-LaborValCost) }{Concept.Ins.Val<Residence>(1)}";
            }
        }
        public static bool CanBeBuiltOn(IMap map, Vector2Int vec) {
            return map.Values.Get<Labor>().Val >= LaborValCost;
        }

        public bool CanConstruct() {
            return true;
        }

        public bool CanDestruct() {
            return Map.Values.Get<Labor>().Inc >= Level;
        }

        public IMap Map { get; set; }
        public UnityEngine.Vector2Int Pos { get; set; }
        private IValue mapLabor;
        private IValue mapFood;
        private IValue mapLevel;
        public const long LaborValCost = 100;
        public const long LaborIncRevenuePerLevel = 1;
        public const long FoodIncCostPerLevel = 2;
        public const long LaborMaxPerLevel = 100;
        public const long FoodValCostLevelingUp = 10;
        public void Initialize() {
            if (Values == null) {
                Values = Weathering.Values.Create();
            }
            mapLabor = Map.Values.Get<Labor>();
            mapFood = Map.Values.Get<Food>();
            mapLevel = Values.Get<Level>();
        }

        public void OnConstruct() {
        }

        public void OnDestruct() {
        }

        private long Level { get => mapLevel.Max; set => mapLevel.Max = value; }

    }
}

