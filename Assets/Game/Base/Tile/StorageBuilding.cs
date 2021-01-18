
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class StorageBuilding : StandardTile
    {
        public override string SpriteKey => "StorageBuilding";

        public override void OnEnable() {
            base.OnEnable();
            if (Refs == null) {
                Refs = Weathering.Refs.Create();
            }
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            throw new NotImplementedException();
        }
    }
}

