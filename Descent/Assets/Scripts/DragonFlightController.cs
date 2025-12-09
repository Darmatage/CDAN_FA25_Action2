using System;
using UnityEngine;

public class DragonFlightController : MonoBehaviour
{

	public GameHandler GameHandler;

	[Header("Movement Settings")]
	public float forwardSpeed = 10f; //Forward should be fastest
	public float strafeSpeed = 8f; //Side/Side movement :)
	public float backwardSpeed = 6f; //Backward should be slowest
	public float verticalSpeed = 8f; //Up/Down speed 
	public float acceleration = 3f; //How fast does it speed up?
	public float drag = 2f; //'Water Resistance', slowing down when you stop moving
	public float dashSpeed = 0f; //boost to movement speed
	public float dashFalloffTimer = 0f; //time since dash was initiated
	public float dashCDTimer = 3f; //time since last dash
	public bool isDashing = false;
	public AudioSource SFX_Dash;

	[Header("Mouse Settings")]
	public float lookRateSpeed = 90f; //How fast it turns
	public float mouseSensitivity = 3f; //How much the mouse tells it to turn
	public float mouseReturnSpeed = 3f; //how fast realmouse returns to center

	[Header("Roll Speed")]
	public float rollSpeed = 90f; //How many degrees per second it rolls
	public float rollAcceleration = 0.5f; //How fast it speeds up rolling.

	[Header("Constraints")]
	public float maxTurnRate = 150f; //Degrees for max turn rate to prevent weird movement
	[Range (0,90)]
	public float maxPitchAngle = 85f; //Max angle up/down (More than 90 fucks it up)
	public LayerMask collisionLayers; //What layers you can't go through.
	public float collisionCheckDistance = 2f; //How far it checks for walls
	public float collisionPushbackForce = 5f; //How hard it pushes from walls

	private Vector3 currentVelocity = Vector3.zero; //Track of speed/direction, starts at (0,0,0)
	private float rollInput = 0f; //Current Rollinput
	private float accumulatedRoll = 0f; //Current Roll (Intentional)
	private Vector2 realMouseDistance = Vector2.zero; //Mouseinput, where the screen center is, how far the mouse is from it. Switched to delta so it can always input, unlike previous version which broke whenever mouse was locked.

	float boostFOV = 60f;
	//float defaultFOV = 60f;
	float FOVtimer = 0f;
	void Start()
	{
		
		LockCursor();
		//Sets cursor invisible/Locked.
		
		accumulatedRoll = 0f;
		//Starts with 0 Roll

		FOVtimer = 100f;

		GameHandler = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
	}
	
	void Update()
	{
		if (PauseMenuHandler.GameisPaused)
		{ 
			return; //no move if pause
		}
        //Camera.main.fieldOfView = (defaultFOV + boostFOV);
        Camera.main.fieldOfView = boostFOV;
        if (Time.frameCount <=5)
        {
            LockCursor(); //Locks cursor if it isn't
        }
		HandleMouseLook(); //Mouse looking movement
		HandleRoll(); //Rolls control :) Redone
		HandleDash(); //dash detectinator
        HandleMovement(); // Movement Controls
		CursorToggle(); //Escape to view Cursor/unlock it

	}

	void HandleMouseLook()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
			return; //Don't move if unlocked

		float mouseDeltaX = Input.GetAxis("Mouse X")* mouseSensitivity; //Where mouse x
		float mouseDeltaY = Input.GetAxis("Mouse Y")* mouseSensitivity; // Where mouse y
		//mosue delta (movement per frame)
		realMouseDistance.x += mouseDeltaX * Time.deltaTime * 60f;
		realMouseDistance.y += mouseDeltaY * Time.deltaTime * 60f;
		//adds delta to position 
		
		realMouseDistance = Vector2.ClampMagnitude(realMouseDistance, 1f); //If mouse isn't on 'screen', it's still the max value as if you were at the screen edge.
		
		if (Mathf.Abs(mouseDeltaX)<0.01f && Mathf.Abs (mouseDeltaY)<0.01f)
        {
            realMouseDistance = Vector2.Lerp(realMouseDistance, Vector2.zero, mouseReturnSpeed*Time.deltaTime);
        } //Returns to center when not moving
		
