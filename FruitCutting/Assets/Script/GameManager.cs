using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class prescriptionData {

    public string mainEye;
    public int verticalMin;
    public int verticalMax;
    public int horizontalMin;
    public int horizontalMax;
    public int objectMin;
    public int objectMax;
    public int blurMax;
    public int blurMin;
    public int vividMax;
    public int vividMin;

    public void print () {
        Debug.Log (blurMax);
    }
}

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public GameObject scoreText;
    public GameObject bestScoreText;
    public GameObject timeText;
    public GameObject finishText;
    public GameObject spawner;
    public GameObject board;
    public GameObject blurPanel;
    public Image fadeImageRight;
    public Image fadeImageLeft;
    public bool gameStart;

    private int score;
    private float time;
    private int fruitNum;
    private float scorePercent;
    private string url = "http://15.164.220.109/Api/MediBoard/TrainingChart";
    private string[] arguments;
    private int round;
    private float gameTime;

    public string token = "";
    public prescriptionData preData;

    AudioSource timeupSound;

    //초기 실행 함수
    void Awake () {
        instance = this;
        arguments = Environment.GetCommandLineArgs ();
        //토큰 받아오기

        token = arguments[1].ToString();
        //token = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiI1ZGM0MmFlNWM5ZTc3YzAwMDFkYzYwMTYiLCJyb2xlcyI6IlJPTEVfUGF0aWVudCIsImlhdCI6MTU3MzE0Mjc2MSwiZXhwIjoxNTczMTQ2MzYxfQ.YaFk2JuUvdmVNrUKMg5EWDk8mTcQmR1Ja2HWdzjMiUA";
        gameTime = 60.0f;

        //게임 시작 전 처방 데이터 받아오기
        StartCoroutine ("GetData");

    }

    void Start () {

        gameStart = false;
        round = 1; //라운드 1
        score = 0;
        time = gameTime;
        timeupSound = GetComponent<AudioSource> ();

    }

    void Update () {
        if (gameStart == true && round == 1) {
            scoreText.GetComponent<TextMesh> ().text = score.ToString ();
            bestScoreText.GetComponent<TextMesh> ().text = "BEST: " + GetBestScore ();
            timeText.GetComponent<TextMesh> ().text = time.ToString ("F1") + "초";
            time -= Time.deltaTime;

            //1 round 끝

            if (time < 0 && round == 1) {
                round = 2;
                gameStart = false;
                time = 0;
                fruitNum = spawner.GetComponent<FruitNinja> ().fruitNum;
                //1 round 끝나고, 집중도 데이터 보내기
                GameObject.Find ("EyeFocusCheck").GetComponent<ViveSR.anipal.Eye.EyeFocusCheck> ().PostData (preData.blurMin, preData.horizontalMin, preData.verticalMin);
                StartCoroutine ("Round2");
            }
        } else if (gameStart == true && round == 2) {
            scoreText.GetComponent<TextMesh> ().text = score.ToString ();
            bestScoreText.GetComponent<TextMesh> ().text = "BEST: " + GetBestScore ();
            timeText.GetComponent<TextMesh> ().text = time.ToString ("F1") + "초";
            time -= Time.deltaTime;

            //2 round 끝
            if (time < 0 && round == 2) {
                gameStart = false;
                time = 0;
                fruitNum = spawner.GetComponent<FruitNinja> ().fruitNum;
                //게임 끝나고, 집중도 데이터 보내기
                GameObject.Find ("EyeFocusCheck").GetComponent<ViveSR.anipal.Eye.EyeFocusCheck> ().PostData (preData.blurMax, preData.horizontalMax, preData.verticalMax);
                StartCoroutine ("Finish");
            }

        }

    }

    void UpdateBestScore () {
        if (GetBestScore () < score)
            PlayerPrefs.SetInt ("BestScore", score);
    }

    int GetBestScore () {

        int bestScore = PlayerPrefs.GetInt ("BestScore");

        return bestScore;

    }

    public void GetScore () {

        score++;

    }
