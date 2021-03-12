

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

    public abstract class Factory : StandardTile, ILinkProvider, ILinkConsumer
    {
        public abstract override string SpriteKey { get; }

        private IValue worker; // 工人

        private IRef out0Ref; // 输出
        //private IRef out1Ref; // 输出
        //private IRef out2Ref; // 输出
        //private IRef out3Ref; // 输出

        private IRef in_0Ref; // 输入
        //private IRef in_1Ref; // 输入
        //private IRef in_2Ref; // 输入
        //private IRef in_3Ref; // 输入

        // protected virtual ((Type, long), (Type, long), (Type, long), (Type, long)) Out { get; }
        // protected virtual ((Type, long), (Type, long), (Type, long), (Type, long)) In_ { get; }

        protected virtual (Type, long) Out0 { get; } = (null, 0);
        //protected virtual (Type, long) Out1 { get; } = (null, 0);
        //protected virtual (Type, long) Out2 { get; } = (null, 0);
        //protected virtual (Type, long) Out3 { get; } = (null, 0);

        protected virtual (Type, long) In_0 { get; } = (null, 0);
        //protected virtual (Type, long) In_1 { get; } = (null, 0);
        //protected virtual (Type, long) In_2 { get; } = (null, 0);
        //protected virtual (Type, long) In_3 { get; } = (null, 0);


        public void Provide(List<IRef> refs) {
            refs.Add(out0Ref);
        }

        public void Consume(List<IRef> refs) {
            refs.Add(in_0Ref);
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();

            if (In_0.Item1 != null) {
                in_0Ref = Refs.Create<FactoryIn_0>();
                in_0Ref.Type = In_0.Item1;
                in_0Ref.BaseValue = In_0.Item2;
            }
            if (Out0.Item1 != null) {
                out0Ref = Refs.Create<FactoryOut0>();
                out0Ref.Type = Out0.Item1;
            }


            Values = Weathering.Values.GetOne();
            worker = Values.Create<Factory>();

            if (CanSendWorker()) SendWorker(); // 自动派遣工人
        }

        public override void OnEnable() {
            base.OnEnable();
            worker = Values.Get<Factory>();

            if (In_0.Item1 != null) {
                in_0Ref = Refs.Get<FactoryIn_0>();
            }
            if (Out0.Item1 != null) {
                out0Ref = Refs.Get<FactoryOut0>();
            }
        }

        protected abstract long WorkerCost { get; }
        protected bool Working => worker.Max != 0;

        private bool CanSendWorker() => worker.Max == 0 && Map.Inventory.Get<Worker>() >= WorkerCost;

        private void SendWorker() {
            Map.Inventory.Remove<Worker>(WorkerCost);
            worker.Max += WorkerCost;
            NeedUpdateSpriteKeys = true;
        }

        private bool CanRun() {
            // 如果有工人和所有原材料，那么制造输出。
            if (worker.Max != WorkerCost) return false;
            return true;
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
            }, () => worker.Max == WorkerCost && out0Ref.Value == Out0.Item2));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => worker.Max == 0));

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }
    }
}
