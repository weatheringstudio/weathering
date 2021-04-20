

using System;
using System.Collections.Generic;

namespace Weathering
{

    [ConstructionCostBase(typeof(ToolPrimitive), 100, 0)]
    public class MarketOfMineral : AbstractMarket
    {
        public override long MultiplierIfSell => 10;
        public override Type CurrencyType => typeof(GoldCoin);

        protected override void AddMarketButtons(List<IUIItem> items) {
            items.Add(CreateMarketButtonFor<GoldOre>(1000));
            items.Add(CreateMarketButtonFor<Stone>(10000));
            items.Add(CreateMarketButtonFor<Clay>(10000));
            items.Add(CreateMarketButtonFor<IronOre>(10000));
            items.Add(CreateMarketButtonFor<CopperOre>(10000));
        }
    }
}

