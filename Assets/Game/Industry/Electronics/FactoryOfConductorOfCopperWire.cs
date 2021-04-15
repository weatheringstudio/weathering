

using System;

namespace Weathering
{
    // 铜导线
    [Depend(typeof(DiscardableSolid))]
    public class CopperWire { }

    public class FactoryOfConductorOfCopperWire : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 10);

        protected override (Type, long) Out0 => (typeof(CopperWire), 8);

        protected override (Type, long) In_0 => (typeof(CopperIngot), 1);
    }
}
