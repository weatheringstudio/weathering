
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

        bool NeedUpdateSpriteKeys { get; set; }

        // bool CanConstruct();
        bool CanDestruct();

        IMap GetMap();
        Vector2Int GetPos();
        uint GetTileHashCode();
    }

    public interface ITileDefinition : ITile, ISavableDefinition
    {
        int NeedUpdateSpriteKeysPositionX { get; set; }
        int NeedUpdateSpriteKeysPositionY { get; set; }


        string SpriteKeyBedrock { get; }
        Tile TileSpriteKeyBedrockBuffer { get; set; }
        string SpriteKeyWater { get; }
        Tile TileSpriteKeyWaterBuffer { get; set; }
        string SpriteKeyGrass { get; }
        Tile TileSpriteKeyGrassBuffer { get; set; }
        string SpriteKeyTree { get; }
        Tile TileSpriteKeyTreeBuffer { get; set; }
        string SpriteKeyHill { get; }
        Tile TileSpriteKeyHillBuffer { get; set; }
        string SpriteKeyRoad { get; }
        Tile TileSpriteKeyRoadBuffer { get; set; }

        string SpriteLeft { get; }
        Tile TileSpriteKeyLeftBuffer { get; set; }
        string SpriteRight { get; }
        Tile TileSpriteKeyRightBuffer { get; set; }
        string SpriteUp { get; }
        Tile TileSpriteKeyUpBuffer { get; set; }
        string SpriteDown { get; }
        Tile TileSpriteKeyDownBuffer { get; set; }

        string SpriteKey { get; }
        Tile TileSpriteKeyBuffer { get; set; }
        string SpriteKeyOverlay { get; }
        Tile TileSpriteKeyOverlayBuffer { get; set; }

        IMap Map { get; set; }
        UnityEngine.Vector2Int Pos { get; set; }
        uint TileHashCode { get; set; }

        void OnConstruct(ITile oldTile);
        void OnDestruct(ITile newTile);

        void OnEnable();

    }

}








