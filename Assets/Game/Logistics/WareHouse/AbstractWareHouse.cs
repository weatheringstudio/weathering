

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class WareHouseResource { }

    public enum WareHouseMode
    {
        None, WriteOnly, ReadWrite, Disabled,
    }

    /// <summary>
    /// WareHouse特征：输入任意，可以绑定储存对应Resource
    /// </summary>
    public abstract class AbstractWareHouse : StandardTile, ILinkConsumer, ILinkProvider, ILinkEvent, IPassable, IStepOn
    {
        public bool Passable => true;

        private static AudioClip clip;
        public void OnStepOn() {
            if (ValueOfResource.Val > 0) {
                CollectItems();
                if (clip == null) {
                    clip = Sound.Ins.Get("mixkit-hard-pop-click-2364");
                }
                Sound.Ins.Play(clip);
            }
        }

        public override string SpriteKey => TypeOfResource.Type == null ? GetType().Name : $"{GetType().Name}_Working";
        public override string SpriteLeft => GetSprite(Vector2Int.left, typeof(ILeft));
        public override string SpriteRight => GetSprite(Vector2Int.right, typeof(IRight));
        public override string SpriteUp => GetSprite(Vector2Int.up, typeof(IUp));
        public override string SpriteDown => GetSprite(Vector2Int.down, typeof(IDown));
        private string GetSprite(Vector2Int pos, Type direction) {
            IRefs refs = Map.Get(Pos - pos).Refs;
            if (refs == null) return null;
            if (refs.TryGet(direction, out IRef result)) return result.Value < 0 ? result.Type.Name : null;
            return null;
        }

        public void OnLink(Type direction, long quantity) {
            TypeOfResource.Type = RefOfSupply.Type;
            ValueOfResource.Inc = RefOfSupply.Value;
            if (ValueOfResource.Val == 0 && !LinkUtility.HasAnyLink(this)) {
                RefOfSupply.Type = null;
                TypeOfResource.Type = null;
            }
            NeedUpdateSpriteKeys = true;
        }

        public IRef RefOfSupply { get; private set; } // 无法作为输入
        public IRef TypeOfResource { get; private set; }
        public void Consume(List<IRef> refs) {
            if (TypeOfResource.X == 3 || TypeOfResource.X == 1) {
                refs.Add(RefOfSupply);
            }
        }

        public void Provide(List<IRef> refs) {
            if (TypeOfResource.X == 3) {
                refs.Add(RefOfSupply);
            }
        }

        protected abstract long Capacity { get; }

        public override void OnConstruct(ITile tile) {
            base.OnConstruct(tile);

            Values = Weathering.Values.GetOne();

            ValueOfResource = Values.Create<WareHouseResource>();
            if (Capacity <= 0) throw new Exception();
            ValueOfResource.Max = Capacity;
            ValueOfResource.Del = Weathering.Value.Second;

            if (Refs == null) {
                Refs = Weathering.Refs.GetOne();
            }
            RefOfSupply = Refs.Create<AbstractWareHouse>();
            RefOfSupply.BaseValue = long.MaxValue;

            TypeOfResource = Refs.Create<WareHouseResource>();
            TypeOfResource.X = 1; // 1为只输入，3为输入输出，4为禁用。暂时用魔法数字。若要更多模式则需要重构仓库
        }


        private IValue ValueOfResource;

        public override void OnEnable() {
            base.OnEnable();
            RefOfSupply = Refs.Get<AbstractWareHouse>();
            ValueOfResource = Values.Get<WareHouseResource>();
            TypeOfResource = Refs.Get<WareHouseResource>();
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            if (TypeOfResource.Type != null) {
                items.Add(UIItem.CreateValueProgress(TypeOfResource.Type, ValueOfResource));
                items.Add(UIItem.CreateTimeProgress(TypeOfResource.Type, ValueOfResource));

                items.Add(UIItem.CreateDynamicContentButton(() => $"拿走{Localization.Ins.Val(TypeOfResource.Type, ValueOfResource.Val)}", CollectItems));

                items.Add(UIItem.CreateSeparator());

            }

            items.Add(UIItem.CreateButton("建筑费用", () => ConstructionCostBaseAttribute.ShowBuildingCostPage(OnTap, Map, GetType())));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);
            

            if (TypeOfResource.Type != null) {
                items.Add(UIItem.CreateTileImage(TypeOfResource.Type));
            }

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateStaticDestructButton<MapOfPlanetDefaultTile>(this, CanDestruct()));

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }
        public override bool CanDestruct() {
            if (LinkUtility.HasAnyLink(this)) {
                return false;
            }
            if (TypeOfResource.Type != null && !Map.Inventory.CanAdd((TypeOfResource.Type, ValueOfResource.Val))) {
                UIPreset.InventoryFull(null, Map.Inventory);
            }
            return true;
        }//; ValueOfResource.Val == 0 && !LinkUtility.HasAnyLink(this);
        public override void OnDestruct(ITile tile) {
            if (TypeOfResource.Type != null) {
                if (!Map.Inventory.CanAdd((TypeOfResource.Type, ValueOfResource.Val))) {
                    throw new Exception();
                }
                Map.Inventory.Add(TypeOfResource.Type, ValueOfResource.Val);
            }
        }

        private void CollectItems() {
            long quantity = Math.Min(Map.Inventory.CanAdd(TypeOfResource.Type), ValueOfResource.Val);
            Map.Inventory.Add(TypeOfResource.Type, quantity);
            ValueOfResource.Val -= quantity;

            if (ValueOfResource.Val == 0 && !LinkUtility.HasAnyLink(this)) {
                RefOfSupply.Type = null;
                TypeOfResource.Type = null;
            }
            NeedUpdateSpriteKeys = true;

            // OnTap();
            UI.Ins.Active = false;
        }



        //private void SetWareHouseModePage() {
        //    var items = UI.Ins.GetItems();

        //    items.Add(UIItem.CreateReturnButton(OnTap));

        //    items.Add(UIItem.CreateText($"当前仓库模式为: {CalcWareHouseModeDescription(WareHouseMode)}"));

        //    items.Add(UIItem.CreateButton($"设置为：{CalcWareHouseModeDescription(WareHouseMode.WriteOnly)}", () => { WareHouseMode = WareHouseMode.WriteOnly; OnTap(); }));
        //    items.Add(UIItem.CreateButton($"设置为：{CalcWareHouseModeDescription(WareHouseMode.ReadWrite)}", () => { WareHouseMode = WareHouseMode.ReadWrite; OnTap(); }));
        //    items.Add(UIItem.CreateButton($"设置为：{CalcWareHouseModeDescription(WareHouseMode.Disabled)}", () => { WareHouseMode = WareHouseMode.Disabled; OnTap(); }));

        //    UI.Ins.ShowItems("设置仓库模式", items);
        //}

        //private string CalcWareHouseModeDescription(WareHouseMode x) {
        //    // return Localization.Ins.Get(...)
        //    string result;
        //    switch (x) {
        //        case WareHouseMode.WriteOnly:
        //            result = "只写";
        //            break;
        //        case WareHouseMode.ReadWrite:
        //            result = "可读可写";
        //            break;
        //        case WareHouseMode.Disabled:
        //            result = "停用";
        //            break;
        //        default:
        //            throw new Exception($"??{x}??");
        //    }
        //    return result;
        //}
        //private WareHouseMode WareHouseMode {
        //    get {
        //        WareHouseMode result;
        //        switch (TypeOfResource.X) {
        //            case 1:
        //                result = WareHouseMode.WriteOnly;
        //                break;
        //            case 3:
        //                result = WareHouseMode.ReadWrite;
        //                break;
        //            case 4:
        //                result = WareHouseMode.Disabled;
        //                break;
        //            default:
        //                throw new Exception($"??{TypeOfResource.X}??");
        //        }
        //        return result;

        //    }
        //    set {
        //        long result;
        //        switch (value) {
        //            case WareHouseMode.WriteOnly:
        //                result = 1;
        //                break;
        //            case WareHouseMode.ReadWrite:
        //                result = 3;
        //                break;
        //            case WareHouseMode.Disabled:
        //                result = 4;
        //                break;
        //            default:
        //                throw new Exception($"??{TypeOfResource.X}??");
        //        }
        //        TypeOfResource.X = result;
        //    }
        //}
    }
}
