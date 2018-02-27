using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mixspace.Lexicon.Samples;

using Strings = Mixspace.Lexicon.Actions.ClearMeasurementsStrings;

namespace Mixspace.Lexicon.Actions
{
    public class ClearMeasurementsAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            MeasurementLabel[] labels = GameObject.FindObjectsOfType<MeasurementLabel>();
            foreach (MeasurementLabel label in labels)
            {
                GameObject.Destroy(label.gameObject);
            }
            
            return false;
        }
    }
}
