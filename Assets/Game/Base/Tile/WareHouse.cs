
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

        public override string SpriteLeft => Refs.Has<IRight>() && Refs.Get<IRight>().Value > 0 ? TypeOfResource.Type.Name : null;
        public override string SpriteRight => Refs.Has<ILeft>() && Refs.Get<ILeft>().Value > 0 ? TypeOfResource.Type.Name : null;
        public override string SpriteUp => Refs.Has<IDown>() && Refs.Get<IDown>().Value > 0 ? TypeOfResource.Type.Name : null;
        public override string SpriteDown => Refs.Has<IUp>() && Refs.Get<IUp>().Value > 0 ? TypeOfResource.Type.Name : null;

        public void OnLink(Type direction, long quantity) {
            if (RefOfSupply.Value == 0) { }
            TypeOfResource.Type = ConceptResource.Get(RefOfSupply.Type);
            ValueOfResource.Inc = RefOfSupply.Value;
        }

        public IRef RefOfSupply { get; private set; } // 无法作为输入
        public IRef TypeOfResource { get; private set; }
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

                items.Add(UIItem.CreateDynamicButton(() => $"拿走{Localization.Ins.Val(TypeOfResource.Type, ValueOfResource.Val)}", CollectItems));

                items.Add(UIItem.CreateSeparator());
            }

            LinkUtility.AddButtons(items, this);

            if (ValueOfResource.Val == 0) {
                items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));
            }

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

