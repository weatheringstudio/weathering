

//using System;

//namespace Weathering
//{
//    // 煤矿
//    [ConceptSupply(typeof(GoidCoinSupply))]
//    [ConceptDescription(typeof(GoidCoinDescription))]
//    [Depend(typeof(Discardable))]
//    [Concept]
//    public class GoidCoin { }
//    [ConceptResource(typeof(GoidCoin))]
//    [Depend(typeof(Transportable))]
//    [Concept]
//    public class GoidCoinSupply { }
//    [Concept]
//    public class GoidCoinDescription { }

//    [ConstructionCostBase(typeof(WheelPrimitive), 100, 0)]
//    public class MarketForPlayer : StandardTile
//    {
//        public override void OnTap() {
//            var items = UI.Ins.GetItems();

//            if (Map.Inventory.TypeCapacity - Map.Inventory.TypeCount < 3) {
//                items.Add(UIItem.CreateText("背包里东西种类太多了，清理一下再来逛街吧"));
//            } else {
//                (Type, long) foodQuantity = (typeof(Food), 100);
//                items.Add(UIItem.CreateStaticButton("卖出食物", () => { Map.Inventory.RemoveWithTag(foodQuantity); OnTap(); }, Map.Inventory.CanRemoveWithTag(foodQuantity)));
//            }


//            UI.Ins.ShowItems(Localization.Ins.Get<MarketForPlayer>(), items);
//        }

//        private float sliderValue = 0;
//        private void OpenPageForRecipe(Type type, long price) {

//            var items = UI.Ins.GetItems();

//            long inventoryCapacity = Map.Inventory.QuantityCapacity - Map.Inventory.Quantity;
//            long goldCoin = Map.Inventory.CanRemove<GoidCoin>();
//            long maxCanBuy = Math.Min(goldCoin / price, inventoryCapacity);
//            long maxCanSell = Math.Min(Map.Inventory.CanRemove(type), 0);

//            var buySlider = new UIItem {
//                Type = IUIItemType.Slider,
//                InitialSliderValue = 1,
//                DynamicSliderContent = (float x) => {
//                    sliderValue = x;
//                    long quantity = (long)(x * maxCanBuy);
//                    long cost = quantity * price;
//                    return $"买进{quantity}花费{cost}";
//                }
//            };
//            items.Add(buySlider);
//            items.Add(UIItem.CreateButton("确认买进"));

//            UI.Ins.ShowItems(Localization.Ins.Get<MarketForPlayer>(), items);
//        }
//    }
//}
