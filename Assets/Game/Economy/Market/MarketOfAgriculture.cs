

using System;
using System.Collections.Generic;

namespace Weathering
{

    [ConstructionCostBase(typeof(ToolPrimitive), 100, 0)]
    public class MarketOfAgriculture : AbstractMarket
    {
        public override long MultiplierIfSell => 10;
        public override Type CurrencyType => typeof(GoldCoin);

        protected override void AddMarketButtons(List<IUIItem> items) {
            items.Add(CreateMarketButtonFor<GoldOre>(1000));
            items.Add(CreateMarketButtonFor<Berry>(10000));
            items.Add(CreateMarketButtonFor<Grain>(10000));
            items.Add(CreateMarketButtonFor<FishFlesh>(10000));
            items.Add(CreateMarketButtonFor<DeerMeat>(10000));
        }
    }
}

