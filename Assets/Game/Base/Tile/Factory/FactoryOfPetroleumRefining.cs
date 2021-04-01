

using System;

namespace Weathering
{
    // 重油
    [ConceptSupply(typeof(HeavyOilSupply))]
    [ConceptDescription(typeof(HeavyOilDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class HeavyOil { }

    [ConceptResource(typeof(HeavyOil))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class HeavyOilSupply { }
    [Concept]
    public class HeavyOilDescription { }

    // 轻油
    [ConceptSupply(typeof(LightOilSupply))]
    [ConceptDescription(typeof(LightOilDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class LightOil { }

    [ConceptResource(typeof(LightOil))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class LightOilSupply { }
    [Concept]
    public class LightOilDescription { }

    // 石油气
    [ConceptSupply(typeof(LiquefiedPetroleumGasSupply))]
    [ConceptDescription(typeof(LiquefiedPetroleumGasDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class LiquefiedPetroleumGas { }

    [ConceptResource(typeof(LiquefiedPetroleumGas))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class LiquefiedPetroleumGasSupply { }
    [Concept]
    public class LiquefiedPetroleumGasDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfPetroleumRefining : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 10);

        protected override (Type, long) Out0 => (typeof(LiquefiedPetroleumGasSupply), 3);
        //protected override (Type, long) Out1 => (typeof(LightOilSupply), 1);
        //protected override (Type, long) Out2 => (typeof(HeavyOilSupply), 1);

        protected override (Type, long) In_0 => (typeof(CrudeOilSupply), 3);
    }
}
