

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [BindTerrainType(TerrainType.Sea)]
    /// <summary>
    /// TransportStation的特征：物流输入任意，背包输出对应的东西
    /// TransportStationDest的特征：输入背包里的东西，物流输出对应的东西
    /// </summary>
    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class TransportStationDestPort : AbstractTransportStationDest
    {
        protected override long Capacity => 10;
    }
}
