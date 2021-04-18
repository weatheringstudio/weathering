

using System;

namespace Weathering
{
    [Depend(typeof(DiscardableSolid))]
    public class SteelRod { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfSteelRod : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 2);
        protected override (Type, long) Out0 => (typeof(SteelRod), 4);
        protected override (Type, long) In_0 => (typeof(SteelIngot), 1);
    }
}
