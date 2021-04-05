

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    /// <summary>
    /// TransportStation的特征：物流输入任意，背包输出对应的东西
    /// TransportStationDest的特征：输入背包里的东西，物流输出对应的东西
    /// </summary>
    public abstract class AbstractTransportStation : StandardTile, ILinkEvent, ILinkConsumer, IRunnable, IPassable
    {
        public bool Passable => true;

        public override string SpriteKeyRoad => Running ? "TransportStation_Working" : "TransportStation";
        public override string SpriteKey => RefOfDelivery.Value > 0 ? ConceptResource.Get(RefOfDelivery.Type).Name : null;
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

        public IRef RefOfDelivery { get; private set; } // 无法作为输入
        public void Consume(List<IRef> refs) {
            refs.Add(RefOfDelivery);
        }

        protected abstract long Capacity { get; }

        public bool Running { get => RefOfDelivery.X == 1; set => RefOfDelivery.X = value ? 1 : 0; }
        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();

            Refs = Weathering.Refs.GetOne();

            RefOfDelivery = Refs.Create<AbstractTransportStation>();
            RefOfDelivery.BaseValue = Capacity;

            Running = false;
        }

        public override void OnEnable() {
            base.OnEnable();
            RefOfDelivery = Refs.Get<AbstractTransportStation>();
        }

        // private const long WorkerCost = 1;
        protected abstract long CostQuantity { get; }
        protected abstract Type CostType { get; }



        private IInventory UniverseInventoryBuffer = null;
        private IInventory GetUniverseInventory { 
            get {
                if (UniverseInventoryBuffer == null) UniverseInventoryBuffer = GameEntry.Ins.GetParentMap(AboutUniverseType, Map).Inventory;
                return UniverseInventoryBuffer;
            }
        }
        protected virtual bool AboutUniverse => false;
        protected virtual Type AboutUniverseType => null;

        private IInventory SourceInventory => Map.Inventory;
        private IInventory TargetInventory => AboutUniverse ? GetUniverseInventory : Map.Inventory;



        public void Run() {
            if (!CanRun()) throw new Exception();
            if (RefOfDelivery.Type == null || RefOfDelivery.Value != Capacity) throw new Exception();
            RefOfDelivery.BaseValue = 0;
            RefOfDelivery.Value = 0;
            Running = true;
            NeedUpdateSpriteKeys = true;

            SourceInventory.Remove(CostType, CostQuantity);
            TargetInventory.Add(RefOfDelivery.Type, Capacity);
        }
        public bool CanRun() {
            if (Running) return false; // 已经开始运输了
            if (RefOfDelivery.Type == null) return false; // 没有输入

            if (!SourceInventory.CanRemove((CostType, CostQuantity))) return false;
            if (!TargetInventory.CanAdd((RefOfDelivery.Type, Capacity))) { // 背包装不下
                UIPreset.InventoryFull(null, Map.Inventory);
                return false;
            }

            return true;
        }

        public void Stop() {
            if (!CanStop()) throw new Exception();
            RefOfDelivery.BaseValue = Capacity;
            RefOfDelivery.Value = Capacity;
            Running = false;
            NeedUpdateSpriteKeys = true;

            SourceInventory.Add((CostType, CostQuantity));
            TargetInventory.Remove(RefOfDelivery.Type, Capacity);
        }
        public bool CanStop() {
            if (!Running) return false;
            if (RefOfDelivery.Type == null) throw new Exception();

            IInventory targetInventory = AboutUniverse ? GetUniverseInventory : Map.Inventory;

            if (!TargetInventory.CanRemove((RefOfDelivery.Type, Capacity))) return false; // 背包里没有送出去的物品
            if (!SourceInventory.CanAdd((CostType, CostQuantity))) { // 背包装不下
                UIPreset.InventoryFull(null, Map.Inventory);
                return false;
            }

            return true;
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            string itemName = RefOfDelivery.Type == null ? "" : Localization.Ins.ValPlus(RefOfDelivery.Type, Capacity);

            items.Add(UIItem.CreateDynamicButton($"开始运输{itemName}", () => { Run(); OnTap(); }, CanRun));
            items.Add(UIItem.CreateDynamicButton($"停止运输{itemName}", () => { Stop(); OnTap(); }, CanStop));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateText($"运输能力: {Capacity}"));
            items.Add(UIItem.CreateText($"资源需求：{Localization.Ins.Val(CostType, CostQuantity)}"));
            items.Add(UIItem.CreateStaticDestructButton<MapOfPlanetDefaultTile>(this, CanDestruct()));

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }

        public override bool CanDestruct() {
            return !Running && !LinkUtility.HasAnyLink(this);
        }

        public void OnLink(Type direction, long quantity) {
            if (quantity < 0 && !LinkUtility.HasAnyLink(this)) {
                RefOfDelivery.Type = null;
            }
        }
    }
}
