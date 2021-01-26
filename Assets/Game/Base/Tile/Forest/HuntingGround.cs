
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
                if (!food.Maxed || food.Max == 0) {
                    return typeof(HuntingGround).Name + "Producing";
                }
                return typeof(HuntingGround).Name;
            }
        }

        private IValue food;

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            food = Values.Create<Meat>();
            food.Max = foodMax;
            food.Inc = foodInc;
            food.Del = 100 * Value.Second;
        }
        private const long foodInc = 10;
        private const long foodMax = 100;

        public override void OnEnable() {
            base.OnEnable();
            food = Values.Get<Meat>();
        }

        private const long foodSupply = 1;
        public override void OnTap() {
            if (food.Inc != 0) {
                UI.Ins.ShowItems(TileName,
                    UIItem.CreateText("正在等待兔子撞上树干"),
                    UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Meat>()}", GatherFood, () => food.Val > 0),
                    UIItem.CreateValueProgress<Meat>(Values),
                    UIItem.CreateTimeProgress<Meat>(Values),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"按时捡走兔子", () => {
                        if (Map.Inventory.CanAdd<MeatSupply>() < foodSupply) {
                            UIPreset.InventoryFull(OnTap, Map.Inventory);
                            return;
                        }
                        Map.Inventory.Add<MeatSupply>(foodSupply);
                        food.Inc = 0;
                        food.Max = 0;
                        OnTap();
                    }),

                    UIItem.CreateSeparator(),
                    UIItem.CreateDestructButton<Forest>(this)
                );
            } else {
                UI.Ins.ShowItems(TileName,
                    UIItem.CreateText("森林里每天都有兔子撞上树干，提供了稳定的食物供给"),
                    UIItem.CreateText($"{Localization.Ins.Get<Gathered>()}{Localization.Ins.Val<MeatSupply>(foodSupply)}"),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"不再按时捡走兔子", () => {
                        if (Map.Inventory.Get<MeatSupply>() < foodSupply) {
                            UIPreset.ResourceInsufficient<MeatSupply>(OnTap, foodSupply, Map.Inventory);
                            return;
                        }
                        Map.Inventory.Remove<MeatSupply>(foodSupply);
                        food.Inc = foodInc;
                        food.Max = foodMax;
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
            if (Map.Inventory.CanAdd<Meat>() <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }

            Globals.Sanity.Val -= gatherFoodSanityCost;
            Map.Inventory.AddFrom<Meat>(food);
        }

        public class ProducedFoodSupply { }

        private void ProduceFoodSupply() {

        }
    }
}

