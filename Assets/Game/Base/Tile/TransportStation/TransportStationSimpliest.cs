

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    /// <summary>
    /// TransportStation的特征：物流输入任意，背包输出对应的东西
    /// TransportStationDest的特征：输入背包里的东西，物流输出对应的东西
    /// </summary>
    public class TransportStationSimpliest : AbstractTransportStation
    {
        protected override long Capacity => 1;

        protected override long CostQuantity => 1;

        protected override Type CostType => typeof(Worker);
    }
}
