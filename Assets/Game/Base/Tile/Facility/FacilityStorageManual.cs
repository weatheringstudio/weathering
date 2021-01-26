
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class FacilityStorageManual_TakeAll { }
    [Concept]
    public class FacilityStorageManual_TransferAll { }
    [Concept]
    public class FacilityStorageManual_TransferHalf { }

    [Concept]
    public class FacilityStorageManual : StandardTile
    {
        public override string SpriteKey => "StorageBuilding";

        private string facilityStorageManual;
        public override void OnEnable() {
            base.OnEnable();
            if (Inventory == null) {
                Inventory = Weathering.Inventory.GetOne();
                Inventory.QuantityCapacity = 1000;
                Inventory.TypeCapacity = 6;
            }
            facilityStorageManual = Localization.Ins.Get<FacilityStorageManual>();
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public static List<Type> allTypes = new List<Type>();
        public static bool all = true;
        public static bool takeoutResource = false;
        public override void OnTap() {
            var items = new List<IUIItem>();

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                OnTap = () => {
                    if (Map.Inventory.Empty || Inventory.Maxed) takeoutResource = true;
                    else if (Inventory.Empty || Map.Inventory.Maxed) takeoutResource = false;

                    if (takeoutResource) {
                        Map.Inventory.AddEverythingFrom(Inventory);
                    } else {
                        Inventory.AddEverythingFrom(Map.Inventory);
                    }
                    takeoutResource = !takeoutResource;
                    OnTap();
                },
                Content = "全部转移"
            });


            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateText(Localization.Ins.Get<FacilityStorageManual>()));
            foreach (var pair in Inventory) {
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Localization.Ins.Val(pair.Key, Inventory.Get(pair.Key)),
                    OnTap = () => {
                        if (all) {
                            long val = Map.Inventory.AddFrom(pair.Key, Inventory);
                        } else {
                            long val = Map.Inventory.AddFrom(pair.Key,
                                Inventory, Inventory.Get(pair.Key) / 2);
                        }
                        OnTap();
                    }
                });
            }

            items.Add(UIItem.CreateInventoryCapacity(Inventory));
            items.Add(UIItem.CreateInventoryTypeCapacity(Inventory));

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateText(Localization.Ins.Get<PlayerInventory>()));
            foreach (var pair in Map.Inventory) {
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Localization.Ins.Val(pair.Key, Map.Inventory.Get(pair.Key)),
                    OnTap = () => {
                        if (all) {
                            long val = Inventory.AddFrom(pair.Key, Map.Inventory);
                        } else {
                            long val = Inventory.AddFrom(pair.Key,
                                Map.Inventory, Map.Inventory.Get(pair.Key) / 2);
                        }
                        OnTap();
                    }
                });
            }

            items.Add(UIItem.CreateInventoryCapacity(Map.Inventory));
            items.Add(UIItem.CreateInventoryTypeCapacity(Map.Inventory));

            items.Add(UIItem.CreateSeparator());

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                OnTap = () => all = !all,
                DynamicContent = () => all 
                ? Localization.Ins.Get<FacilityStorageManual_TransferAll>() 
                : Localization.Ins.Get<FacilityStorageManual_TransferHalf>()
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = Localization.Ins.Get<Destruct>(),
                OnTap = () => {
                    Map.UpdateAt<Grassland>(Pos);
                    UI.Ins.Active = false;
                },
                CanTap = () => Inventory.Quantity == 0
            });

            items.Add(UIItem.CreateTransparency());

            UI.Ins.ShowItems(facilityStorageManual, items);
        }
    }
}

