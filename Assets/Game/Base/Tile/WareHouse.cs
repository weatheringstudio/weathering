
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class WareHouseResource { }

    public class WareHouse : StandardTile, ILinkConsumer, ILinkEvent
    {
        public override string SpriteKey => "StorageBuilding";
        public override bool HasDynamicSpriteAnimation => true;

        public override string SpriteLeft => Refs.Has<IRight>() && Refs.Get<IRight>().Value > 0 ? RefOfResource.Type.Name : null;
        public override string SpriteRight => Refs.Has<ILeft>() && Refs.Get<ILeft>().Value > 0 ? RefOfResource.Type.Name : null;
        public override string SpriteUp => Refs.Has<IDown>() && Refs.Get<IDown>().Value > 0 ? RefOfResource.Type.Name : null;
        public override string SpriteDown => Refs.Has<IUp>() && Refs.Get<IUp>().Value > 0 ? RefOfResource.Type.Name : null;

        public void OnLink(Type direction, long quantity) {
            if (RefOfSupply.Value == 0) { }
            RefOfResource.Type = ConceptResource.Get(RefOfSupply.Type);
            ValueOfResource.Inc = RefOfSupply.Value;
        }

        public IRef RefOfSupply { get; private set; } // 无法作为输入
        public IRef RefOfResource { get; private set; }
        public void Consume(List<IRef> refs) {
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

            RefOfResource = Refs.Create<WareHouseResource>();
        }

        private IValue ValueOfResource;

        public override void OnEnable() {
            base.OnEnable();
            RefOfSupply = Refs.Get<WareHouse>();
            ValueOfResource = Values.Get<WareHouseResource>();
            RefOfResource = Refs.Get<WareHouseResource>();
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            if (RefOfResource.Type != null) {
                items.Add(UIItem.CreateValueProgress(RefOfResource.Type, ValueOfResource));
                items.Add(UIItem.CreateTimeProgress(RefOfResource.Type, ValueOfResource));

                items.Add(UIItem.CreateDynamicButton(() => $"拿走{Localization.Ins.Val(RefOfResource.Type, ValueOfResource.Val)}", CollectItems));

                items.Add(UIItem.CreateSeparator());
            }

            LinkUtility.AddButtons(items, this);

            if (ValueOfResource.Val == 0) {
                items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));
            }

            UI.Ins.ShowItems("仓库", items);
        }

        private void CollectItems() {
            long quantity = Math.Min(Map.Inventory.CanAdd(RefOfResource.Type), ValueOfResource.Val);
            Map.Inventory.Add(RefOfResource.Type, quantity);
            ValueOfResource.Val -= quantity;
        }
    }
}

