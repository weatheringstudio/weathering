
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class AbstractTransporter_TransferAll { }
    [Concept]
    public class AbstractTransporter_TransferHalf { }

    [Concept]
    public abstract class AbstractTransporter : StandardTile, ILinkTypeRestriction
    {
        public abstract Type LinkTypeRestriction { get; }

        private Type linkTypeRestriction = null;

        public override string SpriteKey => typeof(CellarForPersonalStorage).Name;

        /// <summary>
        /// static
        /// </summary>
        public virtual bool UseSelfInventoryOrSpaceInventory { get; } = true;

        public virtual long RecycleForAGoldOre { get => 0; }
        public bool EnableRecycle { get => RecycleForAGoldOre > 0 && UseSelfInventoryOrSpaceInventory; }

        private bool useSelfInventoryOrSpaceInventory = true;

        public override void OnConstruct(ITile tile) {
            useSelfInventoryOrSpaceInventory = UseSelfInventoryOrSpaceInventory;
            if (useSelfInventoryOrSpaceInventory) {
                Inventory = Weathering.Inventory.GetOne();
                Inventory.QuantityCapacity = 10000000000;
                Inventory.TypeCapacity = 5;
            } else {

            }
        }

        private IInventory otherInventory;
        private IInventory mapInventory;

        public override void OnEnable() {
            base.OnEnable();

            useSelfInventoryOrSpaceInventory = UseSelfInventoryOrSpaceInventory;
            linkTypeRestriction = LinkTypeRestriction;
            if (linkTypeRestriction == null) throw new Exception();

            otherInventory = useSelfInventoryOrSpaceInventory ? Inventory : Map.ParentTile.GetMap().Inventory;
            mapInventory = Map.Inventory;

            if (otherInventory == null) throw new Exception();
            if (mapInventory == null) throw new Exception();
        }

        // public static List<Type> allTypes = new List<Type>();
        public static bool all = true;
        public static bool takeoutResource = false;
        public override void OnTap() {
            var items = new List<IUIItem>();

            items.Add(UIItem.CreateText($"转移类型限制：{Localization.Ins.Get(linkTypeRestriction)}"));

            if (EnableRecycle) {
                long quantity = otherInventory.Quantity;
                long goldOreRevenue = quantity / RecycleForAGoldOre;
                items.Add(UIItem.CreateStaticButton($"卖掉一切，换取{Localization.Ins.Val<GoldOre>(goldOreRevenue)}", () => {
                    if (mapInventory.CanAdd<GoldOre>() >= goldOreRevenue) {
                        mapInventory.Add<GoldOre>(goldOreRevenue);
                        otherInventory.Clear();
                    }
                    else {
                        UIPreset.Notify(OnTap, "背包满了");
                    }
                    OnTap();
                }, quantity > 0));
            }

            //items.Add(new UIItem {
            //    Type = IUIItemType.Button,
            //    OnTap = () => {
            //        if (mapInventory.Empty || otherInventory.Maxed) takeoutResource = true;
            //        else if (otherInventory.Empty || mapInventory.Maxed) takeoutResource = false;

            //        if (takeoutResource) {
            //            mapInventory.AddEverythingFrom(otherInventory);
            //        } else {
            //            otherInventory.AddEverythingFrom(mapInventory);
            //        }
            //        takeoutResource = !takeoutResource;
            //        OnTap();
            //    },
            //    Content = "全部转移"
            //});
            //items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateText(useSelfInventoryOrSpaceInventory ? Localization.Ins.Get<CellarForPersonalStorage>() : "太空物资"));
            foreach (var pair in otherInventory) {
                if (!Tag.HasTag(pair.Key, linkTypeRestriction)) {
                    continue;
                }
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Val(pair.Key, otherInventory.CanRemove(pair.Key)),
                    OnTap = () => {
                        if (all) {
                            long val = mapInventory.AddFrom(pair.Key, otherInventory);
                        } else {
                            long val = mapInventory.AddFrom(pair.Key,
                                otherInventory, otherInventory.CanRemove(pair.Key) / 2);
                        }
                        OnTap();
                    }
                });
            }

            items.Add(UIItem.CreateInventoryCapacity(otherInventory));
            items.Add(UIItem.CreateInventoryTypeCapacity(otherInventory));

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateText(useSelfInventoryOrSpaceInventory ? "仓库物资" : "地面物资"));
            foreach (var pair in mapInventory) {
                if (!Tag.HasTag(pair.Key, linkTypeRestriction)) {
                    continue;
                }
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Val(pair.Key, mapInventory.CanRemove(pair.Key)),
                    OnTap = () => {
                        if (all) {
                            long val = otherInventory.AddFrom(pair.Key, mapInventory);
                        } else {
                            long val = otherInventory.AddFrom(pair.Key,
                                mapInventory, mapInventory.CanRemove(pair.Key) / 2);
                        }
                        OnTap();
                    }
                });
            }

            items.Add(UIItem.CreateInventoryCapacity(mapInventory));
            items.Add(UIItem.CreateInventoryTypeCapacity(mapInventory));

            items.Add(UIItem.CreateSeparator());

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                OnTap = () => { all = !all; OnTap(); },
                Content = all
                ? Localization.Ins.Get<AbstractTransporter_TransferAll>()
                : Localization.Ins.Get<AbstractTransporter_TransferHalf>()
            });

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateStaticDestructButton(this));

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }

        public override bool CanDestruct() => useSelfInventoryOrSpaceInventory ? otherInventory.TypeCount == 0 : true;
    }
}

