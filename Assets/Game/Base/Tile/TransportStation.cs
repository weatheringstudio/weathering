

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class TransportStation : StandardTile, ILinkEvent, ILinkConsumer, IRunable
    {
        public override string SpriteKeyRoad => typeof(TransportStation).Name;

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

        private const long workerCost = 1;
        private const long capacity = 1;

        private bool Delivering { get => RefOfDelivery.X == 1; set => RefOfDelivery.X = value ? 1 : 0; }
        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();

            Refs = Weathering.Refs.GetOne();

            RefOfDelivery = Refs.Create<TransportStation>();
            RefOfDelivery.BaseValue = capacity;

            Delivering = false;
        }

        public override void OnEnable() {
            base.OnEnable();
            RefOfDelivery = Refs.Get<TransportStation>();
        }


        public void Run() {
            if (!CanRun()) throw new Exception();
            if (RefOfDelivery.Type == null || RefOfDelivery.Value != capacity) throw new Exception();
            RefOfDelivery.BaseValue = 0;
            RefOfDelivery.Value = 0;
            Map.Inventory.Add(RefOfDelivery.Type, capacity);
            Delivering = true;
        }
        public bool CanRun() {
            if (Delivering) return false; // 已经开始运输了
            if (RefOfDelivery.Type == null) return false; // 没有输入
            if (Map.Inventory.CanAdd(RefOfDelivery.Type) < capacity) return false; // 背包装不下
            return true;
        }

        public void Stop() {
            if (!CanStop()) throw new Exception();
            RefOfDelivery.BaseValue = capacity;
            RefOfDelivery.Value = capacity;
            Map.Inventory.Remove(RefOfDelivery.Type, capacity);
            Delivering = false;
        }
        public bool CanStop() {
            if (!Delivering) return false;
            if (RefOfDelivery.Type == null) throw new Exception();
            if (Map.Inventory.Get(RefOfDelivery.Type) < capacity) return false; // 背包里没有送出去的物品
            return true;
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            string itemName = RefOfDelivery.Type == null ? "" : Localization.Ins.ValPlus(RefOfDelivery.Type, capacity);

            items.Add(UIItem.CreateDynamicButton($"开始运输{itemName}", () => { Run(); OnTap(); }, CanRun));
            items.Add(UIItem.CreateDynamicButton($"停止运输{itemName}", () => { Stop(); OnTap(); }, CanStop));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => !Delivering));

            UI.Ins.ShowItems(Localization.Ins.Get<TransportStation>(), items);
        }

        public void OnLink(Type direction, long quantity) {
            if (quantity < 0 && !LinkUtility.HasAnyLink(this)) {
                RefOfDelivery.Type = null;
            }
        }
    }
}
