
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("住房", "FFC888")]
    public class Residence : StandardTile
    {
        public override string SpriteKey => typeof(Residence).Name;

        public override void OnTap() {
            string coloredName = Concept.Ins.ColoredNameOf<Residence>();
            string foodColoredName = Concept.Ins.ColoredNameOf<Food>();
            string laborColoredName = Concept.Ins.ColoredNameOf<Labor>();

            IValue workerCount = Values.Get<Worker>();

            UI.Ins.UIItems(coloredName, new List<IUIItem>() {
                new UIItem {
                    Content = $"每个居民{Concept.Ins.Inc<Labor>(1)}{Concept.Ins.Inc<Food>(-2)}",
                    Type = IUIItemType.MultilineText,
                },
                new UIItem {
                    Type = IUIItemType.OnelineDynamicText,
                    DynamicContent = () => {
                        return "最多住5人。当前入住人数：" + WorkerCount.ToString() + (WorkerCount==0 ? "" : $"{Concept.Ins.Inc<Labor>(1*WorkerCount)}{Concept.Ins.Inc<Food>(-2*WorkerCount)}");
                    }
                },

                new UIItem {
                    Content = $"提供食物，雇佣工人。{Concept.Ins.Val<Food>(-FoodValCostPerWorker)}{Concept.Ins.Inc<Labor>(LaborIncRevenuePerWorker)}",
                    Type = IUIItemType.Button,
                    OnTap = () => {
                        WorkerCount++;
                        mapFood.Val -= FoodValCostPerWorker;
                        mapFood.Dec += FoodIncCostPerWorker;
                        mapLabor.Inc += LaborIncRevenuePerWorker;
                        mapLabor.Max += LaborMaxPerWorker;
                    },
                    CanTap = () => {
                        if (mapFood.Val < FoodValCostPerWorker) return false;
                        if (workerCount.Max >= 5)return false;
                        return mapFood.Sur >= FoodIncCostPerWorker;
                    }
                },
                new UIItem {
                    Content = $"解雇工人。{Concept.Ins.Inc<Labor>(-LaborIncRevenuePerWorker)}",
                    Type = IUIItemType.Button,
                    OnTap = () => {
                        WorkerCount--;
                        mapFood.Val -= FoodValCostPerWorker;
                        mapFood.Dec -= FoodIncCostPerWorker;
                        mapLabor.Inc -= LaborIncRevenuePerWorker;
                        mapLabor.Max -= LaborMaxPerWorker;
                    },
                    CanTap = () => {
                        if (WorkerCount == 0) return false;
                        if (mapLabor.Max < LaborMaxPerWorker) return false;
                        return mapLabor.Sur >= LaborIncRevenuePerWorker;
                    }
                },
                new UIItem {
                    Content = "食物和人力",
                    Type = IUIItemType.MultilineText,
                },
                new UIItem {
                    Content = laborColoredName,
                    Type = IUIItemType.DelProgress,
                    Value = mapLabor
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
                    Type = IUIItemType.DelProgress,
                    Value = mapFood
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
                return $"{ Concept.Ins.Val<Labor>(-LaborValCostOnConstruction) }{ Concept.Ins.Val<Wood>(-WoodValCostOnConstruction) }{Concept.Ins.Val<Residence>(1)}";
            }
        }
        public static bool CanBeBuiltOn(IMap map, Vector2Int vec) {
            return map.Values.Get<Labor>().Val >= LaborValCostOnConstruction;
        }

        public override bool CanConstruct() {
            return true;
        }

        public override bool CanDestruct() {
            return Map.Values.Get<Labor>().Inc >= WorkerCount;
        }

        private IValue mapLabor;
        private IValue mapFood;
        private IValue mapLevel;
        private IValue mapWorkerCount;

        public const long LaborValCostOnConstruction = 100;
        public const long WoodValCostOnConstruction = 10;

        public const long LaborIncRevenuePerWorker = 1;
        public const long FoodIncCostPerWorker = 2;

        public const long LaborMaxPerWorker = 100;
        public const long FoodValCostPerWorker = 100;
        public override void OnEnable() {
            if (Values == null) {
                Values = Weathering.Values.Create();
            }
            mapLabor = Map.Values.Get<Labor>();
            mapFood = Map.Values.Get<Food>();
            mapLevel = Values.Get<Worker>();
            mapWorkerCount = Values.Get<Worker>();
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        private long WorkerCount { get => mapLevel.Max; set => mapLevel.Max = value; }

    }
}

