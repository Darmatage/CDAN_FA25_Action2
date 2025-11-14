using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_BobbitWorm_EndCollider : MonoBehaviour
{

    void Start()
    {

    }


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Debug.Log("Worm bites the player!");
		}
	}

}

//put this script on an Empty GameObject parented to the end of the IK chain, for collision
//this bjct als needs a box collider set to and a rigidbody