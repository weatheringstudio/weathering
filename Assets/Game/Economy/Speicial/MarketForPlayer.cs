

using System;
using System.Collections.Generic;

namespace Weathering
{
    // 金币
    [ConceptSupply(typeof(GoldCoinSupply))]
    [ConceptDescription(typeof(GoldCoinDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class GoldCoin { }
    [ConceptResource(typeof(GoldCoin))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class GoldCoinSupply { }
    [Concept]
    public class GoldCoinDescription { }

    [ConstructionCostBase(typeof(ToolPrimitive), 100, 0)]
    public class MarketForPlayer : AbstractMarket
    {
        public override Type CurrencyType => typeof(GoldCoin);

        protected override void AddMarketButtons(List<IUIItem> items) {
            items.Add(CreateMarketButtonFor<GoldOre>(1000));
            items.Add(CreateMarketButtonFor<Food>(10000));
            items.Add(CreateMarketButtonFor<WoodPlank>(1000));
            items.Add(CreateMarketButtonFor<StoneBrick>(900));
            items.Add(CreateMarketButtonFor<Brick>(800));
            items.Add(CreateMarketButtonFor<ConcretePowder>(1000));
            items.Add(CreateMarketButtonFor<ToolPrimitive>(1000));
            items.Add(CreateMarketButtonFor<WheelPrimitive>(500));
            items.Add(CreateMarketButtonFor<MachinePrimitive>(300));
            items.Add(CreateMarketButtonFor<ConcretePowder>(300));
            items.Add(CreateMarketButtonFor<BuildingPrefabrication>(200));
        }
    }
}

