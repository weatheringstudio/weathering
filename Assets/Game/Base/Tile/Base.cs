
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class BaseCount { }

    [Concept("基地", "88BAFF")]
    public class Base : ITileDefinition
    {
        private float framerate = 0.2f;
        private string spriteKeyBase = "Wardenclyffe";
        private int spriteCount = 6;
        public string SpriteKey {
            get {
                return spriteKeyBase + Utility.GetFrame(framerate, spriteCount).ToString();
            }
        }

        public IValues Values { get; private set; } = null;
        public void SetValues(IValues values) => Values = values;

        public IMap Map { get; set; }
        public UnityEngine.Vector2Int Pos { get; set; }

        public bool CanConstruct() => true;

        public bool CanDestruct() => true;

        public const long BaseLaborMax = 100;
        public const long BaseLaborInc = 1;
        public const long BaseFoodMax = 100;

        public void Initialize() {
        }
        public void OnConstruct() {
            IValue labor = Map.Values.Get<Labor>();
            labor.Max += BaseLaborMax;
            labor.Inc += BaseLaborInc;

            IValue food = Map.Values.Get<Food>();
            food.Max += BaseFoodMax;

            Map.Values.Get<BaseCount>().Max++;
        }

        public void OnDestruct() {
            IValue labor = Map.Values.Get<Labor>();
            labor.Max -= BaseLaborMax;
            labor.Inc -= BaseLaborInc;

            IValue food = Map.Values.Get<Food>();
            food.Max -= BaseFoodMax;

            Map.Values.Get<BaseCount>().Max--;
        }

        public void OnTap() {

            string foodColoredName = Concept.Ins.ColoredNameOf<Food>();
            string laborColoredName = Concept.Ins.ColoredNameOf<Labor>();
            string baseColoredName = Concept.Ins.ColoredNameOf<Base>();
            IValue labor = Map.Values.Get<Labor>();
            UI.Ins.UIItems(baseColoredName, new List<IUIItem> {
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = $"亲自在{baseColoredName}附近工作，提供{laborColoredName}",
                },
                new UIItem {
                    Content = laborColoredName,
                    Type = IUIItemType.ValueProgress,
                    Value = labor
                },
                new UIItem {
                    Content = laborColoredName,
                    Type = IUIItemType.TimeProgress,
                    Value = labor
                },
                new UIItem {
                    Type = IUIItemType.MultilineText,
                    Content = $"{baseColoredName}能储存少量{foodColoredName}",
                },
                new UIItem {
                    Content = foodColoredName,
                    Type = IUIItemType.ValueProgress,
                    Value = Map.Values.Get<Food>()
                },
                new UIItem {
                    Content = Concept.Ins.ColoredNameOf<Destruct>()+Concept.Ins.ColoredNameOf<Base>(),
                    Type = IUIItemType.Button,
                    OnTap = () => {
                        Map.UpdateAt<Grassland>(Pos);
                        UI.Ins.Active = false;
                    }
                },
                new UIItem {
                    Content = "保存游戏",
                    Type = IUIItemType.Button,
                    OnTap = GameEntry.Ins.Save
                },
                new UIItem {
                    Content = "重置存档",
                    Type = IUIItemType.Button,
                    OnTap = GameEntry.Ins.DeleteSave
                }
            });
        }
    }
}

