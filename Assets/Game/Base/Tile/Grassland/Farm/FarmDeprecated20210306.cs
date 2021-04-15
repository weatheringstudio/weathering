
//using System;
//using System.Collections.Generic;

//namespace Weathering
//{
//    [Concept]
//    public class FarmTech { }


//    [Concept]
//    public class Farm : StandardTile
//    {
//        public override string SpriteKey {
//            get {
//                if (level.Max == 2) {
//                    return producing;
//                } else if (level.Max == 1) {
//                    if (food.Maxed) {
//                        return TimeUtility.GetFrame(0.2f, 2) == 0 ? producing : notProducing;
//                    } else {
//                        return producing;
//                    }
//                }
//                return "Farm";
//            }
//        }
//        private string producing = "FarmGrowing";
//        private string notProducing = "FarmRipe";

//        private const long level0FoodInc = 1;
//        private const long level1FoodInc = 5;

//        private void GotoLevel(long i) {
//            if (i == 0) {
//                food.Del = 10 * Value.Second;
//                food.Max = 10;
//                food.Inc = level0FoodInc;
//            } else if (i == 1) {
//                food.Max = 1000;
//                food.Inc = level1FoodInc;
//                level.Max = i;
//                if (level.Max == 2) {
//                    food.Val = 0;
//                }
//            } else if (i == 2) {
//                food.Max = long.MaxValue;
//                food.Inc = 1;
//                level.Max = i;
//            }
//            level.Max = i;
//        }

//        private IValue food;
//        private IValue level;

//        public override void OnConstruct() {
//            Values = Weathering.Values.GetOne();
//            level = Values.Create<Level>();

//            food = Values.Create<Food>();
//            GotoLevel(0);
//            food.Del = 10 * Value.Second;

//            Inventory = Weathering.Inventory.GetOne();
//            Inventory.QuantityCapacity = 5;
//            Inventory.TypeCapacity = 5;
//        }

//        public override void OnEnable() {
//            base.OnEnable();
//            food = Values.Get<Food>();
//            level = Values.Get<Level>();
//        }

//        private const long techMax = 10;
//        private const long techInc = 1;

//        public override void OnTap() {

//            InventoryQuery sowCost = InventoryQuery.Create(OnTap, Map.Inventory
//                , new InventoryQueryItem { Quantity = level0FoodInc, Type = typeof(Worker), Source = Map.Inventory, Target = Inventory }
//                );
//            InventoryQuery sowRevenue = InventoryQuery.Create(OnTap, Map.Inventory
//                , new InventoryQueryItem { Quantity = level1FoodInc, Type = typeof(Food), Target = Map.Inventory }
//                );

//            InventoryQuery sowCostInvsersed = sowCost.CreateInversed();
//            InventoryQuery sowRevenueInversed = sowRevenue.CreateInversed();


//            var items = new List<IUIItem>() { };

//            if (level.Max == 0) {
//                items.Add(UIItem.CreateText("田里偶尔会长出一些能吃的东西"));
//                items.Add(UIItem.CreateValueProgress<Food>(Values));
//                items.Add(UIItem.CreateTimeProgress<Food>(Values));
//                items.Add(UIItem.CreateSeparator());

//                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}", GatherFood, () => food.Val > 0));
//                items.Add(UIItem.CreateButton($"派遣居民种田{sowCost.GetDescription()}", () => {

//                    sowCost.TryDo(() => {
//                        GotoLevel(1);
//                    });
//                }));

//                items.Add(UIItem.CreateDestructButton<Grassland>(this));

//            } else if (level.Max == 1) {
//                items.Add(UIItem.CreateText("居民在田里种了各种作物，土豆、韭菜、蒲公英"));
//                items.Add(UIItem.CreateValueProgress<Food>(Values));
//                items.Add(UIItem.CreateTimeProgress<Food>(Values));
//                items.Add(UIItem.CreateSeparator());

//                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}", GatherFood, () => food.Val > 0));
//                items.Add(UIItem.CreateButton($"取消居民种田{sowCostInvsersed.GetDescription()}", () => {

//                    sowCostInvsersed.TryDo(() => {
//                        GotoLevel(0);
//                    });
//                }));

//                items.Add(UIItem.CreateButton($"开始自动供应食物{sowRevenue.GetDescription()}", () => {
//                    sowRevenue.TryDo(() => {
//                        GotoLevel(2);

//                        IValue farmTech = Globals.Ins.Values.Get<FarmTech>();
//                        farmTech.Max += techMax;
//                        farmTech.Inc += techInc;
//                    });
//                }));
//                items.Add(UIItem.CreateDestructButton<Grassland>(this, () => false));
//            } else if (level.Max == 2) {
//                items.Add(UIItem.CreateText("成熟的食物在农田里源源不断地产出"));
//                items.Add(UIItem.CreateInventoryItem<Food>(Map.Inventory, OnTap));
//                items.Add(UIItem.CreateTimeProgress<Food>(food));
//                items.Add(UIItem.CreateSeparator());

//                IValue farmTech = Globals.Ins.Values.Get<FarmTech>();
//                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}", GatherFood, () => false));
//                items.Add(UIItem.CreateButton($"停止自动供应食物{sowRevenueInversed.GetDescription()}", () => {
//                    sowRevenueInversed.TryDo(() => {
//                        GotoLevel(1);

//                        farmTech.Max -= techMax;
//                        farmTech.Inc -= techInc;
//                    });
//                }));

//                items.Add(UIItem.CreateSeparator());
//                items.Add(UIItem.CreateValueProgress<FarmTech>(farmTech));
//                items.Add(UIItem.CreateTimeProgress<FarmTech>(farmTech));

//                Globals.Ins.Values.Get<FarmTech>().Max += techMax;
//            }


//            UI.Ins.ShowItems(Localization.Ins.Get<Farm>(), items);
//        }

//        private const long gatherFoodSanityCost = 1;
//        private void GatherFood() {
//            if (Globals.Sanity.Val < gatherFoodSanityCost) {
//                UIPreset.ResourceInsufficient<Sanity>(OnTap, gatherFoodSanityCost, Globals.Sanity);
//                return;
//            }
//            if (Map.Inventory.CanAdd<Food>() <= 0) {
//                UIPreset.InventoryFull(OnTap, Map.Inventory);
//                return;
//            }
//            Globals.Sanity.Val -= gatherFoodSanityCost;
//            Map.Inventory.AddFrom<Food>(food);
//        }
//    }
//}

