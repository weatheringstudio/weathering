

using UnityEngine;

namespace Weathering
{

    public class LaunchSite : StandardTile
    {
        public override string SpriteKey => "PlanetLander";
        public override void OnTap() {
            ITile parentTile = Map.ParentTile; // 需要更简单不容易出错的方法访问

            Debug.LogWarning((Map as IMapDefinition).MapKey);

            Debug.LogWarning(parentTile.GetPos());

            Debug.LogWarning(parentTile.GetType().Name);

            Debug.LogWarning((parentTile as MapOfStarSystemDefaultTile).CelestialBodyName);
        }
    }
}
