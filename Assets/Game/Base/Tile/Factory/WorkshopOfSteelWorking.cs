

using System;

namespace Weathering
{


    // 钢锭
    [ConceptSupply(typeof(SteelIngotSupply))]
    [ConceptDescription(typeof(SteelIngotDescription))]
    [Depend(typeof(MetalIngot))]
    [Concept]
    public class SteelIngot { }

    [ConceptResource(typeof(SteelIngot))]
    [Depend(typeof(MetalIngotSupply))]
    [Concept]
    public class SteelIngotSupply { }

    [Concept]
    public class SteelIngotDescription { }

    // 钢厂
    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class WorkshopOfSteelWorking : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 3);
        protected override (Type, long) Out0 => (typeof(SteelIngotSupply), 1);
        protected override (Type, long) In_0 => (typeof(IronIngotSupply), 3);
        protected override (Type, long) In_1 => (typeof(FuelSupply), 3);

    }
}
