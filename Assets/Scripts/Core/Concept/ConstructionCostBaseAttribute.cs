

using System;

namespace Weathering
{
    public struct CostInfo
    {
        public Type CostType;
        public long BaseCostQuantity;
        public long RealCostQuantity;
        public long CostMultiplier;
        public long CountForDoubleCost;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConstructionCostBaseAttribute : Attribute
    {
        public readonly Type CostType;
        public readonly long CostQuantity;
        public readonly long CountForDoubleCost;
        public ConstructionCostBaseAttribute(Type costType, long costQuantity, long costForDoubleCost = 10) {
            if (costType == null) throw new Exception(ToString());
            if (costQuantity < 0) throw new Exception(ToString());
            if (costQuantity == 0 && costForDoubleCost != 0) throw new Exception(ToString());
            if (costForDoubleCost < 0 || CountForDoubleCost >= 10000) throw new Exception(ToString());
            CostType = costType;
            CostQuantity = costQuantity;
            CountForDoubleCost = costForDoubleCost;
        }
        public static ValueTuple<Type, long> GetCostBase(Type type) {
            ConstructionCostBaseAttribute attr = Tag.GetAttribute<ConstructionCostBaseAttribute>(type);
            if (attr == null) {
                return (null, 0);
            }
            return (attr.CostType, attr.CostQuantity);
        }
        public static CostInfo GetCost(Type type, IMap map, bool forConstruction) {
            CostInfo result = new CostInfo();
            ConstructionCostBaseAttribute attr = Tag.GetAttribute<ConstructionCostBaseAttribute>(type);
            if (attr == null) {
                return result;
            }
            result.CostType = attr.CostType;
            result.BaseCostQuantity = attr.CostQuantity;
            result.CostMultiplier = GetCostMultiplier(type, map, forConstruction, attr.CountForDoubleCost);
            result.RealCostQuantity = attr.CostQuantity * result.CostMultiplier;
            result.CountForDoubleCost = attr.CountForDoubleCost;
            return result;
        }
        public static long GetCostMultiplier(Type type, IMap map, bool forConstruction, long countForDoubleCost) {
            long count = map.Refs.GetOrCreate(type).Value; // Map.Ref.Get<建筑>.Value, 为建筑数量。Map.Ref.Get<资源>.Value, 为资源产量
            if (!forConstruction) {
                // 计算拆除返还费用, 与建筑费用有1count的差距。如count为10时, 建筑费用增加, 拆除费用不变
                count--;
            }
            if (count < 0) throw new Exception($"建筑数量为负 {type.Name} {count}");

            //// 10个以上建筑时, 才开始增加费用
            //count = Math.Max(count - 10, 0);

            const long maximun = long.MaxValue / 100000;

            long multiplier = 1;

            if (countForDoubleCost != 0) {
                long magic = countForDoubleCost;
                long magic10 = magic * 10;

                while (count / magic10 > 0) {
                    count -= magic10;
                    multiplier *= 1000;

                    if (multiplier > maximun) break;
                }
                while (count / magic > 0) {
                    count -= magic;
                    multiplier *= 2;

                    if (multiplier > maximun) break;
                }
            }

            return multiplier;
        }


        public static void ShowBuildingCostPage(Action back, IMap map, Type type) {
            var items = UI.Ins.GetItems();

            if (back != null) items.Add(UIItem.CreateReturnButton(back));

            CostInfo cost = ConstructionCostBaseAttribute.GetCost(type, map, true);
            if (cost.CostType != null) {
                items.Add(UIItem.CreateText($"当前建筑费用: {Localization.Ins.Val(cost.CostType, cost.RealCostQuantity)}"));
                items.Add(UIItem.CreateText($"建筑费用乘数: {cost.CostMultiplier}"));
                items.Add(UIItem.CreateText($"费用增长系数: {cost.CountForDoubleCost}"));
            }
            items.Add(UIItem.CreateText($"同类建筑数量: {map.Refs.Get(type).Value}"));

            UI.Ins.ShowItems($"{Localization.Ins.Get(type)}建筑费用", items);
        }
    }
}
