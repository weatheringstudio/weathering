﻿
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Weathering
//{
//    // 水产
//    [ConceptDescription(typeof(AquaticProductDescription))]
//    [Depend(typeof(AnimalFlesh))]
//    [Concept]
//    public class AquaticProduct { }

//    [Depend(typeof(AnimalFleshSupply))]
//    [Concept]
//    public class AquaticProductSupply { }
//    [Concept]
//    public class AquaticProductDescription { }

//    // 鱼肉
//    [ConceptSupply(typeof(FishFleshSupply))]
//    [ConceptDescription(typeof(FishFleshDescription))]
//    [Depend(typeof(AquaticProduct))]
//    [Concept]
//    public class FishFlesh { }

//    [ConceptResource(typeof(FishFlesh))]
//    [Depend(typeof(AquaticProductSupply))]
//    [Concept]
//    public class FishFleshSupply { }
//    [Concept]
//    public class FishFleshDescription { }

//    public class SeaFishery : Factory
//    {
//        protected override bool PreserveLandscape => true;
//        public override string SpriteKey => DecoratedSpriteKey(typeof(SeaFishery).Name);
//        protected override (Type, long) Out0 => (typeof(FishFleshSupply), 1);
//        protected override long WorkerCost => 0;
//    }
//}

