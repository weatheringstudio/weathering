
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class BaseCount { }

    [Concept("基地", "88BAFF")]
    public class Base : StandardTile
    {
        private float spriteFramerate = 0.2f;
        private string spriteKeyBase = "Wardenclyffe";
        private int spriteCount = 6;
        public override string SpriteKey {
            get {
                return spriteKeyBase + Utility.GetFrame(spriteFramerate, spriteCount).ToString();
            }
        }

        public const long BaseLaborMax = 100;
        public const long BaseLaborInc = 1;
        public const long BaseFoodMax = 100;
        public const long BaseWoodMax = 100;
        public const long BaseStoneMax = 100;

        public override void OnConstruct() {
            IValue labor = Map.Values.Get<Labor>();
            labor.Max += BaseLaborMax;
            labor.Inc += BaseLaborInc;

            IValue food = Map.Values.Get<Food>();
            food.Max += BaseFoodMax;

            IValue wood = Map.Values.Get<Wood>();
            wood.Max += BaseWoodMax;

            IValue stone = Map.Values.Get<Stone>();
            stone.Max += BaseWoodMax;

            Map.Values.Get<BaseCount>().Max++;
        }

        public override void OnDestruct() {
            IValue labor = Map.Values.Get<Labor>();
            labor.Max -= BaseLaborMax;
            labor.Inc -= BaseLaborInc;

            IValue food = Map.Values.Get<Food>();
            food.Max -= BaseFoodMax;

            IValue wood = Map.Values.Get<Wood>();
            wood.Max -= BaseWoodMax;

            IValue stone = Map.Values.Get<Stone>();
            stone.Max -= BaseWoodMax;

            Map.Values.Get<BaseCount>().Max--;
        }

        public override void OnTap() {

            string foodColoredName = Concept.Ins.ColoredNameOf<Food>();
            string woodColoredName = Concept.Ins.ColoredNameOf<Wood>();
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
                    Type = IUIItemType.MultilineText,
                    Content = $"{baseColoredName}能储存少量{woodColoredName}",
                },
                new UIItem {
                    Content = woodColoredName,
                    Type = IUIItemType.ValueProgress,
                    Value = Map.Values.Get<Wood>()
                },
                new UIItem {
                    Type = IUIItemType.Image,
                    Content = "icon",
                    DynamicContent = () => {
                        return SpriteKey;
                    },
                    Scale = 8,
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
                    Content = "多地图功能测试",
                    Type = IUIItemType.Button,
                    OnTap = () => {
                        //if (MapView.Ins.Map is InitialMap) {
                        //    GameEntry.Ins.GotoMap(typeof(TestMap));
                        //}
                        //else {
                        //    GameEntry.Ins.GotoMap(typeof(InitialMap));
                        //}
                        UI.Ins.Active = false;
                    }
                },
                new UIItem {
                    Content = "保存游戏",
                    Type = IUIItemType.Button,
                    OnTap = UIDecorator.InformAfter(GameEntry.Ins.Save, "已经保存"),
                },
                new UIItem {
                    Content = "重置存档",
                    Type = IUIItemType.Button,
                    OnTap = UIDecorator.ConfirmBefore(GameEntry.Ins.DeleteSave, "确认重置存档吗？"),
                }
            });
        }
    }
}

