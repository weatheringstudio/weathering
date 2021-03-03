
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Weathering
//{
//    public class Village : StandardTile
//    {
//        public override string SpriteKey => "Residence";

//        public override void OnConstruct() {
//            base.OnConstruct();
//            Values = Weathering.Values.GetOne();
//            foodSupply = Values.Create<FoodSupply>();
//            worker = Values.Create<Worker>();
//        }

//        private IValue foodSupply;
//        private IValue worker;
//        public override void OnEnable() {
//            base.OnEnable();

//            foodSupply = Values.Get<FoodSupply>();
//            worker = Values.Get<Worker>();
//        }

//        private const long foodSupplyCost = 2;
//        private const long workerRevenue = 1;

//        public override void OnTap() {
//            string title = "村庄";
//            var items = new List<IUIItem>();


//            items.Add(BuildHutButton());

//            items.Add(UIItem.CreateDynamicText(() => $"当前小屋数目：{worker.Max}"));

//            // items.Add(UIItem.CreateDecIncMaxText<FoodSupply>(foodSupply));
//            items.Add(UIItem.CreateSurProgress<FoodSupply>(foodSupply));

//            items.Add(UIItem.CreateButton("增加食材供应", () => {
//                if (Map.Inventory.Get<FoodSupply>() < foodSupplyCost) {
//                    UIPreset.ResourceInsufficient<FoodSupply>(OnTap, foodSupplyCost, Map.Inventory);
//                    return;
//                }
//                foodSupply.Inc += foodSupplyCost;
//                Map.Inventory.Remove<FoodSupply>(foodSupplyCost);
//            }, () => foodSupply.Inc < foodSupply.Max));

//            items.Add(UIItem.CreateButton("减少食材供应", () => {
//                if (Map.Inventory.CanAdd<FoodSupply>() < foodSupplyCost) {
//                    UIPreset.InventoryFull(OnTap, Map.Inventory);
//                    return;
//                }
//                foodSupply.Inc -= foodSupplyCost;
//                Map.Inventory.Add<FoodSupply>(foodSupplyCost);
//            }, () => foodSupply.Dec < foodSupply.Inc));

//            items.Add(UIItem.CreateDynamicText(() => $"当前居民数目：{worker.Inc}"));

//            items.Add(UIItem.CreateButton("吸引居民入住", () => {
//                foodSupply.Dec += foodSupplyCost;
//                worker.Inc += workerRevenue;
//            }, () => worker.Inc < worker.Max && foodSupply.Dec < foodSupply.Inc));

//            items.Add(UIItem.CreateButton("驱逐居民离开", () => {
//                foodSupply.Dec -= foodSupplyCost;
//                worker.Inc -= workerRevenue;
//            }, () => worker.Dec < worker.Inc));

//            // items.Add(UIItem.CreateDecIncMaxText<Worker>(worker));
//            items.Add(UIItem.CreateSurProgress<Worker>(worker));

//            items.Add((UIItem.CreateButton("带走一位居民", () => {
//                if (Map.Inventory.CanAdd<Worker>() == 0) {
//                    UIPreset.InventoryFull(OnTap, Map.Inventory);
//                    return;
//                }
//                worker.Dec++;
//                Map.Inventory.Add<Worker>(1);
//            }, () => worker.Dec < worker.Inc)));

//            items.Add((UIItem.CreateButton("送回一位居民", () => {
//                if (Map.Inventory.CanRemove<Worker>() == 0) {
//                    UIPreset.ResourceInsufficient<Worker>(OnTap, 1, Map.Inventory);
//                }
//                worker.Dec--;
//                Map.Inventory.Remove<Worker>(1);

//            }, () => 0 < worker.Dec)));


//            items.Add(UIItem.CreateSeparator());
//            items.Add(UIItem.CreateDestructButton<Grassland>(this, () => {
//                return worker.Inc == 0 && foodSupply.Inc == 0;
//            }));
//            UI.Ins.ShowItems(title, items);
//        }
//        private bool CanFindFoodOn(Vector2Int pos) {
//            ITile tile = Map.Get(Pos + Vector2Int.up);
//            return tile is Forest || tile is Grassland;
//        }

//        private UIItem BuildHutButton() {
//            return UIItem.CreateButton("建造一座小屋。需要10木材", () => {
//                const long hutWoodCost = 10;
//                if (Map.Inventory.Get<Wood>() < hutWoodCost) { // 以后会检测资源，可以用子类资源替代
//                    UIPreset.ResourceInsufficient<Wood>(OnTap, hutWoodCost, Map.Inventory);
//                    return;
//                }
//                worker.Max += 1;
//                foodSupply.Max += 2;
//                Map.Get(Pos).OnTap();
//            });
//        }
//    }
//}

