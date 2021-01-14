
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    //[Concept("农田")]
    //public class Crop : ITileDefinition
    //{
    //    public string SpriteKey => typeof(Crop).Name;

    //    public IValues Values { get; private set; } = null;
    //    public void SetValues(IValues values) => Values = values;

    //    private IValue mapFood;
    //    public void OnTap() {
    //        string cropColoredName = Concept.Ins.ColoredNameOf<Crop>();
    //        string foodColoredName = Concept.Ins.ColoredNameOf<Food>();
    //        UI.Ins.UIItems(cropColoredName, new List<IUIItem>() {
    //                new UIItem {
    //                    Content = "一片" + cropColoredName +"，生产着" + foodColoredName,
    //                    Type = IUIItemType.Text,
    //                },
    //                new UIItem {
    //                    Content = foodColoredName,
    //                    Type = IUIItemType.ValueProgress,
    //                    Value = mapFood
    //                },
    //                new UIItem {
    //                    Content = foodColoredName,
    //                    Type = IUIItemType.TimeProgress,
    //                    Value = mapFood
    //                },
    //        });
    //    }

    //    public bool CanConstruct() => true;

    //    public bool CanDestruct() {
    //        return Map.Values.Get<Food>().Inc >= 1;
    //    }

    //    public IMap Map { get; set; }
    //    public UnityEngine.Vector2Int Pos { get; set; }
    //    public void OnConstruct() {
    //        mapFood = Map.Values.Get<Food>();

    //        mapFood.Val -= 10;
    //        mapFood.Inc += 1;
    //        mapFood.Del = 1 * Value.Minute;

    //        IValue satiety = Map.Values.Get<Labor>();
    //        satiety.Val -= 50;
    //    }

    //    public void OnDestruct() {
    //        mapFood.Inc -= 1;
    //        mapFood.Del = 1 * Value.Minute;
    //    }

    //    public void Initialize() {
    //    }
    //}
}