		float pitchAmount = -realMouseDistance.y * lookRateSpeed * Time.deltaTime; //Inverts Y mouse so up goes up * MouseLookSpeed
		float yawAmount = realMouseDistance.x * lookRateSpeed * Time.deltaTime; //Same but with X

		//Added clamp

		Vector3 currentEuler = transform.eulerAngles;
		float currentPitch = currentEuler.x; //Get's current pitch before applying new

		if (currentPitch > 180f) //pitch converted from over 180  to -180 - 180 range.
			currentPitch -= 360f;

		float newPitch = currentPitch + pitchAmount;
		newPitch = Mathf.Clamp(newPitch, -maxPitchAngle, maxPitchAngle); //Clamps pitch between max and min
		pitchAmount = newPitch - currentPitch; 


		//Added clamp
		transform.Rotate(pitchAmount, 0f, 0f, Space.Self); // Pitch around local X ((Controls so not World Y, dragons OWN updown axis.)
		transform.Rotate(0f, yawAmount, 0f, Space.World);  // Yaw around world Y (imagine horizontal plane, is how you turn left/right.) Removes unintended Roll from pitch/yaw bc we want only the control to do that.
// Both apply them


		Vector3 forward = transform.forward; //Gets the direction it is facing
		Vector3 desiredUp = Vector3.up; // World up
		
