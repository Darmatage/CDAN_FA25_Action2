using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_BobbitWorm_Main : MonoBehaviour
{
	Animator anim;
	private float extendtime = 6;

    void Start()
    {
		anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
		if (Input.GetKeyDown("b"))
		{
			StartCoroutine(BobbitCycle());
		}
    }


	IEnumerator BobbitCycle(){
		anim.SetTrigger("WormClose");
		anim.SetBool("WormWave", false);
		yield return new WaitForSeconds(extendtime);
		anim.SetBool("WormWave", true);
		anim.SetTrigger("WormOpen");


		/*
		anim.SetTrigger("WormOpen");
		anim.SetBool("WormWave", true);
		yield return new WaitForSeconds(extendtime);
		anim.SetBool("WormWave", false);
		anim.SetTrigger("WormClose");
		*/
	}

}
