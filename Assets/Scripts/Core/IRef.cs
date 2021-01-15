
using System;

namespace Weathering
{
    public interface IRef
    {
        Type Type { get; set; }
        long X { get; set; }
        long Y { get; set; }
    }

    public class RefData
    {
        public string Type;
        public long X;
        public long Y;
    }

    public class Ref : IRef
    {
        public Type Type { get; set; } = null;
        public long X { get; set; } = 0;
        public long Y { get; set; } = 0;

        public static RefData ToData(IRef r) {
            return new RefData {
                Type = r.Type.FullName,
                X = r.X,
                Y = r.Y,
            };
        }
        public static IRef FromData(RefData rData) {
            return Create(Type.GetType(rData.Type), rData.X, rData.Y);
        }

        public static Ref Create(Type type, long X, long Y) {
            return new Ref {
                Type = type,
                X = X,
                Y = Y,
            };
        }
    }
}

