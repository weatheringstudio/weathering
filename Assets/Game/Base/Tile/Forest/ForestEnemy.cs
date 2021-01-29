
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public class ForestEnemy : StandardTile
	{
        public override string SpriteKey => $"GrasslandEnemy{enemyIndex}";

        public override void OnEnable() {
            base.OnEnable();
            enemyIndex = HashCode % enemyIndexMax;
        }

        private const uint enemyIndexMax = 4;
        private uint enemyIndex = 0;


        private long defaultFoodCost = 10;

        public override void OnTap() {
            var items = new List<IUIItem>();

            switch (enemyIndex) {
                case 0:
                    const long cost0 = 100;
                    items.Add(UIItem.CreateText("大猫咪在草地上晒太阳"));
                    items.Add(UIItem.CreateButton($"用食物引开{Localization.Ins.Val<Food>(-cost0)}", () => {
                        Dictionary<Type, InventoryItemData> canRemoveDict = new Dictionary<Type, InventoryItemData>();
                        long canRemove = Map.Inventory.CanRemoveWithTag<Food>(canRemoveDict, cost0);
                        if (canRemove < cost0) {
                            UIPreset.ResourceInsufficient<Food>(OnTap, cost0, Map.Inventory);
                            return;
                        }

                        Map.Inventory.RemoveWithTag<Food>(cost0, canRemoveDict, null);

                        Map.UpdateAt<Forest>(Pos);
                        UIPreset.Notify(() => {
                            Map.Get(Pos).OnTap();

                        }, "猫猫被食物引诱走了");
                    }));
                    break;
                case 1:
                    const long cost1 = 100;
                    items.Add(UIItem.CreateText("蛇精不愿意离开草地"));
                    items.Add(UIItem.CreateButton($"找个木棍挑走{Localization.Ins.Val<Wood>(-cost1)}", () => {
                        Dictionary<Type, InventoryItemData> canRemoveDict = new Dictionary<Type, InventoryItemData>();
                        long canRemove = Map.Inventory.CanRemoveWithTag<Wood>(canRemoveDict, cost1);
                        if (canRemove < cost1) {
                            UIPreset.ResourceInsufficient<Wood>(OnTap, cost1, Map.Inventory);
                            return;
                        }

                        Map.Inventory.RemoveWithTag<Wood>(cost1, canRemoveDict, null);

                        Map.UpdateAt<Forest>(Pos);
                        UIPreset.Notify(() => {
                            Map.Get(Pos).OnTap();

                        }, "蛇精不见了");
                    }));
                    break;

                case 2:
                    const long cost2 = 1000;
                    items.Add(UIItem.CreateText("草地上有个好大的史莱姆"));
                    items.Add(UIItem.CreateButton($"造个围栏围起来{Localization.Ins.Val<Wood>(-cost2)}", () => {
                        Dictionary<Type, InventoryItemData> canRemoveDict = new Dictionary<Type, InventoryItemData>();
                        long canRemove = Map.Inventory.CanRemoveWithTag<Wood>(canRemoveDict, cost2);
                        if (canRemove < cost2) {
                            UIPreset.ResourceInsufficient<Wood>(OnTap, cost2, Map.Inventory);
                            return;
                        }

                        Map.Inventory.RemoveWithTag<Wood>(cost2, canRemoveDict, null);

                        Map.UpdateAt<Forest>(Pos);
                        UIPreset.Notify(() => {
                            Map.Get(Pos).OnTap();

                        }, "史莱姆围栏成功困住了史莱姆");
                    }));
                    break;
                case 3:
                    const long cost3 = 1000;
                    items.Add(UIItem.CreateText("草地上的哥布林说不给食物就不走"));
                    items.Add(UIItem.CreateButton($"给食物{Localization.Ins.Val<Food>(-cost3)}", () => {
                        Dictionary<Type, InventoryItemData> canRemoveDict = new Dictionary<Type, InventoryItemData>();
                        long canRemove = Map.Inventory.CanRemoveWithTag<Food>(canRemoveDict, cost3);
                        if (canRemove < cost3) {
                            UIPreset.ResourceInsufficient<Food>(OnTap, cost3, Map.Inventory);
                            return;
                        }

                        Map.Inventory.RemoveWithTag<Food>(cost3, canRemoveDict, null);

                        Map.UpdateAt<Forest>(Pos);
                        UIPreset.Notify(() => {
                            Map.Get(Pos).OnTap();

                        }, "史莱姆围栏成功困住了史莱姆");
                    }));
                    items.Add(UIItem.CreateButton($"给把刀{Localization.Ins.Val<Knife>(-1)}", () => { }, () => false));
                    items.Add(UIItem.CreateButton($"帮哥布林找回丢失的猫咪{Localization.Ins.Val<Kitten>(-1)}", () => { }, () => false));
                    break;
                default:
                    items.Add(UIItem.CreateText("怪物挡在了路上"));
                    items.Add(UIItem.CreateButton($"赶走{Localization.Ins.Val<Food>(-defaultFoodCost)}", () => {
                        long canRemove = Map.Inventory.CanRemove<Food>();
                        if (canRemove < defaultFoodCost) {
                            UIPreset.ResourceInsufficient<Food>(OnTap, defaultFoodCost, Map.Inventory);
                            return;
                        }
                        Map.UpdateAt<Forest>(Pos);
                        Map.Get(Pos).OnTap();
                    }));
                    break;
            }

            UI.Ins.ShowItems(Localization.Ins.Get<GrasslandEnemy>(), items);
        }
    }
}

