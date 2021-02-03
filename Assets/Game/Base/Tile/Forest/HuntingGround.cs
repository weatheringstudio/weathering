
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
                if (level.Max == 2) {
                    return producing;
                } else if (level.Max == 1) {
                    if (meat.Maxed) {
                        return TimeUtility.GetFrame(0.2f, 2) == 0 ? producing : notProducing;
                    } else {
                        return producing;
                    }
                }
                return producing;
            }
        }
        private string producing = typeof(HuntingGround).Name + "Producing";
        private string notProducing = typeof(HuntingGround).Name;


        private IValue meat;
        private IValue level;
        private const long foodInc = 1;
        private const long foodMax = 10;
        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            meat = Values.Create<Meat>();
            meat.Max = foodMax;
            meat.Inc = foodInc;
            meat.Del = 10 * Value.Second;

            level = Values.Create<Level>();
            level.Max = 1;
        }

        public override void OnEnable() {
            base.OnEnable();
            meat = Values.Get<Meat>();
            level = Values.Get<Level>();
        }

        public override void OnTap() {
            var inventoryQuery = InventoryQuery.Create(OnTap, Map.Inventory, new List<InventoryQueryItem> {
                new InventoryQueryItem {Target = Map.Inventory, Quantity = 1, Type = typeof(MeatSupply)}
            });
            var inventoryQueryInversed = inventoryQuery.CreateInversed();

            if (level.Max == 1) {
                UI.Ins.ShowItems(string.Format(Localization.Ins.Get<StateOfProducing>(), Localization.Ins.Get<HuntingGround>()),
                    UIItem.CreateText("正在等待兔子撞上树干"),
                    UIItem.CreateValueProgress<Meat>(Values),
                    UIItem.CreateTimeProgress<Meat>(Values),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Meat>()}", GatherFood, () => meat.Val > 0),
                    UIItem.CreateButton($"按时捡走兔子{inventoryQuery.GetDescription()}", () => {
                        inventoryQuery.TryDo(() => {
                            meat.Max = long.MaxValue;
                            level.Max = 2;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateDestructButton<Forest>(this)
                );
            }
            else if (level.Max == 2) {
                UI.Ins.ShowItems(string.Format(Localization.Ins.Get<StateOfAutomated>(), Localization.Ins.Get<HuntingGround>())
                    , UIItem.CreateText("森林里每天都有兔子撞上树干，提供了稳定的食物供给")
                    , UIItem.CreateInventoryItem<MeatSupply>(Map.Inventory, OnTap)
                    , UIItem.CreateTimeProgress<Meat>(Values)

                    , UIItem.CreateSeparator()
                    , UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Meat>()}", GatherFood, () => false)
                    , UIItem.CreateButton($"不再按时捡走兔子{inventoryQueryInversed.GetDescription()}", () => {
                        inventoryQueryInversed.TryDo(() => {
                            meat.Max = foodMax;
                            meat.Val = 0;
                            level.Max = 1;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateDestructButton<Forest>(this, () => false)
                );
            }
            else {
                throw new Exception();
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
            Map.Inventory.AddFrom<Meat>(meat);
        }
    }
}

