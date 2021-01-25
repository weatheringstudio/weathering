
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Village : StandardTile
    {
        public override string SpriteKey => "Residence";

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            foodSupply = Values.Create<FoodSupply>();
            worker = Values.Create<Worker>();
        }

        private IValue foodSupply;
        private IValue worker;
        public override void OnEnable() {
            base.OnEnable();

            foodSupply = Values.Get<FoodSupply>();
            worker = Values.Get<Worker>();
        }

        public override void OnTap() {
            string title = "村庄";
            var items = new List<IUIItem>();

            items.Add(UIItem.CreateDecIncMaxText<FoodSupply>(foodSupply));
            items.Add(UIItem.CreateDecIncMaxText<Worker>(worker));

            if (worker.Max == 0) {
                // worker.max就是房子数目
                title = "没房子的村庄";
                items.Add(UIItem.CreateText("村庄里连个房子都没有，人肯定不会来"));
                items.Add(BuildHutButton());
            } else if (worker.Inc == 0) {
                title = "没人的村庄";
                items.Add(UIItem.CreateText("村里有房子了，但还没人，人需要食物才愿意来"));

                items.Add(UIItem.CreateButton("为村民提供食物", () => {
                    const long foodSupplyCost = 1;
                    if (Map.Inventory.Get<FoodSupply>() < foodSupplyCost) { // 以后会检测资源，可以用子类资源替代
                        UIPreset.ResourceInsufficient<FoodSupply>(OnTap, foodSupplyCost, Map.Inventory);
                        return;
                    }
                    worker.Max += 1;
                    Map.Get(Pos).OnTap();
                }));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateInventoryItem<FoodSupply>(Map.Inventory, OnTap));
            }

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateDestructButton<Grassland>(this));
            UI.Ins.ShowItems(title, items);
        }
        private bool CanFindFoodOn(Vector2Int pos) {
            ITile tile = Map.Get(Pos + Vector2Int.up);
            return tile is Forest || tile is Grassland;
        }

        private UIItem BuildHutButton() {
            return UIItem.CreateButton("建造一座小屋。需要10木材", () => {
                const long hutWoodCost = 10;
                if (Map.Inventory.Get<Wood>() < hutWoodCost) { // 以后会检测资源，可以用子类资源替代
                    UIPreset.ResourceInsufficient<Wood>(OnTap, hutWoodCost, Map.Inventory);
                    return;
                }
                worker.Max += 1;
                foodSupply.Max += 2;
                Map.Get(Pos).OnTap();
            });
        }


    }
}

