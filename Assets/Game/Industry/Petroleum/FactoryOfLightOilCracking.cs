﻿

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfLightOilCracking : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey("FactoryOfPetroleumCracking");

        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 5);

        protected override (Type, long) Out0 => (typeof(LiquefiedPetroleumGasSupply), 1);
        protected override (Type, long) In_0 => (typeof(LightOilSupply), 1);
    }
}