//프리즘과 블러 세팅
    public void SetPrismBlur () {
        float horiMin = (float) (preData.horizontalMin * 0.57);
        float vertiMin = (float) (preData.verticalMin * 0.57);
        Debug.Log ("prism min" + horiMin + "," + vertiMin);
        Debug.Log (preData.blurMax);

        if (preData.mainEye == "rightEye") //오른쪽이 주시안일 경우
        {
            //약시안인 왼쪽 각도 변경
            GameObject.Find ("CameraLeft").GetComponent<Transform> ().rotation = Quaternion.Euler (vertiMin, horiMin, 0);
            blurPanel.transform.parent = GameObject.FindWithTag ("rightEye").transform;
            blurPanel.GetComponent<Renderer> ().material.SetFloat ("_Radius", preData.blurMax);
        } 
        else //왼쪽이 주시안일 경우
        {
            //약시안인 오른쪽 각도 변경
            GameObject.Find ("CameraRight").GetComponent<Transform> ().rotation = Quaternion.Euler (vertiMin, horiMin, 0);
            blurPanel.GetComponent<Renderer> ().material.SetFloat ("_Radius", preData.blurMax);
            blurPanel.transform.parent = GameObject.FindWithTag ("leftEye").transform;
        }

    }

    IEnumerator Round2 () {

        board.SetActive (false);

        timeupSound.Play ();

        finishText.GetComponent<TextMesh> ().text = "Times Up!";

        yield return new WaitForSeconds (2f);

        finishText.GetComponent<TextMesh> ().text = "잠시 휴식 후 2라운드 시작됩니다.";

        yield return new WaitForSeconds (2f);

        Color startColorRight = fadeImageRight.color;

        Color startColorLeft = fadeImageLeft.color;

        //fade out
        startColorRight.a = 1.0f;
        fadeImageRight.color = startColorRight;
        startColorLeft.a = 1.0f;
        fadeImageLeft.color = startColorRight;

        yield return new WaitForSeconds (5f); //5초 휴식

        //fade in
        startColorRight.a = 0;
        fadeImageRight.color = startColorRight;
        startColorLeft.a = 0;
        fadeImageLeft.color = startColorRight;

        yield return new WaitForSeconds (1f);
        finishText.GetComponent<TextMesh> ().text = "40초";
        yield return new WaitForSeconds (2f);
        timeupSound.Play ();

        finishText.GetComponent<TextMesh> ().text = "Round 2";
        yield return new WaitForSeconds (1f);
        finishText.GetComponent<TextMesh> ().text = "";

//프리즘과 블러 세팅
        float horiMax = (float) (preData.horizontalMax * 0.57);
        float vertiMax = (float) (preData.verticalMax * 0.57);
        if (preData.mainEye == "leftEye") //왼쪽이 주시안일 경우
        {
            //블러 최대값 적용
            blurPanel.GetComponent<Image>().material.SetFloat("_Radius", preData.blurMax);
            //약시안인 오른쪽 각도 변경
            GameObject.Find ("CameraRight").GetComponent<Transform> ().rotation = Quaternion.Euler (vertiMax, horiMax, 0);
        } else //오른쪽이 주시안일 경우
        {
            //블러 최대값 적용
            blurPanel.GetComponent<Image>().material.SetFloat("_Radius", preData.blurMax);
            //약시안인 왼쪽 각도 변경
            GameObject.Find ("CameraLeft").GetComponent<Transform> ().rotation = Quaternion.Euler (vertiMax, horiMax, 0);
        }

        //게임 시작

        board.SetActive (true);
        round = 2;
        time = gameTime;
        gameStart = true;
    }

    IEnumerator Finish () {

        board.SetActive (false);

        timeupSound.Play ();

        finishText.GetComponent<TextMesh> ().text = "Times Up!";

        Debug.Log (score + " " + fruitNum);

        yield return new WaitForSeconds (2f);

        finishText.GetComponent<TextMesh> ().text = (((float) score / fruitNum) * 100).ToString ("F1") + "% 달성하셨습니다!";

        yield return new WaitForSeconds (4f);

        UpdateBestScore ();

        SceneManager.LoadScene (0); //게임 재시작

    }

    IEnumerator GetData () {
        var uwr = new UnityWebRequest (url, "GET");
        uwr.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer ();
        uwr.SetRequestHeader ("accept", "application/json;charset=UTF-8");
        uwr.SetRequestHeader ("X-AUTH-TOKEN", token);

        yield return uwr.SendWebRequest ();

        if (uwr.isNetworkError || uwr.isHttpError) {
            Debug.Log (uwr.error);
        } else {
            // Show results as text
            preData = JsonUtility.FromJson<prescriptionData> (uwr.downloadHandler.text);
            Debug.Log (preData.mainEye);
            SetPrismBlur ();
        }
    }
}