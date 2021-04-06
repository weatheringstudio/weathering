

using System;

namespace Weathering
{
    // 石油，液体
    [ConceptSupply(typeof(CrudeOilSupply))]
    [ConceptDescription(typeof(CrudeOilDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class CrudeOil { }
    [ConceptResource(typeof(CrudeOil))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class CrudeOilSupply { }
    [Concept]
    public class CrudeOilDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class OilDriller : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(OilDriller).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 10);

        protected override (Type, long) Out0 => (typeof(CrudeOilSupply), 1);
    }
}
