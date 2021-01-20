
using System;
using System.Collections.Generic;

namespace Weathering
{
    public interface ITile : ISavable
    {
        void OnTap();
        bool CanConstruct();
        bool CanDestruct();
    }

    public interface ITileDefinition : ITile, ISavableDefinition
    {
        string Name { get; }

        IMap Map { get; set; }
        UnityEngine.Vector2Int Pos { get; set; }

        void OnConstruct();
        void OnDestruct();

        void OnEnable();

        string SpriteKey { get; }
    }

}








