

using System;
using System.Collections.Generic;
using UnityEngine;

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

    public abstract class Factory : StandardTile, ILinkProvider, ILinkConsumer, IRunable //, ILinkEvent
    {
        public string DecoratedSpriteKey(string name) => Working ? $"{name}_Working" : name;

        protected virtual bool PreserveLandscape => false;
        public override string SpriteKeyBase => PreserveLandscape ? TerrainDefault.CalculateTerrainName(Map as StandardMap, Pos) : null;
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

        public override string SpriteKey { get => DecoratedSpriteKey(typeof(Factory).Name); }


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

        /// <summary>
        /// must be static
        /// </summary>
        protected abstract long WorkerCost { get; }
        protected long WorkerCount => worker.Max;
        protected bool WorkerNeeded => WorkerCost != 0;
        protected bool Working { get => worker.Inc == 1; set => worker.Inc = value ? 1 : 0; }
        private IValue worker; // 工人


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

        //public void OnLink(Type direction, long quantity) {
        //    if (quantity > 0) {
        //        if (CanRun()) Run();
        //    }
        //}

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

            if (CanRun()) Run(); // 自动运行。不可能，OnLink里判断吧
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

        private bool CanSendWorker() {
            if (!WorkerNeeded) throw new Exception();
            if (Working) return false;
            return !WorkerNeeded || (worker.Max == 0 && Map.Inventory.Get<Worker>() >= WorkerCost);
        }
        private void SendWorker() {
            if (!WorkerNeeded) throw new Exception(); // defensive // 不需要工人
            if (!CanSendWorker()) throw new Exception(); // defensive

            Map.Inventory.Remove<Worker>(WorkerCost);
            worker.Max += WorkerCost;

            if (CanRun()) Run();
        }
        private bool CanRetriveWorker() {
            if (!WorkerNeeded) throw new Exception();
            if (Working) return false;
            return WorkerCost == 0 || (worker.Max == WorkerCost && Map.Inventory.CanAdd<Worker>() >= WorkerCost);
        }
        private void RetriveWorker() {
            if (!WorkerNeeded) throw new Exception(); // defensive // 不需要工人
            if (!CanRetriveWorker()) throw new Exception(); // defensive

            Map.Inventory.Add<Worker>(WorkerCost);
            worker.Max -= WorkerCost;
        }

        public bool CanRun() {
            if (Working) return false;

            // 如果有工人和所有原材料，那么制造输出。
            if (HasIn_0 && in_0Ref.Value != In_0.Item2) return false; // 输入不足，不能运转
            if (worker.Max != WorkerCost && Map.Inventory.Get<Worker>() < WorkerCost) return false; // 工人不够，不能运转
            return true;
        }
        public void Run() {
            if (Working) throw new Exception();
            if (!CanRun()) throw new Exception(); // defensive

            if (WorkerNeeded && worker.Max == 0) SendWorker(); // 提供工人
            Working = true;  // 派遣工人之后

            if (HasIn_0) {
                in_0Ref.Value = 0; // 消耗输入
                in_0Ref.BaseValue = 0; // 不再需求输入
            }
            if (HasOut0) {
                out0Ref.Value = Out0.Item2; // 生产输出
            }

            NeedUpdateSpriteKeys = true;
        }

        public bool CanStop() {
            if (!Working) return false;

            if (HasOut0 && out0Ref.Value != Out0.Item2) return false; // 产品使用中

            if (worker.Max != WorkerCost) { throw new Exception(); };
            if (Map.Inventory.CanAdd<Worker>() < WorkerCost) return false; // 背包空间不足
            return true;
        }

        public void Stop() {
            if (!Working) throw new Exception();
            if (!CanStop()) throw new Exception(); // defensive

            Working = false; // 收回工人之前
            if (WorkerNeeded) RetriveWorker();

            if (HasIn_0) {
                in_0Ref.BaseValue = In_0.Item2;
                in_0Ref.Value = In_0.Item2;
            }
            if (HasOut0) {
                out0Ref.Value = 0;
            }

            NeedUpdateSpriteKeys = true;
        }

        public override void OnTap() {


            var items = new List<IUIItem>() { };

            items.Add(UIItem.CreateText($"工作人员 {Localization.Ins.Val<Worker>(worker.Max)}"));
            items.Add(UIItem.CreateDynamicButton($"开始运转", ()=> { Run(); OnTap(); }, CanRun));
            items.Add(UIItem.CreateDynamicButton($"停止运转", () => { Stop(); OnTap(); }, CanStop));

            //if (WorkerNeeded) {
            //    items.Add(UIItem.CreateButton($"派遣工人{Localization.Ins.ValPlus<Worker>(-WorkerCost)}", () => { SendWorker(); OnTap(); }, CanSendWorker));
            //    items.Add(UIItem.CreateButton($"取回工人{Localization.Ins.ValPlus<Worker>(WorkerCost)}", () => { RetriveWorker(); OnTap(); }, CanRetriveWorker));
            //}

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => Working == false));

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }
    }
}
