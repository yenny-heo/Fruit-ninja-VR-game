using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitNinja : MonoBehaviour
{
    public GameObject[] fruitPrefab;
    public int fruitNum;
    GameObject manager;
    AudioSource fruitUp;
    bool start;

    // Start is called before the first frame update
    void Start()
    {
        fruitUp = GetComponent<AudioSource>();
        fruitNum = 0;
        StartCoroutine("SpawnFruit");
    }
    

    IEnumerator SpawnFruit()
    {
        while (true)
        {
            start = GameManager.instance.gameStart;
            //시작 전까지 대기
            if (start == true)
             {
                GameObject go = Instantiate(fruitPrefab[Random.Range(0, fruitPrefab.Length)]);//과일 중에 하나 랜덤으로 선택해서 생성
                fruitUp.Play();//효과음
                fruitNum++;
                Rigidbody temp = go.GetComponent<Rigidbody>();//선택된 과일 정보 가져옴

                temp.velocity = new Vector3(0f, Random.Range(4f, 6f), -0.5f);//속도 설정
                temp.angularVelocity = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));//각속도 설정 (물체 회전)
                temp.useGravity = true;//중력 설정

                Vector3 pos = transform.position;
                pos.x += Random.Range(-0.7f, 0.7f);//x축 위치 랜덤으로 설정
                go.transform.position = pos;//위치 지정


                yield return new WaitForSeconds(1.2f);//몇 초 대기

            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
