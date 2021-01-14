
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Mountain : ITileDefinition
    {
        public string SpriteKey => typeof(Mountain).Name;

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
                UI.Ins.UIItems("高山", new List<IUIItem>() {
                    new UIItem {
                        Content = "地势崎岖，不适合建造基地",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }
            else {
                UI.Ins.UIItems("高山", new List<IUIItem>() {
                    new UIItem {
                        Content = "哇！好多的矿石",
                        Type = IUIItemType.MultilineText,
                    }
                });
            }

        }
    }
}

