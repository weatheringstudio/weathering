

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class TransportStationDest : StandardTile, ILinkEvent, ILinkProvider, IRunable
    {
        public override string SpriteKeyRoad => typeof(TransportStation).Name;
        public override bool HasDynamicSpriteAnimation => true;

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
        public void Provide(List<IRef> refs) {
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

            Delivering = false;
        }

        public override void OnEnable() {
            base.OnEnable();
            RefOfDelivery = Refs.Get<TransportStation>();
        }


        public void Run() {
            if (!CanRun()) throw new Exception();
            if (RefOfDelivery.Type == null) throw new Exception();
            RefOfDelivery.BaseValue = capacity; // 提供输出
            RefOfDelivery.Value = capacity; // 提供输出
            Map.Inventory.Remove(RefOfDelivery.Type, capacity); // 移除supply
            Delivering = true;
        }
        public bool CanRun() {
            if (Delivering) return false; // 已经开始运输了
            if (RefOfDelivery.Type == null) return false; // 没有选择输入
            if (Map.Inventory.Get(RefOfDelivery.Type) < capacity) return false; // 背包没有选择的物资
            return true;
        }

        public void Stop() {
            if (!CanStop()) throw new Exception();
            RefOfDelivery.BaseValue = 0;
            RefOfDelivery.Value = 0;
            Map.Inventory.Add(RefOfDelivery.Type, capacity);
            Delivering = false;
        }
        public bool CanStop() {
            if (!Delivering) return false; // 没有开始运输
            if (RefOfDelivery.Type == null) throw new Exception();
            if (RefOfDelivery.BaseValue != capacity) throw new Exception();
            if (RefOfDelivery.BaseValue != RefOfDelivery.Value) return false; // 物资使用中
            if (Map.Inventory.CanAdd(RefOfDelivery.Type) < capacity) return false; // 背包空间不足
            return true;
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            string selectingName = RefOfDelivery.Type == null ? "未选择" : $"已经选择{ Localization.Ins.ValPlus(RefOfDelivery.Type, capacity)}";

            items.Add(UIItem.CreateStaticButton($"选择类型。{selectingName}", SelectTypePage, !Delivering)); ;

            string itemName = RefOfDelivery.Type == null ? "" : Localization.Ins.ValPlus(RefOfDelivery.Type, capacity);
            items.Add(UIItem.CreateStaticButton($"开始运输{itemName}", () => { Run(); OnTap(); }, CanRun()));
            items.Add(UIItem.CreateStaticButton($"停止运输{itemName}", () => { Stop(); OnTap(); }, CanStop()));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => !Delivering));

            UI.Ins.ShowItems(Localization.Ins.Get<TransportStationDest>(), items);
        }
        private void SelectTypePage() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateReturnButton(OnTap));
            items.Add(UIItem.CreateStaticButton("取消选择", () => {
                RefOfDelivery.Type = null;
                OnTap();
            }, RefOfDelivery.Type != null));

            int itemsCount = items.Count;
            foreach (var pair in Map.Inventory) {
                if (pair.Value.value >= capacity // 背包里有足够物资
                    && Tag.HasTag(pair.Key, typeof(NonDiscardableSupply))) { // 物资是supply/nondiscardable类型
                    items.Add(UIItem.CreateButton($"选择{Localization.Ins.ValUnit(pair.Key)}", () => {
                        RefOfDelivery.Type = pair.Key;
                        OnTap();
                    }));
                }
            }
            if (itemsCount == items.Count) {
                items.Add(UIItem.CreateText("背包里没有任何Supply"));
            }

            UI.Ins.ShowItems("选择类型", items);
        }

        public void OnLink(Type direction, long quantity) {
            if (quantity < 0 && !LinkUtility.HasAnyLink(this)) {
                RefOfDelivery.Type = null;
            }
        }
    }
}