		// Rotation that looks forward with World up without roll added
		Quaternion noRollRotation = Quaternion.LookRotation(forward, desiredUp);
		
		
		Quaternion rollRotation = Quaternion.Euler(0f, 0f, accumulatedRoll); //Rotation only holding intentional roll
		
		
		transform.rotation = noRollRotation * rollRotation; //Adds rotation (with no roll) * rotation (intentional roll)
	}

	void HandleRoll()
	{
		float rollInputRaw = 0f; //Starts at 0
		
		if (Input.GetKey(KeyCode.Q))
			rollInputRaw = 1f; //Roll input for left when q
		else if (Input.GetKey(KeyCode.E))
			rollInputRaw = -1f; //Roll input for right when e

		if (Input.GetKey(KeyCode.L)) //Manual Levelling
		{
			accumulatedRoll = Mathf.Lerp(accumulatedRoll, 0f, 2f * Time.deltaTime); //Turns roll towards 0 gradually, using _f*Time.deltaTime (Actually might add as inspector value for easy change?)
			rollInput = 0f; //If you want it level, turns to 0 so levelling doesn't change
		}
		else
		{
			rollInput = Mathf.Lerp(rollInput, rollInputRaw, rollAcceleration * Time.deltaTime); //Tells rollinput to roll towards raw gradually from current rollinput
			accumulatedRoll += rollInput * rollSpeed * Time.deltaTime; //currentroll = rollinput * howfastroll * time
		}
	}
	
	void HandleDash()
	{
		if(dashCDTimer >= GameHandler.dashCD || isDashing)
		{
			//begin dash
			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				dashFalloffTimer = 0f;
				FOVtimer = 0f;
				dashCDTimer = 0f;
				if (SFX_Dash.isPlaying == false){
                        SFX_Dash.Play();
                }
				
			}
			//during dash
			if (Input.GetKey(KeyCode.LeftShift))
			{
				isDashing = true;
				dashFalloffTimer += Time.deltaTime; //increment falloff timer
				
			}
			else
			{
				isDashing = false;
				dashFalloffTimer = 0f;
			}
			
		}
			
		//increment timers
        FOVtimer += Time.deltaTime;
		dashCDTimer += Time.deltaTime;
    }
	void HandleMovement()
	{
		float forward = 0f;
		float strafe = 0f;
		float vertical = 0f;
		//States initial float values

		dashSpeed = isDashing ? (2.5f / (0.5f * dashFalloffTimer + 0.4f)) - 0.1f : 1; //dash speed based on a curve if dashing, else set to 1
		boostFOV = (0.7f / (0.2f * FOVtimer + 0.4f)) + 60;


        if (dashSpeed < 1f) 
		{ 
		  dashSpeed = 1f;
		} //if less than 1, set to 1
		

		if (Input.GetKey(KeyCode.W)) forward = 1f; //Forwards movement
		if (Input.GetKey(KeyCode.S)) forward = -1f; //Backwards movement
		if (Input.GetKey(KeyCode.A)) strafe = -1f; //Left movement
		if (Input.GetKey(KeyCode.D)) strafe = 1f; //Right movement
		if (Input.GetKey(KeyCode.Space)) vertical = 1f; //Up movement
		if (Input.GetKey(KeyCode.LeftControl)) vertical = -1f; //Down movement

		float currentForwardSpeed = forward > 0 ? forwardSpeed : backwardSpeed; // If forward is positive (More than zero), use forwardspeed, otherwise use backward speed.
		
Vector3 targetVelocity = new Vector3(strafe*strafeSpeed, vertical*verticalSpeed,forward*currentForwardSpeed); //Velocity (Direction/Speed) vector3, to tell where it's going and how fast

		targetVelocity *= dashSpeed;

targetVelocity = transform.TransformDirection(targetVelocity); //Changes from local to world. Wasn't sure about this, but it acts really bad without it, without it it navigates strangley. I really don't get the logic behind it, I thought it was backwards? Anyways, makes it so that forwards is always the front of the target. targetVelocity starts in local space relative to dragon so moves according to dragons coordinates until it's changed.

currentVelocity = Vector3.Lerp(currentVelocity,targetVelocity,acceleration * Time.deltaTime); //Acceleration, smoother movement. I'm kinda starting to love lerp.

currentVelocity = Vector3.Lerp(currentVelocity,Vector3.zero,drag*Time.deltaTime); //Drag. Vector3.zero is (0,0,0) so no movement, which means it's slowly going to pause based on drag amount when you aren't pressing something else.

		//Collision Detection yippee
		Vector3 intendedPosition = transform.position + currentVelocity * Time.deltaTime; //Where you are planning/going to be
		Vector3 moveDirection = (intendedPosition - transform.position).normalized; //direction u move
		float moveDistance = (intendedPosition -transform.position).magnitude; //how far player is trying to move

		if (Physics.Raycast(transform.position,moveDirection,out RaycastHit hit, moveDistance + collisionCheckDistance,collisionLayers)) //Shoots ray from position to move direction,  checks intended movement + collisioncheck, and hits objects on collision layers
		{
			//if hit something
			Vector3 slideDirection = Vector3.ProjectOnPlane(currentVelocity, hit.normal); //Slides along where going and hit normal, no longer goes into wall but along (perpendicular to hit surface)
			currentVelocity = slideDirection; //where you're sliding :P

			transform.position += hit.normal * collisionPushbackForce * Time.deltaTime; //pushes back (prevents getting stuck)
		}
		// end collision detection.
transform.position += currentVelocity * Time.deltaTime; //Moves based on currentVelocity (+= to current position).
	}


void CursorToggle()
{
		if (Input.GetKeyDown(KeyCode.Tab) && !PauseMenuHandler.GameisPaused) //Changed to Tab due to Conflict
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				UnlockCursor();
			} //If Tab is pressed with cursor locked, frees cursor and makes it visible
			else
			{
				LockCursor();
			}
		}

		if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None && !PauseMenuHandler.GameisPaused)
		{
			LockCursor();
		} //When user presses the mouse button while it's unlocked, it relocks and invisi's the cursor, only if game is not paused.
}


void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		realMouseDistance = Vector2.zero; //Sets mouse to center
    }

void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
		Cursor.visible=true;
    }

	public float GetCurrentSpeed()
	{
		return currentVelocity.magnitude; //returns length of velocity vector, (converts velocity into speed(number))
	}

	public Vector3 GetCurrentVelocity()
	{
		return currentVelocity; //Gives current velocity (good for prediction)
	}

	public float GetCurrentRoll()
	{
		return accumulatedRoll; //Gives current roll angle
	}
}
