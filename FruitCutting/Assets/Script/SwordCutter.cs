using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]//Rigid body 설정을 자동으로 해줌
public class SwordCutter : MonoBehaviour {
    
	public Material capMaterial;
    AudioSource chopSound;
    void Start()
    {
        chopSound = GetComponent<AudioSource>();

    }

    //물체가 충돌했을 때 호출되는 함수. TriggerEnter는 물리적 계산 X CollisionEnter가 후르츠 닌자에는 적합.
    void OnCollisionEnter(Collision collision)
    {

        GameObject victim = collision.collider.gameObject;
        //점수 증가
        if (victim.tag == "Fruit")
        {
            GameManager.instance.GetScore();
            chopSound.Play();
            victim.tag = "Untagged";
        }


        GameObject[] pieces = BLINDED_AM_ME.MeshCut.Cut(victim, transform.position, transform.right, capMaterial);

        if (!pieces[1].GetComponent<Rigidbody>())
        {

            pieces[1].AddComponent<Rigidbody>();
            MeshCollider temp = pieces[1].AddComponent<MeshCollider>();
            temp.convex = true;

        }
        Destroy(pieces[0], 1);
        Destroy(pieces[1], 1);//잘려진 조각 없앰(left side & right side)
    }
    

}
