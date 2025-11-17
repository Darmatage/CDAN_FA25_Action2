using UnityEngine;

public class DragonFlightController : MonoBehaviour
{
	[Header("Movement Settings")]
	public float forwardSpeed = 10f; //Forward should be fastest
	public float strafeSpeed = 8f; //Side/Side movement :)
	public float backwardSpeed = 6f; //Backward should be slowest
	public float verticalSpeed = 8f; //Up/Down speed 
	public float acceleration = 3f; //How fast does it speed up?
	public float drag = 2f; //'Water Resistance', slowing down when you stop moving

	[Header("Mouse Settings")]
	public float lookRateSpeed = 90f; //How fast it turns
	public float mouseSensitivity = 1f; //How much the mouse tells it to turn

	[Header("Roll Speed")]
	public float rollSpeed = 90f; //How many degrees per second it rolls
	public float rollAcceleration = 0.5f; //How fast it speeds up rolling.

	[Header("Constraints")]
	public float maxTurnRate = 150f; //Degrees for max turn rate to prevent weird movement

	private Vector3 currentVelocity = Vector3.zero; //Track of speed/direction, starts at (0,0,0)
	private float rollInput = 0f; //Current Rollinput
	private float accumulatedRoll = 0f; //Current Roll (Intentional)
	
	private Vector2 lookInput, screenCenter, mouseDistance; //Mouseinput, where the screen center is, how far the mouse is from it.

	void Start()
	{
		screenCenter.x = Screen.width * 0.5f;
		screenCenter.y = Screen.height * 0.5f;
		//Calculates Screen Middle
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		//Sets cursor invisible/Locked.
		
		accumulatedRoll = 0f;
		//Starts with 0 Roll
	}

	void Update()
	{
		HandleMouseLook(); //Mouse looking movement
		HandleRoll(); //Rolls control :) Redone
		HandleMovement(); // Movement Controls
		CursorToggle(); //Escape to view Cursor/unlock it
	}

	void HandleMouseLook()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
			return; //Don't move if unlocked

		lookInput.x = Input.mousePosition.x; //Where mouse x
		lookInput.y = Input.mousePosition.y; // Where mouse y

		mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
		mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;
		//How far the mouse is from center of screen, in a 'circle' (both divided by screenCenter.y)
		mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f); //If mouse isn't on 'screen', it's still the max value as if you were at the screen edge.
		mouseDistance *= mouseSensitivity;

		float pitchAmount = -mouseDistance.y * lookRateSpeed * Time.deltaTime; //Inverts Y mouse so up goes up * MouseLookSpeed
		float yawAmount = mouseDistance.x * lookRateSpeed * Time.deltaTime; //Same but with X

		
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

	void HandleMovement()
	{
		float forward = 0f;
		float strafe = 0f;
		float vertical = 0f;

		//States initial float values

		if (Input.GetKey(KeyCode.W)) forward = 1f; //Forwards movement
		if (Input.GetKey(KeyCode.S)) forward = -1f; //Backwards movement
		if (Input.GetKey(KeyCode.A)) strafe = -1f; //Left movement
		if (Input.GetKey(KeyCode.D)) strafe = 1f; //Right movement
		if (Input.GetKey(KeyCode.Space)) vertical = 1f; //Up movement
		if (Input.GetKey(KeyCode.LeftControl)) vertical = -1f; //Down movement

		float currentForwardSpeed = forward > 0 ? forwardSpeed : backwardSpeed; // If forward is positive (More than zero), use forwardspeed, otherwise use backward speed.
		
Vector3 targetVelocity = new Vector3(strafe*strafeSpeed, vertical*verticalSpeed,forward*currentForwardSpeed); //Velocity (Direction/Speed) vector3, to tell where it's going and how fast

targetVelocity = transform.TransformDirection(targetVelocity); //Changes from local to world. Wasn't sure about this, but it acts really bad without it, without it it navigates strangley. I really don't get the logic behind it, I thought it was backwards? Anyways, makes it so that forwards is always the front of the target. targetVelocity starts in local space relative to dragon so moves according to dragons coordinates until it's changed.

currentVelocity = Vector3.Lerp(currentVelocity,targetVelocity,acceleration * Time.deltaTime); //Acceleration, smoother movement. I'm kinda starting to love lerp.

currentVelocity = Vector3.Lerp(currentVelocity,Vector3.zero,drag*Time.deltaTime); //Drag. Vector3.zero is (0,0,0) so no movement, which means it's slowly going to pause based on drag amount when you aren't pressing something else.

transform.position += currentVelocity * Time.deltaTime; //Moves based on currentVelocity (+= to current position).
	}


void CursorToggle()
{
if (Input.GetKeyDown(KeyCode.Escape))
{
Cursor.lockState = CursorLockMode.None;
Cursor.visible = true;
} //If Escape is pressed, frees cursor and makes it visible
if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
{
Cursor.lockState = CursorLockMode.Locked;
Cursor.visible = false;
} //When user presses the mouse button while it's unlocked, it relocks and invisi's the cursor.
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
