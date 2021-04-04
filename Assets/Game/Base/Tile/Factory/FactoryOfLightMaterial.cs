

using System;

namespace Weathering
{
    // 轻质材料
    [ConceptSupply(typeof(LightMaterialSupply))]
    [ConceptDescription(typeof(LightMaterialDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class LightMaterial { }

    [ConceptResource(typeof(LightMaterial))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class LightMaterialSupply { }
    [Concept]
    public class LightMaterialDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfLightMaterial : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 10);

        protected override (Type, long) Out0 => (typeof(LightMaterialSupply), 3);

        protected override (Type, long) In_0 => (typeof(PlasticSupply), 2);
        protected override (Type, long) In_1 => (typeof(AluminiumIngotSupply), 2);
    }
}
