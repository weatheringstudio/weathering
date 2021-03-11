

using System;
using System.Collections.Generic;

namespace Weathering
{
    public abstract class Factory1Out : StandardTile, ILinkProvider
    {
        public abstract override string SpriteKey { get; }

        private IValue worker;
        private IRef outRef;

        public void Provide(List<IRef> refs) {
            refs.Add(outRef);
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            outRef = Refs.Create<Factory1Out>();
            outRef.Type = Type;

            Values = Weathering.Values.GetOne();
            worker = Values.Create<Factory1Out>();

            if (CanSendWorker()) SendWorker();
        }

        public override void OnEnable() {
            base.OnEnable();
            worker = Values.Get<Factory1Out>();
            outRef = Refs.Get<Factory1Out>();
        }
        protected abstract Type Type { get; }
        protected abstract long WorkerCost { get; }
        protected abstract long BaseValue { get; }
        protected bool Working => worker.Max != 0;

        private bool CanSendWorker() {
            return worker.Max == 0 && Map.Inventory.Get<Worker>() >= WorkerCost;
        }

        private void SendWorker() {
            Map.Inventory.Remove<Worker>(WorkerCost);
            outRef.Value += BaseValue;
            worker.Max += WorkerCost;
            NeedUpdateSpriteKeys = true;
        }

        public override void OnTap() {

            var items = new List<IUIItem>() { };

            items.Add(UIItem.CreateText($"工作人员 {Localization.Ins.Val<Worker>(worker.Max)}"));

            items.Add(UIItem.CreateButton("派遣工人", SendWorker, CanSendWorker));

            items.Add(UIItem.CreateButton("取消派遣", () => {
                if (Map.Inventory.CanAdd<Worker>() >= WorkerCost) {
                    Map.Inventory.Add<Worker>(WorkerCost);
                    outRef.Value -= BaseValue;
                    worker.Max -= WorkerCost;
                    NeedUpdateSpriteKeys = true;
                }
                OnTap();
            }, () => worker.Max == WorkerCost && outRef.Value == BaseValue));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => worker.Max == 0));
            
            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }
    }
}
