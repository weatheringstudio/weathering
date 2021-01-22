
using System;
using System.Collections.Generic;

namespace Weathering
{
    public interface ITile : ISavable
    {
        void OnTap();
        void OnTapPlaySound();

        bool CanConstruct();
        bool CanDestruct();

    }

    public interface ITileDefinition : ITile, ISavableDefinition
    {
        int HashCode { get; }
        string SpriteKey { get; }

        IMap Map { get; set; }
        UnityEngine.Vector2Int Pos { get; set; }

        void OnConstruct();
        void OnDestruct();

        void OnEnable();

    }

}








