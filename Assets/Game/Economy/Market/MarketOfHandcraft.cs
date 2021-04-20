

using System;
using System.Collections.Generic;

namespace Weathering
{

    [ConstructionCostBase(typeof(ToolPrimitive), 100, 0)]
    public class MarketOfHandcraft : AbstractMarket
    {
        public override long MultiplierIfSell => 5;
        public override Type CurrencyType => typeof(GoldCoin);

        protected override void AddMarketButtons(List<IUIItem> items) {
            items.Add(CreateMarketButtonFor<GoldOre>(2000));
            items.Add(CreateMarketButtonFor<WoodPlank>(2000));
            items.Add(CreateMarketButtonFor<StoneBrick>(2000));
            items.Add(CreateMarketButtonFor<Brick>(2000));
            items.Add(CreateMarketButtonFor<WheelPrimitive>(1000));
            items.Add(CreateMarketButtonFor<ToolPrimitive>(1000));
        }
    }
}

