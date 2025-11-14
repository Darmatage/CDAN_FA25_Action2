using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_BobbitWorm : MonoBehaviour
{

//target variables
	private Transform playerTarget;
	private float targetHeadTheshold = 30f;
	private float targetBaseTheshold = 35f;
	private float targetSpeed = 3f;
	public bool playerInRange= false;
	public Transform wormHead;
	public Transform wormBase;
	public Transform wormRest;
	public Transform rotationJoint;
	Quaternion targetRotation;

    void Start()
    {
			playerTarget = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {

		//if the player gets near the worm head:
			if (Vector3.Distance(wormHead.position, playerTarget.position) <= targetHeadTheshold){
				playerInRange=true;
			}
			
			//if the player get far enough away from the base: 
			if (Vector3.Distance(wormBase.position, playerTarget.position) > targetBaseTheshold){
				playerInRange=false;
			}
    }

	void FixedUpdate()
	{
		if (playerInRange)
		{
			transform.position = Vector3.Lerp(transform.position, playerTarget.position, targetSpeed * Time.fixedDeltaTime);
			// Calculate the rotation needed to face the target
           	targetRotation = Quaternion.LookRotation(playerTarget.position - rotationJoint.position);
		}
		else
			{
				transform.position = Vector3.Lerp(transform.position, wormRest.position, targetSpeed * Time.fixedDeltaTime);
				// Calculate the rotation needed to face the target
            	targetRotation = Quaternion.LookRotation(wormRest.position - rotationJoint.position);        
			}
			//adjust rotation: 
			targetRotation = Quaternion.Euler(targetRotation.x, targetRotation.y +90, targetRotation.z);
			// Apply the rotation to the end effector
            rotationJoint.rotation = targetRotation;
	}

}
