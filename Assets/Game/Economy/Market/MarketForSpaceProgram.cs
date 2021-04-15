

using System;
using System.Collections.Generic;

namespace Weathering
{
    // 太空项目科技点
    [Depend(typeof(DiscardableSolid))]
    public class SpaceProgramCurrency { }


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
