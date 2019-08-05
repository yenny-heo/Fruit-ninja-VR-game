using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject timeText;
    public GameObject finishText;
    public GameObject spawner;
    public GameObject board;
    public bool gameStart;
    int score = 0;
    float time = 40.0f;
    int fruitNum;
    float scorePercent;

    AudioSource timeupSound;


    public void GetScore(){
        score++;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameStart = false;
        timeupSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStart == true)
        {
            timeText.GetComponent<TextMesh>().text =  time.ToString("F1") + "초";
            scoreText.GetComponent<TextMesh>().text = score.ToString() + "점";
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
    IEnumerator Finish()
    {
        board.SetActive(false);
        timeupSound.Play();
        finishText.GetComponent<TextMesh>().text = "Times Up!";
        yield return new WaitForSeconds(2f);
        finishText.GetComponent<TextMesh>().text = (((float)score / fruitNum) * 100).ToString("F1") + "% 달성하셨습니다!";//
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(0);//게임 재시작
    }
}