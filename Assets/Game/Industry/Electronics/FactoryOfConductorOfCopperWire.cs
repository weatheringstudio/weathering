

using System;

namespace Weathering
{
    // 铜导线
    [ConceptSupply(typeof(CopperWireSupply))]
    [ConceptDescription(typeof(CopperWireDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class CopperWire { }

    [ConceptResource(typeof(CopperWire))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class CopperWireSupply { }
    [Concept]
    public class CopperWireDescription { }

    public class FactoryOfConductorOfCopperWire : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 10);

        protected override (Type, long) Out0 => (typeof(CopperWireSupply), 8);

        protected override (Type, long) In_0 => (typeof(CopperIngotSupply), 1);
    }
}
