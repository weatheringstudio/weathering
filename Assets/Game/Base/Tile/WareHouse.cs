﻿
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class WareHouseResource { }

    public class WareHouse : StandardTile, ILinkConsumer, ILinkProvider, ILinkEvent
    {
        public override string SpriteKey => "StorageBuilding";
        public override bool HasDynamicSpriteAnimation => true;

        public override string SpriteLeft => GetSprite(Vector2Int.left, typeof(ILeft));
        public override string SpriteRight => GetSprite(Vector2Int.right, typeof(IRight));
        public override string SpriteUp => GetSprite(Vector2Int.up, typeof(IUp));
        public override string SpriteDown => GetSprite(Vector2Int.down, typeof(IDown));
        private string GetSprite(Vector2Int pos, Type direction) {
            IRefs refs = Map.Get(Pos - pos).Refs;
            if (refs == null) return null;
            if (refs.TryGet(direction, out IRef result)) return result.Value < 0 ? ConceptResource.Get(result.Type).Name : null;
            return null;
        }

        public void OnLink(Type direction, long quantity) {
            TypeOfResource.Type = ConceptResource.Get(RefOfSupply.Type);
            ValueOfResource.Inc = RefOfSupply.Value;
        }

        public IRef RefOfSupply { get; private set; } // 无法作为输入
        public IRef TypeOfResource { get; private set; }
        public void Consume(List<IRef> refs) {
            refs.Add(RefOfSupply);
        }

        public void Provide(List<IRef> refs) {
            refs.Add(RefOfSupply);
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();

            ValueOfResource = Values.Create<WareHouseResource>();
            ValueOfResource.Max = 1000;
            ValueOfResource.Del = Weathering.Value.Second;

            Refs = Weathering.Refs.GetOne();
            RefOfSupply = Refs.Create<WareHouse>();
            RefOfSupply.BaseValue = long.MaxValue;

            TypeOfResource = Refs.Create<WareHouseResource>();
        }

        private IValue ValueOfResource;

        public override void OnEnable() {
            base.OnEnable();
            RefOfSupply = Refs.Get<WareHouse>();
            ValueOfResource = Values.Get<WareHouseResource>();
            TypeOfResource = Refs.Get<WareHouseResource>();
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();



            if (TypeOfResource.Type != null) {
                items.Add(UIItem.CreateValueProgress(TypeOfResource.Type, ValueOfResource));
                items.Add(UIItem.CreateTimeProgress(TypeOfResource.Type, ValueOfResource));

                items.Add(UIItem.CreateDynamicContentButton(() => $"拿走{Localization.Ins.Val(TypeOfResource.Type, ValueOfResource.Val)}", CollectItems));

                items.Add(UIItem.CreateSeparator());
            }



            LinkUtility.AddButtons(items, this);

            if (TypeOfResource.Type != null) {
                items.Add(UIItem.CreateTileImage(TypeOfResource.Type));
            }

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => ValueOfResource.Val == 0 && !LinkUtility.HasAnyLink(this)));

            UI.Ins.ShowItems("仓库", items);
        }

        private void CollectItems() {
            long quantity = Math.Min(Map.Inventory.CanAdd(TypeOfResource.Type), ValueOfResource.Val);
            Map.Inventory.Add(TypeOfResource.Type, quantity);
            ValueOfResource.Val -= quantity;
            OnTap();
        }

    }
}

