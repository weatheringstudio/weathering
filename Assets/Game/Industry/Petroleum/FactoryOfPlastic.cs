

using System;

namespace Weathering
{
    // 塑料
    [ConceptSupply(typeof(PlasticSupply))]
    [ConceptDescription(typeof(PlasticDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class Plastic { }

    [ConceptResource(typeof(Plastic))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class PlasticSupply { }

    [Concept]
    public class PlasticDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfPlastic : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 10);
        protected override (Type, long) Out0 => (typeof(PlasticSupply), 1);
        protected override (Type, long) In_0 => (typeof(LiquefiedPetroleumGasSupply), 1);
    }
}
