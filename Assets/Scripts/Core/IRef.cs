
using System;

namespace Weathering
{
    public interface IRef
    {
        Type Type { get; set; }
        Type BaseType { get; set; }

        long Value { get; set; }
        long BaseValue { get; set; }

        Type Left { get; set; }
        Type Right { get; set; }
        long X { get; set; }
        long Y { get; set; }
    }

    public class RefData
    {
        public string base_type;
        public string type;
        public long base_val;
        public long val;
        public string left;
        public string right;
        public long x;
        public long y;
    }

    public class Ref : IRef
    {
        public Type BaseType { get; set; } = null;
        public Type Type { get; set; } = null;
        public long BaseValue { get; set; } = 0;
        public long Value { get; set; } = 0;

        public Type Left { get; set; } = null;
        public Type Right { get; set; } = null;
        public long X { get; set; } = 0;
        public long Y { get; set; } = 0;

        public static RefData ToData(IRef r) {
            return new RefData {
                // base_type = r.BaseType.FullName,
                type = r.Type?.FullName,
                base_val = r.BaseValue,
                val = r.Value,

                left = r.Left?.FullName,
                right = r.Right?.FullName,
                x = r.X,
                y = r.Y,
            };
        }
        public static IRef FromData(RefData rData) {
            return Create(
                rData.base_type == null ? null : Type.GetType(rData.base_type),
                rData.type == null ? null : Type.GetType(rData.type),
                rData.base_val,
                rData.val,
                rData.left == null ? null : Type.GetType(rData.left), 
                rData.right == null ? null : Type.GetType(rData.right),
                rData.x, 
                rData.y
                );
        }

        public static Ref Create(
            Type base_type,
            Type type,
            long base_value,
            long value,

            Type left, 
            Type right, 
            long x, 
            long y
            ) {
            return new Ref {
                BaseType = base_type,
                Type = type,
                BaseValue = base_value,
                Value = value,

                Left = left,
                Right = right,
                X = x,
                Y = y,
            };
        }
    }
}

