
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

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            meat = Values.Create<Meat>();
            meat.Max = foodMax;
            meat.Inc = foodInc;
            meat.Del = 100 * Value.Second;

            level = Values.Create<Level>();
        }
        private const long foodInc = 10;
        private const long foodMax = 100;

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

            if (level.Max == 0) {
                var build = InventoryQuery.Create(OnTap, Map.Inventory,
                    new InventoryQueryItem { Source = Map.Inventory, Quantity = 5, Type = typeof(Wood) }
                );

                UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>()
                    , UIItem.CreateButton($"建造猎场{build.GetDescription()}", () => {
                        build.TryDo(() => {
                            level.Max = 1;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateDestructButton<Forest>(this)
                );
            } else if (level.Max == 1) {
                UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>(),
                    UIItem.CreateText("正在等待兔子撞上树干"),

                    UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<Meat>()}", GatherFood, () => meat.Val > 0),
                    UIItem.CreateValueProgress<Meat>(Values),
                    UIItem.CreateTimeProgress<Meat>(Values),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"按时捡走兔子{inventoryQuery.GetDescription()}", () => {
                        inventoryQuery.TryDo(() => {
                            meat.Inc = 0;
                            meat.Max = 0;
                            level.Max = 2;
                        });
                    })
                    , UIItem.CreateSeparator()

                    , UIItem.CreateDestructButton<Forest>(this)

                );
            }
            else if (level.Max == 2) {
                UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>(),
                    UIItem.CreateText("森林里每天都有兔子撞上树干，提供了稳定的食物供给"),
                    UIItem.CreateButton($"不再按时捡走兔子{inventoryQueryInversed.GetDescription()}", () => {
                        inventoryQueryInversed.TryDo(() => {
                            meat.Inc = foodInc;
                            meat.Max = foodMax;
                            level.Max = 1;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateInventoryTitle()
                    , UIItem.CreateInventoryItem<Meat>(Map.Inventory, OnTap)
                    , UIItem.CreateInventoryCapacity(Map.Inventory)
                    , UIItem.CreateInventoryTypeCapacity(Map.Inventory)

                    , UIItem.CreateDestructButton<Forest>(this)
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

        public class ProducedFoodSupply { }

        private void ProduceFoodSupply() {

        }
    }
}

