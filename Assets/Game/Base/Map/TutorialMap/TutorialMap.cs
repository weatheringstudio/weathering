
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Depend]
    [ConceptDescription(typeof(TutorialMapTheBookDescription))]
    [Concept]
    public class Book { }

    [Depend(typeof(Book))]
    [ConceptDescription(typeof(TutorialMapTheBookDescription))]
    [Concept]
    public class TutorialMapTheBook { }
    [Concept]
    public class TutorialMapTheBookDescription { }

    [Depend(typeof(Book))]
    [ConceptDescription(typeof(TutorialMapTheDiaryDescription))]
    [Concept]
    public class TutorialMapTheDiary { }
    [Concept]
    public class TutorialMapTheDiaryDescription { }


    [Depend(typeof(Book))]
    [ConceptDescription(typeof(TutorialMapTheCurseDescription))]
    [Concept]
    public class TutorialMapTheCurse { }
    [Concept]
    public class TutorialMapTheCurseDescription { }

    public class TutorialMap : StandardMap
    {
        public override Type DefaultTileType => typeof(TutorialTile);
        public override int Width => 32;

        public override int Height => 32;

        public override Type GenerateTileType(Vector2Int pos) {
            return typeof(TutorialTile);
        }

        public override void OnConstruct() {
            base.OnConstruct();
            SetClearColor(new Color(124 / 255f, 181 / 255f, 43 / 255f));

        }
    }
}

