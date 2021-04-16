

using System;

namespace Weathering
{
    // 燃料
    [Depend(typeof(Discardable))]
    public class Electricity { }


    [ConstructionCostBase(typeof(BuildingPrefabrication), 300)]
    public class PowerPlant : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(PowerPlant).Name);

        protected override (Type, long) In_0 => (typeof(Fuel), 3);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0_Inventory => (typeof(Electricity), 100);
    }
}
