

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
        public string DecoratedSpriteKey(string name) => Running ? $"{name}_Working" : name;

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


        private IRef in_0Ref; // 输入
        private IRef in_1Ref; // 输入
        //private IRef in_2Ref; // 输入
        //private IRef in_3Ref; // 输入

        private IRef out0Ref; // 输出
        //private IRef out1Ref; // 输出
        //private IRef out2Ref; // 输出
        //private IRef out3Ref; // 输出



        // protected virtual ((Type, long), (Type, long), (Type, long), (Type, long)) Out { get; }
        // protected virtual ((Type, long), (Type, long), (Type, long), (Type, long)) In_ { get; }


        protected virtual (Type, long) In_0_Inventory { get; } = (null, 0);
        protected virtual (Type, long) In_1_Inventory { get; } = (null, 0);


        private bool HasIn_0_Inventory => In_0_Inventory.Item1 != null;
        private bool HasIn_1_Inventory => In_1_Inventory.Item1 != null;

        protected virtual (Type, long) Out0_Inventory { get; } = (null, 0);
        protected virtual (Type, long) Out1_Inventory { get; } = (null, 0);
        private bool HasOut0_Inventory => Out0_Inventory.Item1 != null;
        private bool HasOut1_Inventory => Out1_Inventory.Item1 != null;

        protected virtual (Type, long) In_0 { get; } = (null, 0);
        protected virtual (Type, long) In_1 { get; } = (null, 0);
        //protected virtual (Type, long) In_2 { get; } = (null, 0);
        //protected virtual (Type, long) In_3 { get; } = (null, 0);
        private bool HasIn_0 => In_0.Item1 != null;
        private bool HasIn_1 => In_1.Item1 != null;


        protected virtual (Type, long) Out0 { get; } = (null, 0);
        //protected virtual (Type, long) Out1 { get; } = (null, 0);
        //protected virtual (Type, long) Out2 { get; } = (null, 0);
        //protected virtual (Type, long) Out3 { get; } = (null, 0);
        private bool HasOut0 => Out0.Item1 != null;



        /// <summary>
        /// must be static
        /// </summary>
        public bool Running { get => running.Value == 1; set => running.Value = value ? 1 : 0; }

        private IRef running;


        public void Consume(List<IRef> refs) {
            if (HasIn_0) {
                refs.Add(in_0Ref);
            }
            if (HasIn_1) {
                refs.Add(in_1Ref);
            }
        }
        public void Provide(List<IRef> refs) {
            if (HasOut0) {
                refs.Add(out0Ref);
            }
        }

        //public void OnLink(Type direction, long quantity) { // 已经被IRunable功能代替
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
            if (HasIn_1) {
                in_1Ref = Refs.Create<FactoryIn_1>();
                in_1Ref.Type = In_1.Item1;
                in_1Ref.BaseValue = In_1.Item2;
            }

            if (HasOut0) {
                out0Ref = Refs.Create<FactoryOut0>(); // Out0 记录第一种输出
                out0Ref.Type = Out0.Item1; // Out0 记录第一种输出的类型
                out0Ref.BaseValue = Out0.Item2; // Out0 记录第一种输出的数量
            }

            running = Refs.Create<Factory>();

            if (CanRun()) Run(); // 自动运行。不可能，OnLink里判断吧
        }

        private long QuantityCapacityRequired = 0;
        private long TypeCapacityRequired = 0;

        public override void OnEnable() {
            base.OnEnable();

            if (HasIn_0) {
                in_0Ref = Refs.Get<FactoryIn_0>();
            }
            if (HasIn_1) {
                in_1Ref = Refs.Get<FactoryIn_1>();
            }
            if (HasOut0) {
                out0Ref = Refs.Get<FactoryOut0>();
            }

            if (HasIn_0_Inventory) { TypeCapacityRequired++; QuantityCapacityRequired += In_0_Inventory.Item2; }
            if (HasIn_1_Inventory) { TypeCapacityRequired++; QuantityCapacityRequired += In_1_Inventory.Item2; }
            if (HasOut0_Inventory) { TypeCapacityRequired++; QuantityCapacityRequired += Out0_Inventory.Item2; }
            if (HasOut1_Inventory) { TypeCapacityRequired++; QuantityCapacityRequired += Out1_Inventory.Item2; }

            running = Refs.Get<Factory>();
        }


        public bool CanRun() {
            if (Running) return false;

            // 如果有工人和所有原材料，那么制造输出。
            if (HasIn_0 && in_0Ref.Value != In_0.Item2) return false; // 输入不足，不能运转
            if (HasIn_1 && in_1Ref.Value != In_1.Item2) return false; // 输入不足，不能运转

            // 有bug !!! 如果每一项都可以加入背包，但加起来不能加入背包呢
            if (Map.Inventory.TypeCapacity - Map.Inventory.TypeCount <= TypeCapacityRequired) return false; // 可能背包空间不足
            if (Map.Inventory.QuantityCapacity - Map.Inventory.Quantity <= QuantityCapacityRequired) return false; // 可能背包空间不足
            if (HasIn_0_Inventory && !Map.Inventory.CanRemove(In_0_Inventory)) return false; // 背包物品不足，不能运转
            if (HasIn_1_Inventory && !Map.Inventory.CanRemove(In_1_Inventory)) return false; // 背包物品不足，不能运转
            //if (HasOut0_Inventory && !Map.Inventory.CanAdd(Out0_Inventory)) return false; // 背包空间不足，不能运转
            //if (HasOut1_Inventory && !Map.Inventory.CanAdd(Out1_Inventory)) return false; // 背包空间不足，不能运转

            return true;
        }
        public void Run() {
            if (Running) throw new Exception();
            if (!CanRun()) throw new Exception(); // defensive
            Running = true;  // 派遣工人之后
            NeedUpdateSpriteKeys = true;

            if (HasIn_0) {
                in_0Ref.Value = 0; // 消耗输入
                in_0Ref.BaseValue = 0; // 不再需求输入
            }
            if (HasIn_1) {
                in_1Ref.Value = 0; // 消耗输入
                in_1Ref.BaseValue = 0; // 不再需求输入
            }
            if (HasOut0) {
                out0Ref.Value = Out0.Item2; // 生产输出
            }

            if (HasIn_0_Inventory) Map.Inventory.Remove(In_0_Inventory);
            if (HasIn_1_Inventory) Map.Inventory.Remove(In_1_Inventory);
            if (HasOut0_Inventory) Map.Inventory.Add(Out0_Inventory);
            if (HasOut1_Inventory) Map.Inventory.Add(Out1_Inventory);
        }

        public bool CanStop() {
            if (!Running) return false;

            if (HasOut0 && out0Ref.Value != Out0.Item2) return false; // 产品使用中

            // 有bug !!! 如果每一项都可以加入背包，但加起来不能加入背包呢
            if (Map.Inventory.TypeCapacity - Map.Inventory.TypeCount <= TypeCapacityRequired) return false; // 可能背包空间不足
            if (Map.Inventory.QuantityCapacity - Map.Inventory.Quantity <= QuantityCapacityRequired) return false; // 可能背包空间不足
            if (HasOut0_Inventory && !Map.Inventory.CanRemove(Out0_Inventory)) return false; // 背包物品不足，不能回收
            if (HasOut1_Inventory && !Map.Inventory.CanRemove(Out1_Inventory)) return false; // 背包物品不足，不能回收
            //if (HasIn_0_Inventory && !Map.Inventory.CanAdd(In_0_Inventory)) return false; // 背包空间不足
            //if (HasIn_1_Inventory && !Map.Inventory.CanAdd(In_1_Inventory)) return false; // 背包空间不足

            return true;
        }

        public void Stop() {
            if (!Running) throw new Exception();
            if (!CanStop()) throw new Exception(); // defensive

            Running = false; // 收回工人之前
            NeedUpdateSpriteKeys = true;

            // 收回工人
            if (HasIn_0) {
                in_0Ref.BaseValue = In_0.Item2;
                in_0Ref.Value = In_0.Item2;
            }
            if (HasIn_1) {
                in_1Ref.BaseValue = In_1.Item2;
                in_1Ref.Value = In_1.Item2;
            }
            if (HasOut0) {
                out0Ref.Value = 0;
            }

            if (HasOut0_Inventory) Map.Inventory.Remove(Out0_Inventory);
            if (HasOut1_Inventory) Map.Inventory.Remove(Out1_Inventory);
            if (HasIn_0_Inventory) Map.Inventory.Add(In_0_Inventory); // 背包空间不足
            if (HasIn_1_Inventory) Map.Inventory.Add(In_1_Inventory);
        }

        public override void OnTap() {
            var items = new List<IUIItem>() { };

            items.Add(UIItem.CreateButton("建筑介绍", BuildingDescriptionPage));

            items.Add(UIItem.CreateStaticButton($"开始运转", () => { Run(); OnTap(); }, CanRun()));
            items.Add(UIItem.CreateStaticButton($"停止运转", () => { Stop(); OnTap(); }, CanStop()));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => Running == false && !LinkUtility.HasAnyLink(this)));

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }

        public override bool CanDestruct() => Running == false && !LinkUtility.HasAnyLink(this);

        private void BuildingDescriptionPage() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateReturnButton(OnTap));

            if (HasIn_0_Inventory) AddDescriptionItem(items, In_0_Inventory, "第一种自动输入", true);
            if (HasIn_1_Inventory) AddDescriptionItem(items, In_1_Inventory, "第二种自动输入", true);

            if (HasOut0_Inventory) AddDescriptionItem(items, Out0_Inventory, "第一种自动输出", true);
            if (HasOut1_Inventory) AddDescriptionItem(items, Out1_Inventory, "第二种自动输出", true);

            if (HasIn_0) AddDescriptionItem(items, In_0, "第一种物流输入");
            if (HasIn_1) AddDescriptionItem(items, In_1, "第二种物流输入");
            if (HasOut0) AddDescriptionItem(items, Out0, "第一种物流输出");

            UI.Ins.ShowItems($"{Localization.Ins.Get(GetType())}介绍页面", items);
        }
        private void AddDescriptionItem(List<IUIItem> items, (Type, long) pair, string text, bool dontCreateImage=false) {
            Type res = ConceptResource.Get(pair.Item1);
            items.Add(UIItem.CreateButton($"{text}: {Localization.Ins.Val(res, pair.Item2)}", () => OnTapItem(pair.Item1)));
            if (!dontCreateImage) items.Add(UIItem.CreateTileImage(res));
        }
        private void OnTapItem(Type type) {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateReturnButton(BuildingDescriptionPage));

            UIItem.AddItemDescription(items, type);

            UI.Ins.ShowItems(Localization.Ins.ValUnit(type), items);
        }
    }
}
