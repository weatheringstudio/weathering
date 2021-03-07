

using System;
using System.Collections.Generic;

namespace Weathering
{
    public abstract class Factory1Out : StandardTile, ILinkProvider
    {
        public abstract override string SpriteKey { get; }

        private IValue popValue;
        private IRef resourceRef;

        public void Provide(List<IRef> refs) {
            refs.Add(resourceRef);
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            resourceRef = Refs.Create<Factory1Out>();
            resourceRef.Type = Type;

            Values = Weathering.Values.GetOne();
            popValue = Values.Create<Factory1Out>();
        }

        public override void OnEnable() {
            base.OnEnable();
            popValue = Values.Get<Factory1Out>();
            resourceRef = Refs.Get<Factory1Out>();
        }
        protected abstract Type Type { get; }
        protected abstract long WorkerCost { get; }
        protected abstract long BaseValue { get; }
        protected bool Working => popValue.Max != 0;
        public override void OnTap() {

            var items = new List<IUIItem>() { };

            items.Add(UIItem.CreateText($"工作人员 {Localization.Ins.Val<Worker>(popValue.Max)}"));

            items.Add(UIItem.CreateButton("派遣工人", () => {
                if (Map.Inventory.CanRemove<Worker>() >= WorkerCost) {
                    Map.Inventory.Remove<Worker>(WorkerCost);
                    resourceRef.Value += BaseValue;
                    popValue.Max += WorkerCost;
                    NeedUpdateSpriteKeys = true;
                }
                OnTap();
            }, () => popValue.Max == 0 && Map.Inventory.Get<Worker>() >= WorkerCost));

            items.Add(UIItem.CreateButton("取消派遣", () => {
                if (Map.Inventory.CanAdd<Worker>() >= WorkerCost) {
                    Map.Inventory.Add<Worker>(WorkerCost);
                    resourceRef.Value -= BaseValue;
                    popValue.Max -= WorkerCost;
                    NeedUpdateSpriteKeys = true;
                }
                OnTap();
            }, () => popValue.Max == WorkerCost && resourceRef.Value == BaseValue));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            if (popValue.Max == 0) {
                items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));
            }

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }
    }
}
