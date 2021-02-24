
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IProvider
    {
        Tuple<Type, long> CanProvide { get; }
        void Provide(Tuple<Type, long> items);
    }

    public interface IConsumer
    {
        Tuple<Type, long> CanConsume { get; }
        void Consume(Tuple<Type, long> items);
    }

    public interface IInputLeft { }
    public interface IInputRight { }
    public interface IInputDown { }
    public interface IInputUp { }

    public interface IOutputLeft { }
    public interface IOutputRight { }
    public interface IOutputDown { }
    public interface IOutputUp { }

    public class Road : StandardTile
    {
        public override string SpriteLeft => base.SpriteLeft;
        public override string SpriteRight => base.SpriteRight;
        public override string SpriteUp => base.SpriteUp;
        public override string SpriteDown => base.SpriteDown;

        public override string SpriteKey {
            get {
                Func<ITile, bool> predicate = tile => {
                    return tile as Road != null;
                    //if (tile as IRoadlike != null) {
                    //    return true;
                    //}
                    //if (tile as ILookLikeRoad != null && (tile as ILookLikeRoad).LookLikeRoad) {
                    //    return true;
                    //}
                    //return false;
                };

                int index = TileUtility.Calculate4x4RuleTileIndex(predicate, Map, Pos);
                return $"StoneRoad_{index}";
            }
        }
        public override void OnTap() {
        }
    }
}

