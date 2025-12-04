using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MSG_FacePlayer : MonoBehaviour
{

	private Transform playerCam;

    void Start()
    {
		if (GameObject.FindWithTag("MainCamera") != null){
        	playerCam = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
		}
    }

    void Update()
    {
		//Vector3 target = new Vector3(player.position.x,player.position.y,player.position.z);
        //Transform.LookAt(player.position, Vector3.left);
		transform.LookAt(playerCam.position);
    }
}
