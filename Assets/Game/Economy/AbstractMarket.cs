

using System;
using System.Collections.Generic;

namespace Weathering
{
    public abstract class AbstractMarket : StandardTile
    {
        public override string SpriteKey => typeof(MarketForPlayer).Name;

        public abstract Type CurrencyType { get; }
        public virtual long MultiplierIfSell { get; } = 1;

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateMultilineText($"本市场使用货币{Localization.Ins.Get(CurrencyType)}"));

            if (MultiplierIfSell > 1) {
                items.Add(UIItem.CreateText($"本黑市购买价格是出售价格的{MultiplierIfSell}倍"));
            }
            else if (MultiplierIfSell != 1) {
                throw new Exception();
            }

            if (Map.Inventory.TypeCapacity - Map.Inventory.TypeCount < 3) {
                items.Add(UIItem.CreateText("背包里东西种类太多了，可能放不下要买的商品，清理一下再来市场吧"));
            } else if (Map.Inventory.QuantityCapacity - Map.Inventory.Quantity < maxGood) {
                items.Add(UIItem.CreateText("背包里东西数量太多了，可能放不下要买的商品，清理一下再来市场吧"));
            } else {
                AddMarketButtons(items);
            }

            items.Add(UIItem.CreateStaticDestructButton(this));

            UI.Ins.ShowItems(Localization.Ins.Get<MarketForPlayer>(), items);
        }

        protected abstract void AddMarketButtons(List<IUIItem> items);

        public override bool CanDestruct() {
            return true;
        }

        protected UIItem CreateMarketButtonFor<T>(long forACoin) {
            Type type = typeof(T);
            return UIItem.CreateButton($"买卖{Localization.Ins.ValUnit(type)}", () => OpenPageForRecipe(type, forACoin));
        }

        private void PlayMarketSFX() {
            Sound.Ins.Play("mixkit-video-game-treasure-2066");
        }


        private float sliderValueForBuy = 0;
        private float sliderValueForSell = 0;

        private const long maxGood = 10_000_000;
        private void OpenPageForRecipe(Type type, long forACoinIfBuy) {
            if (forACoinIfBuy <= 0) throw new Exception();
            long forACoinIfSell = forACoinIfBuy * MultiplierIfSell;

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateReturnButton(OnTap));

            long coin = Map.Inventory.CanRemove(CurrencyType);
            long good = Map.Inventory.CanRemoveWithTag(type);

            items.Add(UIItem.CreateSeparator());
            long goldIfBuyAll = coin;
            if (goldIfBuyAll > 0) {
                var buySlider = new UIItem {
                    Type = IUIItemType.Slider,
                    InitialSliderValue = 1,
                    DynamicSliderContent = (float x) => {
                        sliderValueForBuy = x;
                        long hand = (long)(x * goldIfBuyAll);
                        long quantity = hand * forACoinIfBuy;
                        return $"花费{Localization.Ins.Val(CurrencyType, hand)}买进{Localization.Ins.Val(type, quantity)}";
                    }
                };
                items.Add(buySlider);
                items.Add(UIItem.CreateButton("确认买进", () => {
                    long hand = (long)(sliderValueForBuy * goldIfBuyAll);
                    long quantity = hand * forACoinIfBuy;
                    Map.Inventory.Remove(CurrencyType, hand);
                    Map.Inventory.Add(type, quantity);
                    OpenPageForRecipe(type, forACoinIfBuy);
                    PlayMarketSFX();
                }));
            } else {
                items.Add(UIItem.CreateText($"没有任何{Localization.Ins.ValUnit(CurrencyType)}，无法购买{Localization.Ins.ValUnit(type)}"));
            }

            items.Add(UIItem.CreateSeparator());
            long goldIfSellAll = Math.Min(good, maxGood) / forACoinIfSell;
            if (goldIfSellAll > 0) {

                var sellSlider = new UIItem {
                    Type = IUIItemType.Slider,
                    InitialSliderValue = 1,
                    DynamicSliderContent = (float x) => {
                        sliderValueForSell = x;
                        long hand = (long)(x * goldIfSellAll);
                        long quantity = hand * forACoinIfSell;
                        return $"卖出{Localization.Ins.Val(type, quantity)}赚取{Localization.Ins.Val(CurrencyType, hand)}";
                    }
                };
                items.Add(sellSlider);
                items.Add(UIItem.CreateButton("确认卖出", () => {
                    long hand = (long)(sliderValueForSell * goldIfSellAll);
                    long quantity = hand * forACoinIfSell;
                    Map.Inventory.RemoveWithTag(type, quantity);
                    Map.Inventory.Add(CurrencyType, hand);
                    OpenPageForRecipe(type, forACoinIfSell);
                    PlayMarketSFX();
                }));
            } else {
                items.Add(UIItem.CreateText($"{Localization.Ins.Val(type, good)}不足{Localization.Ins.Val(type, forACoinIfSell)}，无法获得任何{Localization.Ins.ValUnit(CurrencyType)}"));
            }

            UI.Ins.ShowItems(Localization.Ins.ValUnit(type), items); ;
        }
    }
}
