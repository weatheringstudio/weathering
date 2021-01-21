

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("海岸", "B1CFFF")]
    class Coast { }
    [Concept("深海", "5C89D2")]
    class DeepSea { }
    [Concept("湖泊", "B1CFFF")]
    class Lake { }
    public class Sea : StandardTile
    {
        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate6x8RuleTileIndex(typeof(Sea), Map, Pos);
                return "Sea_" + index.ToString();
            }
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
                UI.Ins.ShowItems(Name + Concept.Ins.ColoredNameOf<DeepSea>(), new List<IUIItem>() { 
                    new UIItem {
                        Content = "海再深，也可以填成陆地",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
            else if (seaCount ==0) {
                UI.Ins.ShowItems(Name + Concept.Ins.ColoredNameOf<Lake>(), new List<IUIItem>() {
                    new UIItem {
                        Content = "湖泊，波光粼粼，一碧万顷",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
            else {
                UI.Ins.ShowItems(Name + Concept.Ins.ColoredNameOf<Coast>(), new List<IUIItem>() {
                    new UIItem {
                        Content = "海边，可以钓鱼划船造港口",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
        }
    }
}

