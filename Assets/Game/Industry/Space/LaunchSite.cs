

using System;
using UnityEngine;

namespace Weathering
{
    /// <summary>
    /// 发射台的作用：Inventory => Space.Inventory (non supply)
    /// 太空电梯作用：
    /// </summary>
    public class LaunchSite : AbstractTransporter
    {
        public override Type LinkTypeRestriction => typeof(DiscardableSolid);

        public override bool UseSelfInventoryOrSpaceInventory => false;
        public override string SpriteKey => typeof(LaunchSite).Name;

        //public override void OnTap() {
        //    ITile parentTile = Map.ParentTile; // 需要更简单不容易出错的方法访问

        //    //Debug.LogWarning((Map as IMapDefinition).MapKey);

        //    //Debug.LogWarning(parentTile.GetPos());

        //    //Debug.LogWarning(parentTile.GetType().Name);

        //    //Debug.LogWarning((parentTile as MapOfStarSystemDefaultTile).CelestialBodyName);

        //    var items = UI.Ins.GetItems();



        //    UI.Ins.ShowItems(Localization.Ins.Get<LaunchSite>(), items);
        //}
    }
}
