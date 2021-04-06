

using System;

namespace Weathering
{

    // 铀235
    [ConceptSupply(typeof(Uranrium235Supply))]
    [ConceptDescription(typeof(Uranrium235Description))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class Uranrium235 { }

    [ConceptResource(typeof(Uranrium235))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class Uranrium235Supply { }

    [Concept]
    public class Uranrium235Description { }

    // 铀238
    [ConceptSupply(typeof(Uranrium238Supply))]
    [ConceptDescription(typeof(Uranrium238Supply))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class Uranrium238 { }

    [ConceptResource(typeof(Uranrium238))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class Uranrium238Supply { }

    [Concept]
    public class Uranrium238Description { }



    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class PowerGeneratorOfNulearFissionEnergy : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(PowerPlant).Name);

        protected override (Type, long) In_0 => (typeof(Uranrium235Supply), 1);
        protected override (Type, long) Out0_Inventory => (typeof(ElectricitySupply), 300);
    }
}
