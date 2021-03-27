
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    //public class IronOreDeposit { }

    //public class CopperOreDeposit { }

    //public class StoneDeposit { }


    // 应该用class做，
    public enum MineralType
    {
        Iron, Copper, Silver, Gold,

        TiO2_Rutile, // 金红石
        CuFeS2_Chalcopyrite, // 黄铜矿
        Cu2OH2CO3_Malachite, // 孔雀石
        CuSO45H20_SulfatePentahydrate, // 胆矾 五水硫酸铜
        CaCO3_Calcite,
        NaAlSi2O6_Jade,

        AluminosilicateMineral, // 硅铝酸盐。。。云母。高岭土
        // Ca钙长石
        // K正长石
        // Mg蛇纹石
        Lazurite, // 青金石

        Galena_PbS, // 方铅矿
        PbSO4_Anglesite, // 铅矾

        Coal, CrudeOil, NaturalGas,
        Chromium_Cr_Chromite, Aluminum_Al_Bauxite, Barite_Ba_Barium,
        Platinum,
        RareEarth,
        Tungsten, Tin, Zinc, Surfur, Thorium,
        Gems,  Nickel, Quartz, Rutile, Lithia,
        Silicon, Ca, Na, K, Mg,
        Cinnabar_HgS,
        Uranium,

        NaCl_Salt,
        KNO3_Nitre,



        // 片麻岩。gneiss
        // 片岩，千枚岩。phyllite
        // 大理石 Marble
        // 花岗石 granite
        // 闪长岩
        // 辉长岩
        // 安山岩 andesite

        // 砾石
        // 砂石
        // 石英盐
        // 板岩
        // 页岩

        // 浮岩
        // 玄武岩/安山岩

        // 黑曜石
        // 燧石
        // 灰岩

    }


}

