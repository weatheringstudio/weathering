
using System;
using System.Collections.Generic;

namespace Weathering
{
    public class Wood : ITileDefinition
    {
        public string SpriteKey => typeof(Wood).Name;

        public IValues Values { get; private set; } = null;
        public void SetValues(IValues values) => Values = values;

        public IMap Map { get; set; }
        public UnityEngine.Vector2Int Pos { get; set; }

        public bool CanConstruct() => true;

        public bool CanDestruct() => true;

        public void Initialize() {
        }

        public void OnConstruct() {
        }

        public void OnDestruct() {
        }

        public void OnTap() {

            if (Map.Values.Get<BaseCount>().Max == 0) {
                UI.Ins.UIItems("森林", new List<IUIItem> {
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "一片森林，可以开采木材，但是基地不能造在这里",
                },
            });
            } else {
                UI.Ins.UIItems("森林", new List<IUIItem> {
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = "一片森林，可以开采木材",
                },
            });
            }
        }
    }
}

