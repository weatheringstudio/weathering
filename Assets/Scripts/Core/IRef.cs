
using System;

namespace Weathering
{
    public interface IRef
    {
        Type Type { get; set; }
        Type Another { get; set; }
        long X { get; set; }
        long Y { get; set; }
        long Value { get; set; }
    }

    public class RefData
    {
        public string type;
        public string another;
        public long val;
        public long x;
        public long y;
    }

    public class Ref : IRef
    {
        public Type Type { get; set; } = null;
        public Type Another { get; set; } = null;
        public long X { get; set; } = 0;
        public long Y { get; set; } = 0;
        public long Value { get; set; }

        public static RefData ToData(IRef r) {
            return new RefData {
                type = r.Type?.FullName,
                another = r.Another?.FullName,
                x = r.X,
                y = r.Y,
                val = r.Value
            };
        }
        public static IRef FromData(RefData rData) {
            return Create(
                rData.type == null ? null : Type.GetType(rData.type),
                rData.another == null ? null : Type.GetType(rData.another), 
                rData.x, rData.y, rData.val);
        }

        public static Ref Create(Type type, Type another, long X, long Y, long V) {
            return new Ref {
                Type = type,
                Another = another,
                X = X,
                Y = Y,
                Value = V
            };
        }
    }
}

