
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class SeaToGrassland : StandardTile
    {
        public override string SpriteKey => typeof(SeaToGrassland).Name;

        private IValue progress;
        public override void OnConstruct() {
            base.OnConstruct();

            Values = Weathering.Values.GetOne();

            progress = Values.Create<ProductionProgress>();
            progress.Max = 10000;
            progress.Del = 1 * Value.Second;

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 10;
            Inventory.TypeCapacity = 10;
        }

        public override void OnEnable() {
            base.OnEnable();
            progress = Values.Get<ProductionProgress>();
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            InventoryQuery inventoryQuery = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = 5, Type = typeof(FoodSupply), Source = Map.Inventory, Target = Inventory }
                , new InventoryQueryItem { Quantity = 5, Type = typeof(WoodSupply), Source = Map.Inventory, Target = Inventory }
                );
            InventoryQuery inventoryQueryInversed = inventoryQuery.CreateInversed();


            if (progress.Inc == 0) {
                items.Add(UIItem.CreateDestructButton<Sea>(this));
            } else {
                items.Add(UIItem.CreateText("填海造陆，围湖造田"));
            }

            items.Add(UIItem.CreateValueProgress<ProductionProgress>(progress));
            items.Add(UIItem.CreateTimeProgress<ProductionProgress>(progress));

            items.Add(UIItem.CreateButton($"验收完成：填海造陆", () => {
                Map.UpdateAt<Forest>(Pos);
                Map.Get(Pos).OnTap();
            }, () => progress.Maxed));

            items.Add(UIItem.CreateSeparator());

            if (progress.Inc == 0) {
                items.Add(UIItem.CreateButton($"开始填海造陆{inventoryQuery.GetDescription()}", () => {
                    inventoryQuery.TryDo(() => {
                        progress.Inc = 1;
                    });
                }));
            } else {
                items.Add(UIItem.CreateButton($"停止填海造陆{inventoryQueryInversed.GetDescription()}", () => {
                    inventoryQueryInversed.TryDo(() => {
                        progress.Inc = 0;
                    });
                }));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateText("填海造陆工程占用了以下资源供给："));
                UIItem.AddEntireInventoryContentWithTag<FoodSupply>(Inventory, items, OnTap);
                UIItem.AddEntireInventoryContentWithTag<WoodSupply>(Inventory, items, OnTap);
            }

            UI.Ins.ShowItems(Localization.Ins.Get<SeaToGrassland>(), items);
        }
    }
}

