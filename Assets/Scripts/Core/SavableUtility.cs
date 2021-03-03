

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Debit { }
    public class Credit { }
    
	public class SavableUtility : MonoBehaviour
	{
        // source的inc到dest
        public static void TakeIncFrom(Type type, ISavable dest, ISavable source, long val) {
            IValue sourceValue = source.Values.Get(type);
            IValue destValue = dest.Values.Get(type);

            if (val > sourceValue.Sur) {
                throw new Exception();
            }
            sourceValue.Dec += val;
            destValue.Inc += val;


            throw new Exception();
        }
        // dest的inc还给source
        public static void ReturnIncTo(Type type, ISavable dest, ISavable source, long val) {
            IValue sourceValue = source.Values.Get(type);
            IValue destValue = dest.Values.Get(type);

            if (val > sourceValue.Sur) {
                throw new Exception();
            }
            sourceValue.Dec += val;
            destValue.Inc += val;
        }
    }
}

