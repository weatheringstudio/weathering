

using System;
using System.Collections.Generic;

namespace Weathering
{
    // 太空项目科技点
    [ConceptSupply(typeof(SpaceProgramCurrencySupply))]
    [ConceptDescription(typeof(SpaceProgramCurrencyDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class SpaceProgramCurrency { }
    [ConceptResource(typeof(SpaceProgramCurrency))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class SpaceProgramCurrencySupply { }
    [Concept]
    public class SpaceProgramCurrencyDescription { }


    public class MarketForSpaceProgram : AbstractMarket
    {
        public override Type CurrencyType => typeof(SpaceProgramCurrency);

        protected override void AddMarketButtons(List<IUIItem> items) {
            items.Add(CreateMarketButtonFor<GoldCoin>(10));
            items.Add(CreateMarketButtonFor<LiquefiedPetroleumGas>(200000));
            items.Add(CreateMarketButtonFor<Plastic>(100000));
        }
    }
}
