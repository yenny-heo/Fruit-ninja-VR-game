using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject scoreText;
    public GameObject bestScoreText;
    public GameObject timeText;
    public GameObject finishText;
    public GameObject spawner;
    public GameObject board;
    public GameObject dominantEye;
    public GameObject blurPanel;
    public bool gameStart;
    int score;
    float time;
    int fruitNum;
    float scorePercent;
    string token;

    AudioSource timeupSound;

    //초기 실행 함수
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameStart = false;
        score = 0;
        time = 40.0f;
        timeupSound = GetComponent<AudioSource>();
        blurPanel.transform.parent = dominantEye.transform;//우세안 블러처리
        
    }

    void Update()
    {
        if (gameStart == true)
        {
            scoreText.GetComponent<TextMesh>().text = score.ToString();
            bestScoreText.GetComponent<TextMesh>().text = "BEST: " + GetBestScore();
            timeText.GetComponent<TextMesh>().text = time.ToString("F1") + "초";
            time -= Time.deltaTime;

            if (time < 0)
            {
                gameStart = false;
                time = 0;
                fruitNum = spawner.GetComponent<FruitNinja>().fruitNum;
                StartCoroutine("Finish");
            }
        }
    }
    void UpdateBestScore()
    {
        if (GetBestScore() < score)
            PlayerPrefs.SetInt("BestScore", score);
    }

    int GetBestScore()
    {
        int bestScore = PlayerPrefs.GetInt("BestScore");
        return bestScore;
    }

    public void GetScore(){
        score++;
    }
    
   
    IEnumerator Finish()
    {
        board.SetActive(false);
        timeupSound.Play();
        finishText.GetComponent<TextMesh>().text = "Times Up!";
        Debug.Log(score + " " + fruitNum);
        yield return new WaitForSeconds(2f);
        finishText.GetComponent<TextMesh>().text = (((float)score / fruitNum) * 100).ToString("F1") + "% 달성하셨습니다!";
        yield return new WaitForSeconds(4f);
        UpdateBestScore();
        SceneManager.LoadScene(0);//게임 재시작
    }

}