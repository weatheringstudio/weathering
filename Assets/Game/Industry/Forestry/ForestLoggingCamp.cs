
using System;
using System.Collections.Generic;

namespace Weathering
{
    // 木材
    [Depend(typeof(Fuel))]
    public class Wood { }

    [ConstructionCostBase(typeof(Wood), 10, 20)]
    [BindTerrainType(typeof(TerrainType_Forest))]
    [Concept]
    class ForestLoggingCamp : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(ForestLoggingCamp).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(Wood), 1);

        public bool Passable => false;
    }
}

