

using System;
using System.Collections.Generic;

namespace Weathering
{

    [ConstructionCostBase(typeof(ToolPrimitive), 100, 0)]
    public class MarketOfMetalProduct : AbstractMarket
    {
        public override long MultiplierIfSell => 5;
        public override Type CurrencyType => typeof(GoldCoin);

        protected override void AddMarketButtons(List<IUIItem> items) {
            items.Add(CreateMarketButtonFor<GoldOre>(1000));
            items.Add(CreateMarketButtonFor<IronIngot>(2000));
            items.Add(CreateMarketButtonFor<CopperIngot>(2000));
            items.Add(CreateMarketButtonFor<IronProduct>(1000));
            items.Add(CreateMarketButtonFor<CopperProduct>(1000));
            items.Add(CreateMarketButtonFor<MachinePrimitive>(500));
        }
    }
}

