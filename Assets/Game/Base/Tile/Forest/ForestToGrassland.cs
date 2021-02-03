
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class ForestToGrassland : StandardTile
    {
        public override string SpriteKey => typeof(GrasslandToForest).Name;


        private IValue progress;
        public override void OnConstruct() {
            base.OnConstruct();

            Values = Weathering.Values.GetOne();

            progress = Values.Create<ProductionProgress>();
            progress.Max = 1000;
            progress.Del = 1 * Value.Second;

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 4;
            Inventory.TypeCapacity = 4;
        }

        public override void OnEnable() {
            base.OnEnable();
            progress = Values.Get<ProductionProgress>();
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            InventoryQuery inventoryQuery = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = 1, Type = typeof(FoodSupply), Source = Map.Inventory, Target = Inventory }
                , new InventoryQueryItem { Quantity = 1, Type = typeof(WoodSupply), Source = Map.Inventory, Target = Inventory }
                );
            InventoryQuery inventoryQueryInversed = inventoryQuery.CreateInversed();


            if (progress.Inc == 0) {
                items.Add(UIItem.CreateDestructButton<Grassland>(this));
            } else {
                items.Add(UIItem.CreateText("刀耕火种"));
            }

            items.Add(UIItem.CreateValueProgress<ProductionProgress>(progress));
            items.Add(UIItem.CreateTimeProgress<ProductionProgress>(progress));

            items.Add(UIItem.CreateButton($"验收完成：移除树林", () => {
                Map.UpdateAt<Forest>(Pos);
                Map.Get(Pos).OnTap();
            }, () => progress.Maxed));

            items.Add(UIItem.CreateSeparator());

            if (progress.Inc == 0) {
                items.Add(UIItem.CreateButton($"开始移除树林{inventoryQuery.GetDescription()}", () => {
                    inventoryQuery.TryDo(() => {
                        progress.Inc = 1;
                    });
                }));
            } else {
                items.Add(UIItem.CreateButton($"停止移除树林{inventoryQueryInversed.GetDescription()}", () => {
                    inventoryQueryInversed.TryDo(() => {
                        progress.Inc = 0;
                    });
                }));

                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateText("植树造林工程占用了以下资源供给："));
                UIItem.AddEntireInventoryContentWithTag<FoodSupply>(Inventory, items, OnTap);
                UIItem.AddEntireInventoryContentWithTag<WoodSupply>(Inventory, items, OnTap);
            }

            UI.Ins.ShowItems(Localization.Ins.Get<ForestToGrassland>(), items);
        }
    }
}

