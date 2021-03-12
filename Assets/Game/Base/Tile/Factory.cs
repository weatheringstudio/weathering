

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

        // protected long WorkerCount => worker.Max;

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

        /// <summary>
        /// must be static
        /// </summary>
        protected virtual (Type, long) Out0 { get; } = (null, 0);
        //protected virtual (Type, long) Out1 { get; } = (null, 0);
        //protected virtual (Type, long) Out2 { get; } = (null, 0);
        //protected virtual (Type, long) Out3 { get; } = (null, 0);

        /// <summary>
        /// must be static
        /// </summary>
        protected virtual (Type, long) In_0 { get; } = (null, 0);
        //protected virtual (Type, long) In_1 { get; } = (null, 0);
        //protected virtual (Type, long) In_2 { get; } = (null, 0);
        //protected virtual (Type, long) In_3 { get; } = (null, 0);

        private bool HasIn_0 => In_0.Item1 != null;
        private bool HasOut0 => Out0.Item1 != null;

        public void Consume(List<IRef> refs) {
            if (HasIn_0) {
                refs.Add(in_0Ref);
            }
        }
        public void Provide(List<IRef> refs) {
            if (HasOut0) {
                refs.Add(out0Ref);
            }
        }


        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();

            if (HasIn_0) {
                in_0Ref = Refs.Create<FactoryIn_0>(); // In_0 记录第一种输入
                in_0Ref.Type = In_0.Item1; // In_0.Item1 是输入的类型
                in_0Ref.BaseValue = In_0.Item2; // In_0.Item2 是输入的数量
            }
            if (HasOut0) {
                out0Ref = Refs.Create<FactoryOut0>(); // Out0 记录第一种输出
                out0Ref.Type = Out0.Item1; // Out0 记录第一种输出的类型
                out0Ref.BaseValue = Out0.Item2; // Out0 记录第一种输出的数量
            }

            Values = Weathering.Values.GetOne();
            worker = Values.Create<Factory>(); // 记录工人数目
        }

        public override void OnEnable() {
            base.OnEnable();
            worker = Values.Get<Factory>();

            if (HasIn_0) {
                in_0Ref = Refs.Get<FactoryIn_0>();
            }
            if (HasOut0) {
                out0Ref = Refs.Get<FactoryOut0>();
            }
        }

        protected abstract long WorkerCost { get; }

        private void SendWorker() {
            if (WorkerCost == 0) return; // 不需要工人
            Map.Inventory.Remove<Worker>(WorkerCost);
            worker.Max += WorkerCost;
        }
        private void RetriveWorker() {
            Map.Inventory.Add<Worker>(WorkerCost);
            worker.Max -= WorkerCost;
        }

        private bool CanRun() {
            // 如果有工人和所有原材料，那么制造输出。
            if (HasIn_0 && in_0Ref.Value != In_0.Item2) return false; // 输入不足，不能运转

            if (worker.Max != WorkerCost && Map.Inventory.Get<Worker>() < WorkerCost) return false; // 工人不够，不能运转
            return true;
        }
        private void Run() {
            if (!CanRun()) throw new Exception(); // defensive

            if (worker.Max == 0) SendWorker(); // 提供工人

            if (HasIn_0) {
                in_0Ref.Value = 0; // 消耗输入
                in_0Ref.BaseValue = 0; // 不再需求输入
            }
            if (HasOut0) {
                out0Ref.Value = Out0.Item2; // 生产输出
            }

            NeedUpdateSpriteKeys = true;
            OnTap();
        }

        private bool CanStop() {
            if (HasOut0 && out0Ref.Value != Out0.Item2) return false; // 产品使用中

            if (worker.Max != WorkerCost) throw new Exception();
            if (Map.Inventory.CanAdd<Worker>() < WorkerCost) return false; // 背包空间不足
            return true;
        }

        private void Stop() {
            if (!CanStop()) throw new Exception(); // defensive

            RetriveWorker();

            if (HasIn_0) {
                in_0Ref.BaseValue = In_0.Item2;
                in_0Ref.Value = In_0.Item2;
            }
            if (HasOut0) {
                out0Ref.Value = 0;
            }


            NeedUpdateSpriteKeys = true;
            OnTap();
        }

        public override void OnTap() {

            var items = new List<IUIItem>() { };

            items.Add(UIItem.CreateText($"工作人员 {Localization.Ins.Val<Worker>(worker.Max)}"));

            items.Add(UIItem.CreateButton($"开始运转{Localization.Ins.ValPlus<Worker>(-WorkerCost)}", Run, CanRun));

            items.Add(UIItem.CreateButton($"停止运转{Localization.Ins.ValPlus<Worker>(WorkerCost)}", Stop, CanStop));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => worker.Max == 0));

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }
    }
}
