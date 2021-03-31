
using System;
using System.Collections.Generic;

namespace Weathering
{
    // 木材
    [ConceptSupply(typeof(WoodSupply))]
    [ConceptDescription(typeof(WoodDescription))]
    [Depend(typeof(Fuel))]
    [Concept]
    public class Wood { }
    [ConceptResource(typeof(Wood))]
    [Depend(typeof(FuelSupply))]
    [Concept]
    public class WoodSupply { }
    [Concept]
    public class WoodDescription { }

    [ConstructionCostBase(typeof(Food), 100)]
    [BindTerrainType(typeof(TerrainType_Forest))]
    [Concept]
    class ForestLoggingCamp : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(ForestLoggingCamp).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(WoodSupply), 1);

        public bool Passable => false;
    }
}

