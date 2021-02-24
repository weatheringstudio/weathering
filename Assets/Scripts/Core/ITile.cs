
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weathering
{
    public interface ITile : ISavable
    {
        void OnTap();
        void OnTapPlaySound();

        bool CanConstruct();
        bool CanDestruct();

        IMap GetMap();
        UnityEngine.Vector2Int GetPos();
    }

    public interface ITileDefinition : ITile, ISavableDefinition
    {
        bool HasSpriteDirection { get; }
        bool NeedUpdateSpriteKeys { get; set; }
        int NeedUpdateSpriteKeysPositionX { get; set; }
        int NeedUpdateSpriteKeysPositionY { get; set; }
        Tile TileSpriteKeyBuffer { get; set; }
        string SpriteKeyBase { get; }
        string SpriteKey { get; }
        string SpriteKeyOverlay { get; }
        string SpriteLeft { get; }
        string SpriteRight { get; }
        string SpriteUp { get; }
        string SpriteDown { get; }

        IMap Map { get; set; }
        UnityEngine.Vector2Int Pos { get; set; }
        uint HashCode { get; set; }

        void OnConstruct();
        void OnDestruct();

        void OnEnable();

    }

}








