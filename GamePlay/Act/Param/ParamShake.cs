using UnityEngine;
using System.Collections;
using System;

namespace Cfg.Act
{
    public class ParamShake : ParamBase
    {
        public ECameraShake Type = ECameraShake.H;
        public float Amplitude;
        public float AmplitudeAttenuation;
        public float Frequency;
        public float FrequencyKeepDuration;
        public float FrequencyAttenuation;
        public float LiftTime = 1;

        public override void Save(ref string s)
        {
            this.Set(0, (int)Type, ref s);
            this.Set(1, Amplitude, ref s);
            this.Set(2, AmplitudeAttenuation, ref s);
            this.Set(3, Frequency, ref s);
            this.Set(4, FrequencyKeepDuration, ref s);
            this.Set(5, FrequencyAttenuation, ref s);
            this.Set(6, LiftTime, ref s);
        }

        public override void Read(string[] array)
        {
            this.Type                  = (ECameraShake)DHelper.ReadInt32((Get(0, array)));
            this.Amplitude             = DHelper.ReadFloat(Get(1, array));
            this.AmplitudeAttenuation  = DHelper.ReadFloat(Get(2, array));
            this.Frequency             = DHelper.ReadFloat(Get(3, array));
            this.FrequencyKeepDuration = DHelper.ReadFloat(Get(4, array));
            this.FrequencyAttenuation  = DHelper.ReadFloat(Get(5, array));
            this.LiftTime              = DHelper.ReadFloat(Get(6, array));
        }
    }
}

