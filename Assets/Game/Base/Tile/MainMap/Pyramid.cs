
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept("金字塔", "c69f27 ")]
    public class Pyramid : StandardTile
    {
        public override string SpriteKey {
            get {
                return $"{typeof(Pyramid).Name}Stage{stage.Max}";
            }
        }

        private static bool initialied = false;
        private static string pyramid;
        private IValue stage;
        public override void OnEnable() {
            base.OnEnable();
            if (!initialied) {
                pyramid = Concept.Ins.ColoredNameOf<Pyramid>();
            }
            stage = Values.Get<Stage>();
        }

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            stage = Values.Create<Stage>();
            stage.Max = 0;
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            // var items = new List<IUIItem>();

            stage.Max++;
            if (stage.Max == 4) {
                Map.UpdateAt<EmptyTile>(Pos);
            }

            // UI.Ins.ShowItems(pyramid, items);
        }
    }
}

