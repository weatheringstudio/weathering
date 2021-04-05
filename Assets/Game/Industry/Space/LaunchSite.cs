

using UnityEngine;

namespace Weathering
{

    public class LaunchSite : StandardTile
    {
        public override string SpriteKey => "PlanetLander";
        public override void OnTap() {
            ISavable savable = GameEntry.Ins.GetParentMap(typeof(MapOfStarSystem), Map);
            savable.Inventory.Add<Worker>(1);
            Debug.LogWarning((savable as IMapDefinition).MapKey);

        }
    }
}
