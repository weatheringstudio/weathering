

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(SolarPanelComponent), 100)]
    public class PowerGeneratorOfSolarPanelStation : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(PowerPlant).Name;

        protected override (Type, long) Out0_Inventory => (typeof(Electricity), 30);
    }
}
