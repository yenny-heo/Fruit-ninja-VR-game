﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartFruit : MonoBehaviour
{

    public Image fadeImageRight;
    public Image fadeImageLeft;
    GameObject manager;
    GameObject readyText;
    public GameObject board;
    public GameObject quitFruit;
    bool flag = false;
    AudioSource clickSound;
    // Start is called before the first frame update
    void Start()
    {
        clickSound = GetComponent<AudioSource>();
        manager = GameObject.Find("GameManager");
        readyText = GameObject.Find("ReadyText");

    }

    void OnCollisionEnter(Collision collision)
    {
        if (flag == false)
        {
            flag = true;
            Debug.Log("Game Start!"+flag);
            StartCoroutine("GameStart");
        }
    }

    IEnumerator GameStart()
    {
        //시작, 기록 과일 떨어지게
        GetComponent<Rigidbody>().useGravity = true;
        quitFruit.GetComponent<Rigidbody>().useGravity = true;

        yield return new WaitForSeconds(0.3f);

        Color startColorRight = fadeImageRight.color;
        Color startColorLeft = fadeImageLeft.color;
        //fade out fade in
        for(int i = 0; i < 100; i++)
        {
            startColorRight.a = startColorRight.a + 0.01f;
            fadeImageRight.color = startColorRight;
            startColorLeft.a = startColorLeft.a + 0.01f;
            fadeImageLeft.color = startColorLeft;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.02f);
        for (int i = 0; i < 100; i++)
        {
            startColorRight.a = startColorRight.a - 0.01f;
            fadeImageRight.color = startColorRight;
            startColorLeft.a = startColorLeft.a - 0.01f;
            fadeImageLeft.color = startColorLeft;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.5f);
        readyText.GetComponent<TextMesh>().text = "40초";
        yield return new WaitForSeconds(2f);
        clickSound.Play();
        readyText.GetComponent<TextMesh>().text = "GO!";
        yield return new WaitForSeconds(1f);
        readyText.GetComponent<TextMesh>().text = "";
        //게임 시작
        board.SetActive(true);
        manager.GetComponent<GameManager>().gameStart = true;
    }
    // Update is called once per frame
    void Update()
    {
        //과일 회전
        float frequency = -60 * Time.deltaTime;
        transform.Rotate(0, 0, frequency);
    }
}
