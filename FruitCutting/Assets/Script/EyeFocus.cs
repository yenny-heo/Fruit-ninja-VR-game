using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ViveSR.anipal.Eye
{
    public class EyeFocus : MonoBehaviour
    {
        public bool NeededToGetData = false;
        private FocusInfo LeftFocusInfo, RightFocusInfo;
        private readonly float MaxDistance = 20;
        private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
        private static EyeData eyeData = new EyeData();
        private bool eye_callback_registered = false;
        void Start()
        {
            if (!SRanipal_Eye_Framework.Instance.EnableEye)
            {
                enabled = false;
                return;
            }
        }

        void Update()
        {
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;
            if (NeededToGetData)
            {
                if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                {
                    SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                    eye_callback_registered = true;
                }
                else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                {
                    SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                    eye_callback_registered = false;
                }

                Ray LeftGazeRay, RightGazeRay;
                bool left_eye_focus, right_eye_focus;
                //Left eye
                if (eye_callback_registered)
                    left_eye_focus = SRanipal_Eye.Focus(GazeIndex.LEFT, out LeftGazeRay, out LeftFocusInfo, 0, MaxDistance, eyeData);
                else
                    left_eye_focus = SRanipal_Eye.Focus(GazeIndex.LEFT, out LeftGazeRay, out LeftFocusInfo, 0, MaxDistance);

                //Right eye
                if (eye_callback_registered)
                    right_eye_focus = SRanipal_Eye.Focus(GazeIndex.RIGHT, out RightGazeRay, out RightFocusInfo, 0, MaxDistance, eyeData);
                else
                    right_eye_focus = SRanipal_Eye.Focus(GazeIndex.RIGHT, out RightGazeRay, out RightFocusInfo, 0, MaxDistance);

                if (left_eye_focus && right_eye_focus)
                {
                    if (LeftFocusInfo.transform == RightFocusInfo.transform)
                    {
                        //왼쪽 오른쪽 포커스 일치한 경우
                    }
                    else
                    {
                        //왼쪽 오른쪽  포커스 일치하지 않은 경우
                    }
                }

            }

        }

        private void Release()
        {
            if (eye_callback_registered == true)
            {
                SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }
        }

        private static void EyeCallback(ref EyeData eye_data)
        {
            eyeData = eye_data;
        }
    }
}