

using System;

namespace Weathering
{
    // 石油，液体
    [ConceptSupply(typeof(CrudeOilSupply))]
    [ConceptDescription(typeof(CrudeOilDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class CrudeOil { }
    [ConceptResource(typeof(CrudeOil))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class CrudeOilSupply { }
    [Concept]
    public class CrudeOilDescription { }

    [ConstructionCostBase(typeof(IronProduct), 300)]
    public class OilDriller : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 1);

        protected override (Type, long) Out0 => (typeof(CrudeOilSupply), 1);
    }
}
