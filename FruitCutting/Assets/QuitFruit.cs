using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitFruit : MonoBehaviour
{
    public GameObject startFruit;
    // Start is called before the first frame update
    void Start()
    {
    }
    void OnCollisionEnter(Collision collision)
    {
        startFruit.GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().useGravity = true;
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {

        float frequency = 60 * Time.deltaTime;
        transform.Rotate(0, 0, frequency);
    }
}
