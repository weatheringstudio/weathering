
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Concept]
    public class CellarForPersonalStorage : AbstractTransporter, IPassable
    {
        public bool Passable => true;


        public override Type LinkTypeRestriction => typeof(Discardable);
    }
}

