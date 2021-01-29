
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public class ForestEnemy : StandardTile
	{
        public override string SpriteKey => $"ForestEnemy{enemyIndex}";

        public override void OnEnable() {
            base.OnEnable();
            enemyIndex = HashCode % enemyIndexMax;
        }

        private const uint enemyIndexMax = 4;
        private uint enemyIndex = 0;



        public override void OnTap() {
            var items = new List<IUIItem>();

            switch (enemyIndex) {
                case 0:
                    InventoryQuery enemy0Query = InventoryQuery.Create(() => { UI.Ins.Active = false; }, Map.Inventory,
                        new InventoryQueryItem { Type = typeof(Food), Quantity = 100, Source = Map.Inventory },
                        new InventoryQueryItem { Type = typeof(Kitten), Quantity = 1, Target = Map.Inventory }
                        );

                    items.Add(UIItem.CreateText("猫咪在森林里捉迷藏"));
                    items.Add(UIItem.CreateButton($"用猫薄荷捕捉{enemy0Query.GetDescription()}", () => {
                        enemy0Query.TryDo(() => {
                            Map.UpdateAt<Forest>(Pos);
                        });
                    }));
                    break;
                case 1:
                    InventoryQuery enemy1Query = InventoryQuery.Create(() => { UI.Ins.Active = false; }, Map.Inventory,
                        new InventoryQueryItem { Type = typeof(Wood), Quantity = 100, Source = Map.Inventory },
                        new InventoryQueryItem { Type = typeof(Snake), Quantity = 1, Target = Map.Inventory }
                    );

                    items.Add(UIItem.CreateText("蛇精不愿意离开森林"));
                    items.Add(UIItem.CreateButton($"找个木棍挑走捉{enemy1Query.GetDescription()}", () => {
                        enemy1Query.TryDo(() => {
                            Map.UpdateAt<Forest>(Pos);
                        });
                    }));
                    break;
                case 2:
                    InventoryQuery enemy2Query = InventoryQuery.Create(() => { UI.Ins.Active = false; }, Map.Inventory,
                        new InventoryQueryItem { Type = typeof(Wood), Quantity = 1000, Source = Map.Inventory },
                        new InventoryQueryItem { Type = typeof(SlimeLiquid), Quantity = 100, Target = Map.Inventory }
                    );

                    items.Add(UIItem.CreateText("森林里有个好大的史莱姆"));
                    items.Add(UIItem.CreateButton($"造个围栏围起来{enemy2Query.GetDescription()}", () => {
                        enemy2Query.TryDo(() => {
                            Map.UpdateAt<Forest>(Pos);
                        });
                    }));
                    break;
                case 3:
                    const long cost3 = 1000;
                    items.Add(UIItem.CreateText("哥布林说不给食物就不离开森林"));
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
                        }, "哥布林满意地离开了森林");
                    }));
                    const long kittenCost = 1;
                    items.Add(UIItem.CreateButton($"给哥布林瞧瞧村里最好的物理学圣剑{Localization.Ins.Val<Knife>(-1)}", () => { }, () => false));
                    items.Add(UIItem.CreateButton($"帮哥布林找回丢失的猫咪{Localization.Ins.Val<Kitten>(-kittenCost)}", () => {
                        Map.Inventory.Remove<Kitten>(kittenCost);

                        Map.UpdateAt<Forest>(Pos);
                        UIPreset.Notify(() => {
                            UI.Ins.Active = false;
                        }, "哥布林抱着猫猫离开了森林");
                    }, () => {
                        return Map.Inventory.Get<Kitten>() >= kittenCost;
                    }));
                    break;
                default:
                    const long defaultFoodCost = 10;
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

            UI.Ins.ShowItems(Localization.Ins.Get<ForestEnemy>(), items);
        }
    }
}

