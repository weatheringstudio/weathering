

using System;
using System.Collections.Generic;

namespace Weathering
{
    public class FactoryIn_0 { }
    public class FactoryIn_1 { }
    public class FactoryIn_2 { }
    public class FactoryIn_3 { }

    public class FactoryOut0 { }
    public class FactoryOut1 { }
    public class FactoryOut2 { }
    public class FactoryOut3 { }

    public abstract class Factory : StandardTile, ILinkProvider
    {
        public abstract override string SpriteKey { get; }

        private IValue worker; // 工人


        private IRef out0Ref; // 输出
        private IRef in_0Ref;

        protected abstract long Out0BaseValue { get; }
        protected abstract long In_0BaseValue { get; }

        protected virtual Type TypeOut0 { get; } = null;
        protected virtual Type TypeIn_0 { get; } = null;


        public void Provide(List<IRef> refs) {
            refs.Add(out0Ref);
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();

            out0Ref = Refs.Create<FactoryOut0>();
            in_0Ref = Refs.Create<FactoryIn_0>();

            out0Ref.Type = TypeOut0;
            in_0Ref.Type = TypeIn_0;

            Values = Weathering.Values.GetOne();
            worker = Values.Create<Factory>();

            if (CanSendWorker()) SendWorker();
        }

        public override void OnEnable() {
            base.OnEnable();
            worker = Values.Get<Factory>();
            in_0Ref = Refs.Get<FactoryIn_0>();
            out0Ref = Refs.Get<FactoryOut0>();
        }

        protected abstract long WorkerCost { get; }
        protected bool Working => worker.Max != 0;

        private bool CanSendWorker() => worker.Max == 0 && Map.Inventory.Get<Worker>() >= WorkerCost;

        private void SendWorker() {
            Map.Inventory.Remove<Worker>(WorkerCost);
            worker.Max += WorkerCost;
            NeedUpdateSpriteKeys = true;

            out0Ref.Value += Out0BaseValue;
        }

        private void TryRun() {
            // 如果有工人和所有原材料，那么制造输出。
            
        }

        public override void OnTap() {

            var items = new List<IUIItem>() { };

            items.Add(UIItem.CreateText($"工作人员 {Localization.Ins.Val<Worker>(worker.Max)}"));

            items.Add(UIItem.CreateButton($"开始运转{Localization.Ins.ValPlus<Worker>(-WorkerCost)}", SendWorker, CanSendWorker));

            items.Add(UIItem.CreateButton($"停止运转{Localization.Ins.ValPlus<Worker>(WorkerCost)}", () => {
                if (Map.Inventory.CanAdd<Worker>() >= WorkerCost) {
                    Map.Inventory.Add<Worker>(WorkerCost);
                    worker.Max -= WorkerCost;
                    NeedUpdateSpriteKeys = true;

                    // out0Ref.Value -= Out0BaseValue;
                }
                OnTap();
            }, () => worker.Max == WorkerCost && out0Ref.Value == Out0BaseValue));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => worker.Max == 0));

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }
    }
}
