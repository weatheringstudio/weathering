
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

    public class PlanetLander : StandardTile
    {
        public override string SpriteKey => typeof(PlanetLander).Name;

        public override void OnTap() {
            ILandable landable = Map as ILandable;
            UI.Ins.ShowItems("是否乘坐火箭进入轨道",
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
    }
}

