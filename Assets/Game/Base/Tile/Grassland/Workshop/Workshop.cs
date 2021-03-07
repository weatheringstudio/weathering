
//using System.Collections.Generic;

//namespace Weathering
//{
//    public class Workshop : StandardTile
//    {
//        public override string SpriteKey => typeof(Workshop).Name;





//        private void GotoLevel(long i) {
//            switch (i) {
//                case 0:
//                    handicraft.Max = 100;
//                    handicraft.Inc = 0;
//                    handicraft.Del = 10 * Value.Second;
//                    break;
//                case 1:
//                    handicraft.Max = 100;
//                    handicraft.Inc = 1;
//                    handicraft.Del = 10 * Value.Second;
//                    if (level.Max == 2) {
//                        handicraft.Val = 0;
//                    }
//                    break;
//                default:
//                    handicraft.Max = long.MaxValue;
//                    handicraft.Inc = 1;
//                    handicraft.Del = 10 * Value.Second;
//                    break;
//            }
//            level.Max = i;
//        }

//        private IValue handicraft;
//        private IValue level;

//        public override void OnConstruct() {
//            Values = Weathering.Values.GetOne();
//            level = Values.Create<Level>();
//            handicraft = Values.Create<WorkshopProduct>();
//            GotoLevel(0);

//            Inventory = Weathering.Inventory.GetOne();
//            Inventory.QuantityCapacity = 10;
//            Inventory.TypeCapacity = 10;
//        }
//        public override void OnEnable() {
//            base.OnEnable();
//            level = Values.Get<Level>();
//            handicraft = Values.Get<WorkshopProduct>();
//        }

//        public override void OnTap() {

//            InventoryQuery workCost = InventoryQuery.Create(OnTap, Map.Inventory
//                , new InventoryQueryItem { Quantity = 1, Type = typeof(Worker), Source = Map.Inventory }
//                , new InventoryQueryItem { Quantity = 1, Type = typeof(WoodSupply), Source = Map.Inventory }
//                );

//            InventoryQuery workRevenue = InventoryQuery.Create(OnTap, Map.Inventory
//                , new InventoryQueryItem { Quantity = 1, Type = typeof(WorkshopProduct), Target = Map.Inventory }
//                );

//            var workCostInversed = workCost.CreateInversed();
//            var workRevenueInversed = workCost.CreateInversed();

//            var items = new List<IUIItem>() { };


//            if (level.Max == 0) {
//                items.Add(UIItem.CreateText("工坊需要有人工作"));
//                items.Add(UIItem.CreateValueProgress<WorkshopProduct>(Values));
//                items.Add(UIItem.CreateTimeProgress<WorkshopProduct>(Values));
//                items.Add(UIItem.CreateSeparator());

//                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<WorkshopProduct>()}"
//                    , GatherWorkshopProduct, () => handicraft.Val > 0));

//                items.Add(UIItem.CreateButton($"派遣居民制作物品{workCost.GetDescription()}", () => {
//                    workCost.TryDo(() => {
//                        GotoLevel(1);
//                    });
//                }));
//            } else if (level.Max == 1) {
//                items.Add(UIItem.CreateText("拿块石头，切割木材"));
//                items.Add(UIItem.CreateValueProgress<WorkshopProduct>(Values));
//                items.Add(UIItem.CreateTimeProgress<WorkshopProduct>(Values));
//                items.Add(UIItem.CreateSeparator());

//                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<WorkshopProduct>()}"
//                   , GatherWorkshopProduct, () => handicraft.Val > 0));
//                items.Add(UIItem.CreateButton($"取消居民工作{workCostInversed.GetDescription()}", () => {
//                    workCostInversed.TryDo(() => {
//                        GotoLevel(0);
//                    });
//                }));

//                items.Add(UIItem.CreateButton($"开始自动供应人力物力{workRevenue.GetDescription()}", () => {
//                    workRevenue.TryDo(() => {
//                        GotoLevel(2);
//                    });
//                }));
//            } else if (level.Max == 2) {
//                items.Add(UIItem.CreateText("工匠不断地在制作物品"));
//                items.Add(UIItem.CreateInventoryItem<WorkshopProduct>(Map.Inventory, OnTap));
//                items.Add(UIItem.CreateTimeProgress<WorkshopProduct>(Values));
//                items.Add(UIItem.CreateSeparator());

//                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<WorkshopProduct>()}"
//                    , GatherWorkshopProduct, () => false));
//                items.Add(UIItem.CreateButton($"停止自动供应人力物力{workRevenueInversed.GetDescription()}", () => {
//                    workRevenueInversed.TryDo(() => {
//                        GotoLevel(1);
//                    });
//                }));
//            }

//            items.Add(UIItem.CreateDestructButton<Grassland>(this, () => level.Max <= 0));

//            UI.Ins.ShowItems(Localization.Ins.Get<Workshop>(), items);
//        }

//        private void GatherWorkshopProduct() {
//            Globals.SanityCheck();

//            if (Map.Inventory.CanAdd<WorkshopProduct>() <= 0) {
//                UIPreset.InventoryFull(OnTap, Map.Inventory);
//                return;
//            }
//            Map.Inventory.AddFrom<WorkshopProduct>(handicraft);
//        }
//    }
//}

