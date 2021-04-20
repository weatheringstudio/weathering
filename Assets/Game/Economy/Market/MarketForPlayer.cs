

using System;
using System.Collections.Generic;

namespace Weathering
{


    [ConstructionCostBase(typeof(ToolPrimitive), 100, 0)]
    public class MarketForPlayer : AbstractMarket
    {
        public override long MultiplierIfSell => 10;
        public override Type CurrencyType => typeof(GoldCoin);

        protected override void AddMarketButtons(List<IUIItem> items) {
            items.Add(CreateMarketButtonFor<GoldOre>(1000));
            items.Add(CreateMarketButtonFor<Food>(10000));
            items.Add(CreateMarketButtonFor<WoodPlank>(1000));
            items.Add(CreateMarketButtonFor<StoneBrick>(1000));
            items.Add(CreateMarketButtonFor<Brick>(800));
            items.Add(CreateMarketButtonFor<ToolPrimitive>(1000));
            items.Add(CreateMarketButtonFor<WheelPrimitive>(1000));
            items.Add(CreateMarketButtonFor<MachinePrimitive>(300));
            items.Add(CreateMarketButtonFor<ConcretePowder>(300));
            items.Add(CreateMarketButtonFor<BuildingPrefabrication>(200));
        }
    }
}

