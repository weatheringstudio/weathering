
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class HuntingGround : StandardTile
    {
        public override string SpriteKey {
            get {
                if (!food.Maxed) {
                    return typeof(HuntingGround).Name + "Producing";
                }
                return typeof(HuntingGround).Name;
            }
        }

        private IValue food;

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            food = Values.Create<Food>();
            food.Max = 100;
            food.Inc = foodInc;
            food.Del = 100 * Value.Second;
        }
        private const long foodInc = 10;

        public override void OnEnable() {
            base.OnEnable();
            food = Values.Get<Food>();
        }

        private const long foodSupply = 1;
        public override void OnTap() {
            if (food.Inc != 0) {
                UI.Ins.ShowItems(TileName,
                    UIItem.CreateText("正在等待兔子撞上树干"),
                    UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}", GatherFood),
                    UIItem.CreateValueProgress<Food>(Values),
                    UIItem.CreateTimeProgress<Food>(Values),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"按时捡走兔子", () => {
                        if (Map.Inventory.CanAdd<FoodSupply>() < foodSupply) {
                            UIPreset.InventoryFull(OnTap, Map.Inventory);
                            return;
                        }
                        Map.Inventory.Add<FoodSupply>(foodSupply);
                        food.Inc = 0;
                        OnTap();
                    }),

                    UIItem.CreateSeparator(),
                    UIItem.CreateDestructButton<Forest>(this)
                );
            } else {
                UI.Ins.ShowItems(TileName,
                    UIItem.CreateText("森林里每天都有兔子撞上树干，提供了稳定的食物供给"),
                    UIItem.CreateText("获得了1食物供给"),
                    UIItem.ShowInventoryButton(OnTap, Map.Inventory),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"不再按时捡走兔子", () => {
                        if (Map.Inventory.Get<FoodSupply>() < foodSupply) {
                            UIPreset.ResourceInsufficient<FoodSupply>(OnTap, foodSupply, Map.Inventory);
                            return;
                        }
                        Map.Inventory.Remove<FoodSupply>(foodSupply);
                        food.Inc = foodInc;
                        OnTap();
                    }),

                    UIItem.CreateSeparator(),
                    UIItem.CreateDestructButton<Forest>(this)
                );
            }
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
            Map.Inventory.AddAsManyAsPossible<Food>(food);
        }

        public class ProducedFoodSupply { }

        private void ProduceFoodSupply() {

        }
    }
}

