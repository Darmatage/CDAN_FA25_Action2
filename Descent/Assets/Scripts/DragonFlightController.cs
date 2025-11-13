using UnityEngine;
using UnityEngine.InputSystem;

public class DragonFlightController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 10f; // Forward is fastest! :) or should be.
    public float strafeSpeed = 8f; //Side to Side Movement, Left/Right (Same speed)
    public float backwardSpeed = 6f; //Backward would be slowest!
    public float verticalSpeed = 8f; //Up/Down speed, faster than backward.
    public float acceleration = 3f; //How fast does it speed up? (You accelerate into a sprint)
    public float drag = 2f; //'Water resistance', slowing down when you stop moving.

    [Header("Mouse Settings")]
    public float mouseSensitivity = 2.3f; //How much it turns when the mouse is moved
    public float rotationSpeed = 5f; //How fast it actually rotates once told

    [Header("Roll Speed")] //???Not quite how I want it at the moment.
    public float rollSpeed = 90f; //How many degrees per second it rolls

    [Header("Constraints")]
    public float maxTurnRate = 150f; // Degrees for max turn rate to prevent weird movement

    private Vector3 currentVelocity = Vector3.zero; //Track of speed/direction, starts at (0,0,0)
    private float currentYaw = 0f; //Which way it faces (Horizontal)
    private float currentPitch = 0f; //How much it faces (Up/Down)
    private float currentRoll = 0f; //How much it rolls
    private float targetRoll = 0f; //Targetroll starts at 0
    private Quaternion targetRotation; //Current rotations in all axis

    void Start() //Sets mouse.
    {
        Cursor.lockState = CursorLockMode.Locked; //Smoother movement feeling + I don't want to fling my cursor off the screen when I move.
        Cursor.visible = false; //It's very distracting.
    }

    void Update()
    {
        HandleMouseLook(); //Mouse looking movement
        HandleRoll(); //Rolls control :)
        HandleMovement(); // Movement Controls
        CursorToggle(); // Escape to view Cursor
    }

    void HandleMouseLook()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; //Gets the input of how left/right the mouse was moved in the frame, (Negative/Positive), and then multiplies by the mouseSensitivity value so Higher mouse sensitivity = More input onto the Yaw.
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;  //Gets the input of how down/up the mouse was moved in the frame, (Negative/Positive), and then multiplies by the mouseSensitivity value so Higher mouse sensitivity = More input onto the Pitch.
        
        
        currentYaw += mouseX; //Adds mouse movement to current rotation
        currentPitch -= mouseY; //Inverted?, so moving mouse up moves character up.
        // The mouselook only handles updating rotation values not actual rotation
        //If we wanted to limit either add a Mathf.Clamp(value,min,max); to the end to prevent 360 movement.
        /*
        float rollRadians = currentRoll * Mathf.Deg2Rad;
        //From what I understand, it needs to do this math in radians, so converts roll degrees to radians

        float adjustedYaw = mouseX * Mathf.Cos(rollRadians) - mouseY * Mathf.Sin(rollRadians); //I don�t fucking know at this point

        float adjustedPitch = mouseX * Mathf.Sin(rollRadians) + mouseY * Mathf.Cos(rollRadians); //IDK I FORGOT SIN AND COS

        currentYaw += adjustedYaw;
        currentPitch += adjustedPitch;
        */
    }

    void HandleRoll()
    {
        float rollInput = 0f; //No roll when no keys are pressed
        if (Input.GetKey(KeyCode.Q))
            rollInput = 1f; //Roll input for left when Q
        else if (Input.GetKey(KeyCode.E))
            rollInput = -1f; //Roll input for right when E

        if (Input.GetKey(KeyCode.L)) //Manually Levelling. If You want.
        {
            targetRoll = Mathf.Lerp(targetRoll, 0f, 3f * Time.deltaTime);
        }
        else
        {
            targetRoll += rollInput * rollSpeed * Time.deltaTime; //TargetRolls Left/Right at designated roll speed, told to do Time.deltaTime so people cannot abuse speed with Frame speed (ie faster on some frames)
        }

        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * 10f); //Smooths roll.
        targetRotation = Quaternion.Euler(currentPitch, currentYaw, currentRoll); //Stores target rotation in all axis (XYZ)
        float maxRotationPerFrame = maxTurnRate * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationPerFrame); //Says in the name what it does, rotates 'From transform.rotation, to targetRotation, with 'maxDegreesDelta' as the how fast/tight the turn constraint is.
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

        Vector3 targetVelocity = new Vector3(strafe * strafeSpeed, vertical * verticalSpeed, forward * currentForwardSpeed); //Velocity (Direction/Speed) vector3, to tell where it's going and how fast

        targetVelocity = transform.TransformDirection(targetVelocity); //Changes from local to world. Wasn't sure about this, but it acts really bad without it, without it it navigates strangley. I really don't get the logic behind it, I thought it was backwards? Anyways, makes it so that forwards is always the front of the target. targetVelocity starts in local space relative to dragon so moves according to dragons coordinates until it's changed.

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime); //Acceleration, smoother movement. I'm kinda starting to love lerp.

        currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, drag * Time.deltaTime); //Drag. Vector3.zero is (0,0,0) so no movement, which means it's slowly going to pause based on drag amount when you aren't pressing something else.

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
        return currentVelocity.magnitude;
    }

    public Vector3 GetCurrentVelocity()
    {
        return currentVelocity;
    }

    public float GetCurrentRoll()
    {
        return currentRoll;
    }
}
