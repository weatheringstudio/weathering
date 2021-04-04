

using System;

namespace Weathering
{
    // 盐
    [ConceptSupply(typeof(SaltSupply))]
    [ConceptDescription(typeof(SaltDescription))]
    [Depend(typeof(Fuel))]
    [Concept]
    public class Salt { }
    [ConceptResource(typeof(Salt))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class SaltSupply { }
    [Concept]
    public class SaltDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [BindTerrainType(typeof(TerrainType_Mountain))]
    [Concept]
    public class MineOfSalt : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(SaltSupply), 1);

        public bool Passable => false;
    }
}
