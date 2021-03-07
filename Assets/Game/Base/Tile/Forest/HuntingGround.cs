
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    // 兽肉
    [ConceptDescription(typeof(AnimalFleshDescription))]
    [Depend(typeof(AnimalFlesh))]
    [Concept]
    public class Meat { }

    [Depend(typeof(AnimalFleshSupply))]
    [Concept]
    public class MeatSupply { }
    [Concept]
    public class MeatDescription { }

    // 鹿肉
    [ConceptSupply(typeof(DeerMeatSupply))]
    [ConceptDescription(typeof(DeerMeatDescription))]
    [Depend(typeof(Meat))]
    [Concept]
    public class DeerMeat { }
    [ConceptResource(typeof(DeerMeat))]
    [Depend(typeof(MeatSupply))]
    [Concept]
    public class DeerMeatSupply { }
    [Concept]
    public class DeerMeatDescription { }

    // 兔肉
    [ConceptSupply(typeof(RabbitMeatSupply))]
    [ConceptDescription(typeof(RabbitMeatDescription))]
    [Depend(typeof(Meat))]
    [Concept]
    public class RabbitMeat { }
    [ConceptResource(typeof(RabbitMeat))]
    [Depend(typeof(MeatSupply))]
    [Concept]
    public class RabbitMeatSupply { }
    [Concept]
    public class RabbitMeatDescription { }


    [Concept]
    public class HuntingGround : Factory1OutWithoutWorker, ILinkProvider
    {
        public override string SpriteKey => typeof(HuntingGround).Name;

        protected override Type Type => typeof(DeerMeatSupply);

        protected override long BaseValue => 1;
    }
}

