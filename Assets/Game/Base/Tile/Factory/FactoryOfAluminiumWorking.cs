

using System;

namespace Weathering
{

    // 铝锭
    [ConceptSupply(typeof(AluminiumIngotSupply))]
    [ConceptDescription(typeof(AluminiumIngotDescription))]
    [Depend(typeof(MetalIngot))]
    [Concept]
    public class AluminiumIngot { }

    [ConceptResource(typeof(AluminiumIngot))]
    [Depend(typeof(MetalIngotSupply))]
    [Concept]
    public class AluminiumIngotSupply { }

    [Concept]
    public class AluminiumIngotDescription { }


    public class FactoryOfAluminiumWorking : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalSmelting).Name);


        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 5);
        protected override (Type, long) Out0 => (typeof(AluminiumIngotSupply), 1);
        protected override (Type, long) In_0 => (typeof(AluminumOreSupply), 3);
    }
}
