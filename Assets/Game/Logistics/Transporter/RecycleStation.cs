
using System;

namespace Weathering
{

    [Concept]
    public class RecycleStation : AbstractTransporter, IPassable
    {
        public bool Passable => false;

        public override long RecycleForAGoldOre => 100;

        public override Type LinkTypeRestriction => typeof(Discardable);
    }
}

