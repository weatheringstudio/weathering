
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("私人仓库")]
    public class FacilityStorageManual : StandardTile
    {
        public override string SpriteKey => "StorageBuilding";

        private string facilityStorageManual;
        public override void OnEnable() {
            base.OnEnable();
            if (Inventory == null) {
                Inventory = Weathering.Inventory.Create();
                Inventory.QuantityCapacity = 1000;
                Inventory.TypeCapacity = 6;
            }
            facilityStorageManual = Concept.Ins.ColoredNameOf<FacilityStorageManual>();
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public static bool all = true;
        public override void OnTap() {
            var items = new List<IUIItem>();

            items.Add(UIItem.CreateText("仓库"));

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                OnTap = () => all = !all,
                DynamicContent = () => all ? "转移全部" : "转移一半"
            });

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateText("仓库"));
            foreach (var pair in Inventory) {
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Concept.Ins.Val(pair.Key, Inventory.Get(pair.Key)),
                    OnTap = () => {
                        if (all) {
                            long val = Map.Inventory.AddAsManyAsPossible(pair.Key, Inventory);
                        } else {
                            long val = Map.Inventory.AddAsManyAsPossible(pair.Key,
                                Inventory, Inventory.Get(pair.Key) / 2);
                        }
                        OnTap();
                    }
                });
            }
            UIItem.AddInventoryInfo(Inventory, items);


            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateText("背包"));
            foreach (var pair in Map.Inventory) {
                items.Add(new UIItem {
                    Type = IUIItemType.Button,
                    DynamicContent = () => Concept.Ins.Val(pair.Key, Map.Inventory.Get(pair.Key)),
                    OnTap = () => {
                        if (all) {
                            long val = Inventory.AddAsManyAsPossible(pair.Key, Map.Inventory);
                        } else {
                            long val = Inventory.AddAsManyAsPossible(pair.Key,
                                Map.Inventory, Map.Inventory.Get(pair.Key) / 2);
                        }
                        OnTap();
                    }
                });
            }
            UIItem.AddInventoryInfo(Map.Inventory, items);

            items.Add(UIItem.CreateSeparator());
            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = Concept.Ins.ColoredNameOf<Destruct>(),
                OnTap = UIDecorator.ConfirmBefore(() => {
                    Map.UpdateAt<Grassland>(Pos);
                    UI.Ins.Active = false;
                })
            });


            items.Add(UIItem.CreateTransparency());

            UI.Ins.ShowItems(facilityStorageManual, items);
        }
    }
}

