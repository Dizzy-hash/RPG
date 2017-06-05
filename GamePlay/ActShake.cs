using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

namespace ACT
{
    public class ActShake : ActItem
    {
        [SerializeField]
        public EnumCameraShake Shake                 = EnumCameraShake.Vertical;
        [SerializeField]
        public float           Amplitude             = 0.50f;  //振幅
        [SerializeField]
        public float           AmplitudeAttenuation  = 0.02f;  //振幅衰减
        [SerializeField]
        public float           Frequency             = 20f;   //震动频率
        [SerializeField]
        public float           FrequencyKeepduration = 10f;   //频率时间
        [SerializeField]
        public float           FrequencyAttenuation  = 0.05f; //频率衰减

        public Camera          TargetCamera
        {
            get { return GTCameraManager.Instance.MainCamera; }
        }

        public Vector3         RetainPos
        {
            get; set;
        }

        public ActShake()
        {
            this.ActType   = EActType.TYPE_SHAKE;
            this.EventType = EActEventType.Subtain;
        }

        protected override void Trigger()
        {
            base.Trigger();
            this.RetainPos = TargetCamera.transform.localPosition;
            TargetCamera.DOShakePosition(Duration);
            GTCameraManager.Instance.CameraCtrl.DoShake();
        }

        protected override void Execute()
        {
            base.Execute();
            //this.DoShake(ElapsedTime);
        }

        protected override void End()
        {
            base.End();
            TargetCamera.transform.position = RetainPos;
            GTCameraManager.Instance.CameraCtrl.StopShake();
        }

        protected override void Exit()
        {
            base.Exit();
            TargetCamera.transform.position = RetainPos;
            GTCameraManager.Instance.CameraCtrl.StopShake();
        }

        void DoShake(float time)
        {
            float useTime = time - this.StTime;
            float num1 = 0f;
            if (this.AmplitudeAttenuation > 0f)
            {
                float num2 = 1f / this.AmplitudeAttenuation;
                num1 = num2 / (useTime + num2);
            }
            float num3 = this.Amplitude * num1;
            float num4 = this.Frequency;
            if (this.FrequencyKeepduration > 0f && useTime > this.FrequencyKeepduration && this.FrequencyAttenuation > 0f)
            {
                float num5 = 1f / this.FrequencyAttenuation;
                num4 *= num5 / (useTime + num5);
            }
            float delta = num3 * (float)Math.Sin(Mathf.PI * 2 * (float)num4 * (float)useTime);
            Vector3 pos = Vector3.zero;
            switch (this.Shake)
            {
                case EnumCameraShake.HorizontalAndVertical:
                    pos.x += delta;
                    pos.y += delta;
                    break;
                case EnumCameraShake.Horizontal:
                    pos.x += delta;
                    break;
                case EnumCameraShake.Vertical:
                    pos.y += delta;
                    break;
            }
            TargetCamera.transform.position = RetainPos + pos;
        }
    }
}
