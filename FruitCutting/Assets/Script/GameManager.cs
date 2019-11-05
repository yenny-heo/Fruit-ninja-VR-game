using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System;
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
    public void print(){
        Debug.Log(blurMax);
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
    
    public string token;
    public prescriptionData preData;

    AudioSource timeupSound;

    //초기 실행 함수
    void Awake () {
        instance = this;
        arguments = Environment.GetCommandLineArgs();
    }

    void Start () {
        gameStart = false;
        //토큰 받아오기
        token = arguments[1].ToString();
        //게임 시작 전 처방 데이터 받아오기
        StartCoroutine(GetData());
        round = 1;//라운드 1
        score = 0;
        time = 40.0f;
        timeupSound = GetComponent<AudioSource> ();

    }

    void Update () {
        if (gameStart == true && round == 1) {
            scoreText.GetComponent<TextMesh> ().text = score.ToString ();
            bestScoreText.GetComponent<TextMesh> ().text = "BEST: " + GetBestScore ();
            timeText.GetComponent<TextMesh> ().text = time.ToString ("F1") + "초";
            time -= Time.deltaTime;
            
            //1 round 끝
            if (time < 0 && round == 1)
            {
                round = 2; 
                gameStart = false;
                time = 0;
                fruitNum = spawner.GetComponent<FruitNinja> ().fruitNum;
                //게임 끝나고, 집중도 데이터 보내기
                //GameObject.Find ("EyeFocusCheck").GetComponent<ViveSR.anipal.Eye.EyeFocusCheck> ().PostData ();

                StartCoroutine ("Round2");
            }
        }
        else if(gameStart == true && round == 2){
            scoreText.GetComponent<TextMesh> ().text = score.ToString ();
            bestScoreText.GetComponent<TextMesh> ().text = "BEST: " + GetBestScore ();
            timeText.GetComponent<TextMesh> ().text = time.ToString ("F1") + "초";
            time -= Time.deltaTime;
             if (time < 0 && round == 2)
            {
                gameStart = false;
                time = 0;
                fruitNum = spawner.GetComponent<FruitNinja> ().fruitNum;
                //게임 끝나고, 집중도 데이터 보내기
              //  GameObject.Find ("EyeFocusCheck").GetComponent<ViveSR.anipal.Eye.EyeFocusCheck> ().PostData ();

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

    T JsonToObject<T> (string jsonData){
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    public void SetBlur() {
        if(preData.mainEye == "rightEye"){
            blurPanel.transform.parent = GameObject.FindWithTag("rightEye").transform;
        }
        else blurPanel.transform.parent = GameObject.FindWithTag("leftEye").transform;
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
        yield return new WaitForSeconds (5f);//5초 휴식
        //fade in
        startColorRight.a = 0;
        fadeImageRight.color = startColorRight;
        startColorLeft.a = 0;
        fadeImageLeft.color = startColorRight;
        yield return new WaitForSeconds (1f);
        finishText.GetComponent<TextMesh>().text = "40초";
        yield return new WaitForSeconds(2f);
        timeupSound.Play();
        finishText.GetComponent<TextMesh>().text = "Round 2";
        yield return new WaitForSeconds(1f);
        finishText.GetComponent<TextMesh>().text = "";
        //게임 시작
        board.SetActive(true);
        round = 2;
        time = 40.0f;
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

    IEnumerator GetData(){
        var uwr = new UnityWebRequest(url, "GET");
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("accept", "application/json;charset=UTF-8");
        uwr.SetRequestHeader("X-AUTH-TOKEN", token);

        yield return uwr.SendWebRequest();
         if(uwr.isNetworkError || uwr.isHttpError) {
            Debug.Log(uwr.error);
        }
        else {
            // Show results as text
            preData = JsonToObject<prescriptionData>(uwr.downloadHandler.text);
            SetBlur();
        }
    }

}