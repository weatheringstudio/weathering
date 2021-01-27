

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    class Coast { }
    [Concept]
    class DeepSea { }
    [Concept]
    class Lake { }

    public class Sea : StandardTile
    {
        private int index = 0;
        public override string SpriteKey {
            get {
                index = TileUtility.Calculate6x8RuleTileIndex(typeof(Sea), Map, Pos);
                if (index == 9) {
                    if (HasWhale()) {
                        return "SeaWhale";
                    }
                }
                return "Sea_" + index.ToString();
            }
        }
        private bool HasWhale() {
            return HashCode % 100 == 0;
        }

        public override bool CanConstruct() => true;

        public override bool CanDestruct() => true;

        public override void OnEnable() {
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {

            if (index == 9 && HasWhale()) {
                UI.Ins.ShowItems(Localization.Ins.Get<DeepSea>(), new List<IUIItem>() {
                    new UIItem {
                        Content = "发现一只鲸鱼",
                        Type = IUIItemType.MultilineText,
                    }
                });
                return;
            }

            ITile left = Map.Get(Pos + Vector2Int.left);
            ITile right = Map.Get(Pos + Vector2Int.right);
            ITile up = Map.Get(Pos + Vector2Int.up);
            ITile down = Map.Get(Pos + Vector2Int.down);

            int seaCount = 0;
            if (left is Sea) seaCount++;
            if (right is Sea) seaCount++;
            if (up is Sea) seaCount++;
            if (down is Sea) seaCount++;

            if (seaCount == 4) {
                UI.Ins.ShowItems(Localization.Ins.Get<DeepSea>(), new List<IUIItem>() { 
                    new UIItem {
                        Content = "海再深，也可以填成陆地",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
            else if (seaCount ==0) {
                UI.Ins.ShowItems(Localization.Ins.Get<Lake>(), new List<IUIItem>() {
                    new UIItem {
                        Content = "湖泊，波光粼粼，一碧万顷",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
            else {
                UI.Ins.ShowItems(Localization.Ins.Get<Coast>(), new List<IUIItem>() {
                    new UIItem {
                        Content = "海边，可以钓鱼划船造港口",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
        }
    }
}

