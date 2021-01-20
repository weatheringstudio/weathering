
using System;

namespace Weathering
{
    public interface IRef
    {
        Type Type { get; set; }
        Type Left { get; set; }
        Type Right { get; set; }
        long X { get; set; }
        long Y { get; set; }
        long Value { get; set; }
    }

    public class RefData
    {
        public string type;
        public string left;
        public string right;
        public long val;
        public long x;
        public long y;
    }

    public class Ref : IRef
    {
        public Type Type { get; set; } = null;
        public Type Left { get; set; } = null;
        public Type Right { get; set; } = null;
        public long X { get; set; } = 0;
        public long Y { get; set; } = 0;
        public long Value { get; set; }

        public static RefData ToData(IRef r) {
            return new RefData {
                type = r.Type?.FullName,
                left = r.Left?.FullName,
                right = r.Right?.FullName,
                x = r.X,
                y = r.Y,
                val = r.Value
            };
        }
        public static IRef FromData(RefData rData) {
            return Create(
                rData.type == null ? null : Type.GetType(rData.type),
                rData.left == null ? null : Type.GetType(rData.left), 
                rData.right == null ? null : Type.GetType(rData.right),
                rData.x, rData.y, rData.val);
        }

        public static Ref Create(Type type, Type left, Type right, long X, long Y, long V) {
            return new Ref {
                Type = type,
                Left = left,
                Right = right,
                X = X,
                Y = Y,
                Value = V
            };
        }
    }
}

