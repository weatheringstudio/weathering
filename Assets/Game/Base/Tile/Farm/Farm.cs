
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Farm : StandardTile
    {
        public override string SpriteKey {
            get {
                if (level.Max == 2) {
                    return producing;
                } else if (level.Max == 1) {
                    if (food.Maxed) {
                        return TimeUtility.GetFrame(0.2f, 2) == 0 ? producing : notProducing;
                    } else {
                        return producing;
                    }
                }
                return "Farm";
            }
        }
        private string producing = "FarmGrowing";
        private string notProducing = "FarmRipe";


        private const long foodInc = 3;
        private IValue food;
        private IValue level;

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            level = Values.Create<Level>();
            level.Max = -1;

            food = Values.Create<Food>();
            food.Max = 1000;
            food.Del = 10 * Value.Second;
            food.Inc = 0;

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 5;
            Inventory.TypeCapacity = 5;
        }

        public override void OnEnable() {
            base.OnEnable();
            food = Values.Get<Food>();
            level = Values.Get<Level>();
        }

        public override void OnTap() {

            InventoryQuery sowCost = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = 1, Type = typeof(Worker), Source = Map.Inventory, Target = Inventory }
                );
            InventoryQuery sowRevenue = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = foodInc, Type = typeof(FoodSupply), Target = Map.Inventory }
                );

            InventoryQuery sowCostInvsersed = sowCost.CreateInversed();
            InventoryQuery sowRevenueInversed = sowRevenue.CreateInversed();


            var items = new List<IUIItem>() { };

            if (level.Max == -1) {
                InventoryQuery build = InventoryQuery.Create(OnTap, Map.Inventory
                    , new InventoryQueryItem { Quantity = 10, Type = typeof(Food), Source = Map.Inventory });

                items.Add(UIItem.CreateText("田还没开垦"));
                items.Add(UIItem.CreateButton($"开垦{build.GetDescription()}", () => {
                    build.TryDo(() => {
                        level.Max = 0;
                    });
                }));

                Type grasslandType = typeof(Grassland);
                items.Add(UIItem.CreateConstructButton<GrainFarm>(this, grasslandType));
                items.Add(UIItem.CreateConstructButton<FlowerGarden>(this, grasslandType));
                items.Add(UIItem.CreateConstructButton<VegetableGarden>(this, grasslandType));
                items.Add(UIItem.CreateConstructButton<FruitGarden>(this, grasslandType));
                items.Add(UIItem.CreateConstructButton<Plantation>(this, grasslandType));

            } else if (level.Max == 0) {
                items.Add(UIItem.CreateButton($"派遣居民种田{sowCost.GetDescription()}", () => {
                    sowCost.TryDo(() => {
                        level.Max = 1;
                        food.Inc = foodInc;
                    });
                }));

            } else if (level.Max == 1) {
                items.Add(UIItem.CreateText("居民在田里种了各种作物，土豆、韭菜、蒲公英"));

                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}", GatherFood, () => food.Val > 0));
                items.Add(UIItem.CreateValueProgress<Food>(Values));
                items.Add(UIItem.CreateTimeProgress<Food>(Values));

                items.Add(UIItem.CreateSeparator());

                items.Add(UIItem.CreateButton($"取消居民种田{sowCostInvsersed.GetDescription()}", () => {
                    sowCostInvsersed.TryDo(() => {
                        level.Max = 0;
                    });
                }));

                items.Add(UIItem.CreateButton($"开始自动供应食物{sowRevenue.GetDescription()}", () => {
                    sowRevenue.TryDo(() => {
                        food.Inc = 0;
                        food.Val = 0;
                        level.Max = 2;
                    });
                }));
            }
            else if (level.Max == 2) {
                items.Add(UIItem.CreateText("成熟的食物在农田里源源不断地产出"));

                items.Add(UIItem.CreateButton($"停止自动供应食物{sowRevenueInversed.GetDescription()}", () => {
                    sowRevenueInversed.TryDo(() => {
                        food.Inc = foodInc;
                        level.Max = 1;
                    });
                }));
            }

            items.Add(UIItem.CreateDestructButton<Grassland>(this, () => level.Max <= 0));

            UI.Ins.ShowItems(Localization.Ins.Get<Farm>(), items);
        }

        private const long gatherFoodSanityCost = 1;
        private void GatherFood() {
            if (Globals.Sanity.Val < gatherFoodSanityCost) {
                UIPreset.ResourceInsufficient<Sanity>(OnTap, gatherFoodSanityCost, Globals.Sanity);
                return;
            }
            if (Map.Inventory.CanAdd<Food>() <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }
            Globals.Sanity.Val -= gatherFoodSanityCost;
            Map.Inventory.AddFrom<Food>(food);
        }
    }
}

