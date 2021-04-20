

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(Grain), 10, 20)]
    public class ResidenceOfGrass : AbstractFactoryStatic, IPassable
    {
        public override string SpriteKey => typeof(ResidenceOfGrass).Name;

        public bool Passable => false;

        protected override (Type, long) In_0 => (typeof(Food), 3);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 1);
    }
}
