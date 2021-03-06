﻿

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class PowerGeneratorOfLiquefiedPetroleumGas : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(PowerPlant).Name;

        protected override (Type, long) In_0 => (typeof(LiquefiedPetroleumGas), 6);
        protected override (Type, long) Out0_Inventory => (typeof(Electricity), 150);
    }
}
