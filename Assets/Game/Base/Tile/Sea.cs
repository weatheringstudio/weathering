

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
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
                UI.Ins.UIItems("深海", new List<IUIItem>() { 
                    new UIItem {
                        Content = "海再深，也可以填成陆地",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
            else if (seaCount ==0) {
                UI.Ins.UIItems("湖泊", new List<IUIItem>() {
                    new UIItem {
                        Content = "湖泊，波光粼粼",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
            else {
                UI.Ins.UIItems("海岸", new List<IUIItem>() {
                    new UIItem {
                        Content = "海边，可以钓鱼划船造港口",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
        }
    }
}

