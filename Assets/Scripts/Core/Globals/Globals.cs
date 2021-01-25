
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IGlobals
    {
        IValues Values { get; }
        IRefs Refs { get; }
    }

    public interface IGlobalsDefinition : IGlobals
    {
        IValues ValuesInternal { get; set; }
        IRefs RefsInternal { get; set; }
    }

    public class Globals : MonoBehaviour, IGlobalsDefinition
    {
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }
        public static IGlobals Ins;

        private static IValue sanity;
        public static IValue Sanity {
            get {
                if (sanity == null) sanity = Ins.Values.Get<Sanity>();
                return sanity;
            }
        }

        public IValues ValuesInternal { get; set; }
        public void SetValues(IValues values) => ValuesInternal = values;
        public IRefs RefsInternal { get; set; }
        public void SetRefs(IRefs refs) => RefsInternal = refs;
        public IValues Values => ValuesInternal;
        public IRefs Refs => RefsInternal;
    }
}

