
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
    public class CellarForPersonalStorage : StandardTile, IPassable
    {
        public bool Passable => true;

        public override string SpriteKey => typeof(CellarForPersonalStorage).Name;

        IValue level;
        public override void OnConstruct() {
            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 10000000000;
            Inventory.TypeCapacity = 5;

        }

        public override void OnEnable() {
            base.OnEnable();
        }

        // public static List<Type> allTypes = new List<Type>();
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
            items.Add(UIItem.CreateText(Localization.Ins.Get<CellarForPersonalStorage>()));
            foreach (var pair in Inventory) {
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Val(pair.Key, Inventory.CanRemove(pair.Key)),
                    OnTap = () => {
                        if (all) {
                            long val = Map.Inventory.AddFrom(pair.Key, Inventory);
                        } else {
                            long val = Map.Inventory.AddFrom(pair.Key,
                                Inventory, Inventory.CanRemove(pair.Key) / 2);
                        }
                        OnTap();
                    }
                });
            }

            items.Add(UIItem.CreateInventoryCapacity(Inventory));
            items.Add(UIItem.CreateInventoryTypeCapacity(Inventory));

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateText("仓库物资"));
            foreach (var pair in Map.Inventory) {
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Val(pair.Key, Map.Inventory.CanRemove(pair.Key)),
                    OnTap = () => {
                        if (all) {
                            long val = Inventory.AddFrom(pair.Key, Map.Inventory);
                        } else {
                            long val = Inventory.AddFrom(pair.Key,
                                Map.Inventory, Map.Inventory.CanRemove(pair.Key) / 2);
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
                OnTap = () => { all = !all; OnTap(); },
                Content = all
                ? Localization.Ins.Get<FacilityStorageManual_TransferAll>()
                : Localization.Ins.Get<FacilityStorageManual_TransferHalf>()
            });

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateStaticDestructButton<TerrainDefault>(this, CanDestruct()));

            UI.Ins.ShowItems(Localization.Ins.Get<CellarForPersonalStorage>(), items);
        }

        public override bool CanDestruct() => Inventory.TypeCount == 0;
    }
}

