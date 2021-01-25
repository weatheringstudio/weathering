
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class HuntingGround : StandardTile
    {
        public override string SpriteKey => typeof(HuntingGround).Name;

        private IValue food;

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            food = Values.Create<Food>();
            food.Max = 100;
            food.Inc = 5;
            food.Del = 100 * Value.Second;
        }

        public override void OnEnable() {
            base.OnEnable();
            food = Values.Get<Food>();
        }

        public override void OnTap() {
            UI.Ins.ShowItems(TileName,
                UIItem.CreateText("正在等待兔子撞上树干"),
                UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Food>()}", GatherFood),
                UIItem.CreateValueProgress<Food>(Values),
                UIItem.CreateTimeProgress<Food>(Values),

                UIItem.CreateSeparator(),
                UIItem.CreateDestructButton<Forest>(this)
            );
        }

        private const long gatherFoodSanityCost = 1;
        private void GatherFood() {
            if (Globals.Sanity.Val < gatherFoodSanityCost) {
                UIPreset.ResourceInsufficient<Sanity>(OnTap, gatherFoodSanityCost, Globals.Sanity);
            }
            long canAdd = Map.Inventory.CanAdd<Food>();
            if (canAdd <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
            }

            Globals.Sanity.Val -= gatherFoodSanityCost;
            Map.Inventory.AddAsManyAsPossible<Food>(food);
        }
    }
}

