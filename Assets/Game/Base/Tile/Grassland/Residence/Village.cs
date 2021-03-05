
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Village : StandardTile, ILinkConsumer
    {
        public override string SpriteKey => typeof(Village).Name;

        public override bool HasDynamicSpriteAnimation => true;
        public override string SpriteLeft => Refs.Has<IRight>() && Refs.Get<IRight>().Value > 0 ? ConceptResource.Get(foodRef.Type).Name : null;
        public override string SpriteRight => Refs.Has<ILeft>() && Refs.Get<ILeft>().Value > 0 ? ConceptResource.Get(foodRef.Type).Name : null;
        public override string SpriteUp => Refs.Has<IDown>() && Refs.Get<IDown>().Value > 0 ? ConceptResource.Get(foodRef.Type).Name : null;
        public override string SpriteDown => Refs.Has<IUp>() && Refs.Get<IUp>().Value > 0 ? ConceptResource.Get(foodRef.Type).Name : null;

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            foodRef = Refs.Create<FoodSupply>();
            foodRef.BaseValue = foodPerWorker * villagePopMax;
            foodRef.Value = 0;
            foodRef.Type = typeof(FoodSupply);

            Values = Weathering.Values.GetOne();
            popValue = Values.Create<Worker>();
        }

        private IRef foodRef;
        private IValue popValue;
        public override void OnEnable() {
            base.OnEnable();
            foodRef = Refs.Get<FoodSupply>();
            popValue = Values.Get<Worker>();
        }

        public void Consume(List<IRef> refs) {
            refs.Add(foodRef);
        }

        public const long foodPerWorker = 5;
        public const long villagePopMax = 3;

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            // items.Add(UIItem.CreateValueProgress<Worker>(popValue));


            items.Add(UIItem.CreateText($"睡觉的{Localization.Ins.Val<Worker>(popValue.Val)} 工作的{Localization.Ins.Val<Worker>(popValue.Max - popValue.Val)}"));

            items.Add(UIItem.CreateButton("居民入住", () => {
                long quantity = Math.Min(foodRef.Value / foodPerWorker, villagePopMax - popValue.Max);
                foodRef.Value -= quantity * foodPerWorker;
                popValue.Max += quantity;
                popValue.Val += quantity;
                OnTap();
            }, () => foodRef.Value > 0));

            items.Add(UIItem.CreateButton("居民离开", () => {
                long quantity = popValue.Val;
                popValue.Val -= quantity;
                popValue.Max -= quantity;
                foodRef.Value += quantity * foodPerWorker;
                OnTap();
            }, () => popValue.Val > 0));

            items.Add(UIItem.CreateButton("居民出门工作", () => {
                long quantity = Math.Min(Map.Inventory.CanAdd<Worker>(), popValue.Val);
                popValue.Val -= quantity;
                Map.Inventory.Add<Worker>(quantity);
                OnTap();
            }, () => popValue.Val > 0 && Map.Inventory.CanAdd<Worker>() > 0));

            items.Add(UIItem.CreateButton("居民回家睡觉", () => {
                long quantity = Math.Min(Map.Inventory.CanAdd<Worker>(), popValue.Max - popValue.Val);
                popValue.Val += quantity;
                Map.Inventory.Remove<Worker>(quantity);
                OnTap();
            }, () => (popValue.Max - popValue.Val > 0) && Map.Inventory.Get<Worker>() > 0));


            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            if (popValue.Max == 0 && foodRef.Value == 0) {
                items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));
            }

            items.Add(UIItem.CreateText($"每个村庄最多住{Localization.Ins.Val(foodRef.Type, villagePopMax)}"));
            items.Add(UIItem.CreateText($"每个居民消耗{Localization.Ins.Val(foodRef.Type, foodPerWorker)}"));

            UI.Ins.ShowItems("村庄", items);
        }
    }
}

