
using System;
using System.Collections.Generic;

namespace Weathering
{
    // 谷物
    [Depend(typeof(Food))]
    public class Grain { }


    [ConstructionCostBase(typeof(Grain), 10, 20)]
    [Concept]
    public class Farm : AbstractFactoryStatic, IPassable
    {
        protected override bool CanStoreSomething => true;
        protected override bool CanStoreOut0 => true;

        public bool Passable => true;

        public override string SpriteKeyRoad {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(this, (tile, direction) => { 
                    if (tile is Farm && tile is IRunnable runnable) {
                        return Running == runnable.Running;
                    }
                    return false;
                }
                );
                return $"{(Running ? "Farm" : "FarmGrowing")}_{index}";
            }
        }
        public override string SpriteKey => null;
        public override string SpriteKeyHighLight => null;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(Grain), 6);
    }
}

