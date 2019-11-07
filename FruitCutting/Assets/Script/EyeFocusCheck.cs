using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using System;

namespace ViveSR.anipal.Eye {
      public class FocusData
    {
        public string curGame;
        public string date;
        public int focus;
        public int blur;
        public string mainEye;
        public pd pd;
        public FocusData(string curGame, string date, int focus, int blur, string mainEye, pd pd)
        {
            this.curGame = curGame;
            this.date = date;
            this.focus = focus;
            this.blur = blur;
            this.mainEye = mainEye;
            this.pd = pd;
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this, false);
        }
    }
    [System.Serializable]
    public class pd{
        public double horizontal;
        public double vertical;
        public pd(double horizontal, double vertical){
            this.horizontal = horizontal;
            this.vertical = vertical;
        }
    }

    public class EyeFocusCheck : MonoBehaviour {
        public bool NeededToGetData;
        private FocusInfo LeftFocusInfo, RightFocusInfo;
        private readonly float MaxDistance = 20;
        private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
        private static EyeData eyeData = new EyeData ();
        private bool eye_callback_registered = false;
        private int correct, incorrect;
        private string url = "http://15.164.220.109/Api/MediBoard/Result/Playlog";
        private string token;
        void Start () {
            if (!SRanipal_Eye_Framework.Instance.EnableEye) {
                enabled = false;
                return;
            }
            correct = 0;
            incorrect = 0;
            NeededToGetData = false;
        }

        void Update () {
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

            if (NeededToGetData) {
                if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false) {
                    SRanipal_Eye.WrapperRegisterEyeDataCallback (Marshal.GetFunctionPointerForDelegate ((SRanipal_Eye.CallbackBasic) EyeCallback));
                    eye_callback_registered = true;
                } 
                else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true) {
                    SRanipal_Eye.WrapperUnRegisterEyeDataCallback (Marshal.GetFunctionPointerForDelegate ((SRanipal_Eye.CallbackBasic) EyeCallback));
                    eye_callback_registered = false;
                }

                Ray LeftGazeRay, RightGazeRay;
                bool left_eye_focus, right_eye_focus;
                //Left eye
                if (eye_callback_registered)
                    left_eye_focus = SRanipal_Eye.Focus (GazeIndex.LEFT, out LeftGazeRay, out LeftFocusInfo, 0, MaxDistance, eyeData);
                else
                    left_eye_focus = SRanipal_Eye.Focus (GazeIndex.LEFT, out LeftGazeRay, out LeftFocusInfo, 0, MaxDistance);

                //Right eye
                if (eye_callback_registered)
                    right_eye_focus = SRanipal_Eye.Focus (GazeIndex.RIGHT, out RightGazeRay, out RightFocusInfo, 0, MaxDistance, eyeData);
                else
                    right_eye_focus = SRanipal_Eye.Focus (GazeIndex.RIGHT, out RightGazeRay, out RightFocusInfo, 0, MaxDistance);

                Debug.Log ("left:" + LeftFocusInfo.collider);
                Debug.Log ("right:" + LeftFocusInfo.collider);

                if (left_eye_focus && right_eye_focus) {
                    if (LeftFocusInfo.collider == RightFocusInfo.collider) {
                        //왼쪽 오른쪽 포커스 일치한 경우
                        Debug.Log ("correct");
                        correct++;
                    } else {
                        //왼쪽 오른쪽  포커스 일치하지 않은 경우
                        Debug.Log ("incorrect");
                        incorrect++;
                    }

                }

            }

        }

        private void Release () {
            if (eye_callback_registered == true) {
                SRanipal_Eye.WrapperUnRegisterEyeDataCallback (Marshal.GetFunctionPointerForDelegate ((SRanipal_Eye.CallbackBasic) EyeCallback));
                eye_callback_registered = false;
            }
        }

        private static void EyeCallback (ref EyeData eye_data) {
            eyeData = eye_data;
        }

        //Focus DATA post

        public void PostData(int blurData, double hori, double verti)
        {
            string curGame = "fruitCutting";
            string today = DateTime.Now.ToString("yyyy.MM.dd.HH:mm");
            int focus;
            if(correct+incorrect == 0) focus = 0;
            else focus = (correct* 100) / (correct + incorrect);
            int blur = blurData;
            string mainEye = GameObject.Find("GameManager").GetComponent<GameManager>().preData.mainEye;

            pd pd = new pd(hori, verti);//pd 수정 필요
            FocusData data = new FocusData(curGame, today, focus, blur, mainEye, pd);
 
            // Convert Data to Json
            string body = data.ToJson();
            Debug.Log(body);

            //token 받아오기
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            token = gameManager.token;

            // Post json Data to Server
            StartCoroutine(PostRequest(url, body));

        }
        IEnumerator PostRequest(string url, string json){
            var uwr = new UnityWebRequest(url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("accept", "application/json;charset=UTF-8");
            uwr.SetRequestHeader("X-AUTH-TOKEN", token);
            yield return uwr.SendWebRequest();
            correct = 0;
            incorrect = 0;
            if(uwr.isNetworkError || uwr.isHttpError){
                Debug.Log("Error While Sending "+uwr.error);
            } else{
                Debug.Log("Received: "+uwr.downloadHandler);
            }
        }
    }
}