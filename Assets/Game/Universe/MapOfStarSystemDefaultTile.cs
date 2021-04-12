

using System;

namespace Weathering
{
    public interface ICelestialBodyType { }

    public interface IPlanetType : ICelestialBodyType { }

    public class PlanetBarren : IPlanetType { }
    public class PlanetArid : IPlanetType { }
    public class PlanetOcean : IPlanetType { }
    public class PlanetLava : IPlanetType { }
    public class PlanetIce : IPlanetType { }
    public class PlanetContinental : IPlanetType { }
    public class PlanetGaia : IPlanetType { }
    public class PlanetSuperDimensional : IPlanetType { }

    public interface IGasGiantType { }
    public class GasGiant : IGasGiantType { }
    public class GasGiantRinged : IGasGiantType { }

    public class Asteroid : ICelestialBodyType { }

    public class SpaceEmptiness : ICelestialBodyType { }

    public class MapOfStarSystemDefaultTile : StandardTile, IDontSave, ITileDescription, IHasFrameAnimationOnSpriteKey
    {

        public bool DontSave => true;
        public string TileDescription => null;



        public override string SpriteKeyOverlay {
            get {
                if (IsCelestialBody) {
                    if (CelestialBodyType == typeof(Asteroid)) {
                        return IsCelestialBody ? $"{CelestialBodyName}_{(inversedAnimation * MapView.Ins.AnimationIndex + HashCode) % 64 + 64 * asteroidOffset}" : null;
                    } else {
                        return IsCelestialBody ? $"{CelestialBodyName}_{(inversedAnimation * MapView.Ins.AnimationIndex + HashCode) % 64}" : null;
                    }
                }
                return null;
            }
        }
        public int HasFrameAnimation => IsCelestialBody ? slowedAnimation : 0;
        private bool IsCelestialBody => CelestialBodyType != typeof(SpaceEmptiness);

        private int inversedAnimation = 1;
        private int slowedAnimation = 1;


        public Type CelestialBodyType { get; private set; }
        public string CelestialBodyName { get; private set; }


        private int asteroidOffset = 0;

        public override void OnEnable() {
            uint hashcode = HashUtility.Hash(HashCode);

            uint starHashcode = 0;

            IStarPosition starPosition = Map as IStarPosition;
            if (starPosition == null) throw new Exception();

            // 恒星检验
            bool isStar = (starPosition.X == Pos.x && starPosition.Y == Pos.y) || (starPosition.HasSecondStar && starPosition.SecondStarX == Pos.x && starPosition.SecondStarY == Pos.y);
            if (isStar) {
                starHashcode = GameEntry.SelfMapKeyHashCode(Map);
                CelestialBodyType = MapOfGalaxyDefaultTile.CalculateStarType(starHashcode);
            }
            // 真空校验
            else if (HashUtility.Hashed(ref hashcode) % 50 != 0) {
                CelestialBodyType = typeof(SpaceEmptiness);
            }
            // 小行星
            else if (HashUtility.Hashed(ref hashcode) % 2 != 0) {
                CelestialBodyType = typeof(Asteroid);
                asteroidOffset = (ABS((int)hashcode)) % 4;
            }
            // 盖亚星球
            else if (HashUtility.Hashed(ref hashcode) % 40 == 0) {
                CelestialBodyType = typeof(PlanetGaia);
            }
            // 超维星球
            else if (HashUtility.Hashed(ref hashcode) % 40 == 0) {
                CelestialBodyType = typeof(PlanetSuperDimensional);
            }
            // 气态巨行星
            else if (HashUtility.Hashed(ref hashcode) % 10 == 0) {
                CelestialBodyType = typeof(GasGiant);
            }
            // 气态巨行星
            else if (HashUtility.Hashed(ref hashcode) % 9 == 0) {
                CelestialBodyType = typeof(GasGiantRinged);
            }
            // 类地
            else if (HashUtility.Hashed(ref hashcode) % 3 == 0) {
                CelestialBodyType = typeof(PlanetContinental);
            }
            // 荒芜
            else if (HashUtility.Hashed(ref hashcode) % 4 == 0) {
                CelestialBodyType = typeof(PlanetBarren);
            }
            // 干旱
            else if (HashUtility.Hashed(ref hashcode) % 3 == 0) {
                CelestialBodyType = typeof(PlanetArid);
            }
            // 冰冻
            else if (HashUtility.Hashed(ref hashcode) % 2 == 0) {
                CelestialBodyType = typeof(PlanetIce);
            }
            // 海洋
            else {
                CelestialBodyType = typeof(PlanetOcean);
            }

            CelestialBodyName = CelestialBodyType.Name;

            if (IsCelestialBody) {
                uint again = HashUtility.Hash(isStar ? starHashcode : HashCode);
                inversedAnimation = again % 2 == 0 ? 1 : -1;
                again = HashUtility.Hash(again);
                slowedAnimation = 1 + ABS((int)again % 5);
            }
        }
        private int ABS(int x) => x >= 0 ? x : -x;


        public override void OnTap() {
            var items = UI.Ins.GetItems();
            string title = Localization.Ins.Get(CelestialBodyType);

            if (
                CelestialBodyType == typeof(PlanetContinental) ||
                CelestialBodyType == typeof(PlanetArid) ||
                CelestialBodyType == typeof(PlanetLava) ||
                CelestialBodyType == typeof(PlanetOcean) ||
                CelestialBodyType == typeof(PlanetIce) ||
                CelestialBodyType == typeof(PlanetBarren)
                
                ) {
                uint childMapHashcode = GameEntry.ChildMapKeyHashCode(Map, Pos);
                items.Add(UIItem.CreateText($"此星球大小：{MapOfPlanet.CalculatePlanetSize(childMapHashcode)}"));
                items.Add(UIItem.CreateText($"此星球矿物稀疏度：{MapOfPlanet.CalculateMineralDensity(childMapHashcode)}"));
                items.Add(UIItem.CreateButton($"进入{title}", () => {
                    Map.EnterChildMap(Pos);
                }));
            } else if (CelestialBodyType != typeof(SpaceEmptiness)) {
                items.Add(UIItem.CreateText($"{Localization.Ins.Get(CelestialBodyType)}暂未开放"));
                items.Add(UIItem.CreateText($"只开放了普通行星"));
            } else {
                items.Add(UIItem.CreateButton($"离开此恒星系", () => {
                    Map.EnterParentMap();
                }));
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}
