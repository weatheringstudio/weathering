
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CharacterLanded { }

    public interface ILandable
    {
        bool Landable { get; }
        void Land(Vector2Int pos);
        void Leave();
    }

    public class GlobalProgress { }

    public class PlanetLander : StandardTile, IStepOn
    {
        public override string SpriteKey => typeof(PlanetLander).Name;

        public void OnStepOn() {
            ILandable landable = Map as ILandable;
            UI.Ins.ShowItems("是否乘坐火箭进入行星轨道",
                UIItem.CreateButton("开启火箭", () => {
                    Map.UpdateAt<TerrainDefault>(Pos);
                    UI.Ins.Active = false;
                    landable.Leave();
                }),
                UIItem.CreateButton("暂不开启", () => {
                    UI.Ins.Active = false;
                })
            );
        }

        public override void OnTap() {
            var items = new List<IUIItem>();


            UI.Ins.ShowItems("【火箭】", items);
        }
    }
}

