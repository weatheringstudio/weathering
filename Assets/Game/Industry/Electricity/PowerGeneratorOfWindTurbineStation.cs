﻿

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(WindTurbineComponent), 100)]
    public class PowerGeneratorOfWindTurbineStation : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(PowerPlant).Name;

        protected override (Type, long) Out0_Inventory => (typeof(Electricity), 20);
    }
}
