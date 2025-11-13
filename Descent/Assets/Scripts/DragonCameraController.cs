using UnityEngine;

public class DragonCameraController : MonoBehaviour
{
	[Header("Camera Target")]
	public Transform dragonHead; //Stores object position/rotation/scale to follow
	public Transform dragonBody; //Two targets can be averaged for smoother motion/head is more jerky

	[Header("Camera Position")]
	public float followDistance = 8f; //The distance behind it follows
	public float followHeight = 5f; //How high above it follows
	public float sideDistance = 0f; //How far on the sides
	public Vector3 lookOffset = new Vector3(0f,0f,1.4f); //Looks slightly ahead of target

	[Header("Smoothing")]
	public float positionSmoothing = 5f; //Doesn't jerk as often with higher numbers, but will lag behind targets in position
	public float rotationSmoothing = 3f; //How fast it rotates to target
	public float positionPrediction = 0.3f; //Tries to point where dragon 'will be'

	[Header("Camera Behaviour")]
	public bool dynamicDistance = true; //Camera pulls back when dragon goes faster
	public float speedDistanceMultiplier = 0.2f; //How far the camera pulls back multiplier
	public float minDistance = 4f;
	public float maxDistance = 12f; //Max/min distances.
	
	[Header("Camera Colision")]
	public bool avoidClipping = true;
	public LayerMask collisionLayers;
	public float collisionBuffer = .5f;

	private Vector3 currentVelocity;
	private Vector3 targetVelocity;
	private float currentDistance; //Distance from target
	private Vector3 targetLookPosition; //Where camera wants to look
	private Vector3 smoothedLookPosition; //Where camera is looking

void Start()
{
	currentDistance = followDistance; //states currentDistance
	targetLookPosition = dragonHead.position; //states targetLook is dragon position
	smoothedLookPosition = targetLookPosition; //Camera starts over dragon
}

void LateUpdate()
{
	CalculateDynamicDistance(); //LateUpdates after Update to change where the camera is.
UpdateCameraPosition(); //Moves camera
UpdateCameraRotation(); //rotates camera to look at target
}

void CalculateDynamicDistance()
{
	DragonFlightController flightController = dragonHead.GetComponent<DragonFlightController>(); //Find flightcontrol
	float dragonSpeed = flightController.GetCurrentSpeed(); //How fast player going, grabs it
	float speedOffset = dragonSpeed * speedDistanceMultiplier; //The offset (speed x multiplier) finds extra distance
	currentDistance = Mathf.Clamp(followDistance + speedOffset,minDistance,maxDistance); //Applies the speedoffset, but Keeps it between min/max 
}

void UpdateCameraPosition()
{
	Vector3 followPoint;
	if (dragonBody != null)
	{
		followPoint = Vector3.Lerp(dragonHead.position,dragonBody.position,0.2f); //If dragonbody is there, follow the head and the body, but mostly the head. The .2 is '20%'. So 20% of the cameras influence is the 'body'.
	}
	else
	{
		followPoint = dragonHead.position; //if it's not there just follow the head
	}
	
	Vector3 predictedPosition = followPoint;
	if (positionPrediction >0f)
	{
		DragonFlightController flightController = dragonHead.GetComponent<DragonFlightController>(); // Grabs component
		if (flightController != null)
		{
			Vector3 velocity = flightController.GetCurrentVelocity(); //Grabs velocity
			targetVelocity = Vector3.Lerp(targetVelocity,velocity,Time.deltaTime * 5f); //Smooths velocity, so prediction is less jerky (*5f is 5 times per second)
			predictedPosition = followPoint + targetVelocity.normalized * positionPrediction; //Turns targetvelocity into direction, multiplies by positionprediction ('move __ in that direction'), then adds to current position (followPoint)
		}
	}
	
	Vector3 localOffset = new Vector3(sideDistance,followHeight,-currentDistance); //Local offset 'Local space', directions relative to target
	Vector3 worldOffset = dragonHead.TransformDirection(localOffset); //Finds the world offset using dragonHead rotation and local offset and rotating them, but uses Targets rotation and the local offset to find the World offset. This is because it's always 'above' the dragons back, but it could be 'below' the dragon in the world direction, if the dragon is upside down.

	Vector3 desiredPosition = predictedPosition + worldOffset; //Takes predicted target location and adds the rotated offset to find where camera should be

	if (avoidClipping)
	{
		Vector3 directionToCamera = desiredPosition - followPoint; //Vector pointing from target to camera
		float targetDistance = directionToCamera.magnitude; //Length of the distance to camera so it only measures it

		RaycastHit hit;
		if (Physics.Raycast(followPoint,directionToCamera.normalized,out hit, targetDistance, collisionLayers)) //Checks if something is between target and where camera wants to be, uses targetDistance to only check relevant space and normalizing. Checks collisionLayers. out means fill with results? Shooting towards camera (normalized)
		{
			float safeDistance = hit.distance - collisionBuffer; //How far until wall is hit (hit.distance), collisionBuffer is the safety margin. 
			desiredPosition = followPoint + directionToCamera.normalized *Mathf.Max(safeDistance, 2f); //Mathf.Max prevents camera from ever getting 2units close to the dragon in order to prevent clipping into the dragon itself. Should mean it'll clip through a wall if the wall is 1unit away, but it won't clip through dragon.
		} 
	}
	transform.position = Vector3.SmoothDamp (transform.position,desiredPosition, ref currentVelocity, 1f/ positionSmoothing); // Smoothes movement using SmoothDamp, accelerating toward then decellerating. from where camera is, to where it should be, tracking current velocity, and using 1f/positionSmoothing as time to reach target. Higher smoothing value less time it takes.
}

void UpdateCameraRotation()
{
	Vector3 lookPoint = dragonHead.position + dragonHead.TransformDirection(lookOffset); // look offset in local space (_ units forward) added to the world space (dragonHead.TransformDirection), added to dragons position is the offset in front of the target, whever its facing. (if lookOffset is 0,0,1, it'll be 1 unit ahead) this keeps target in frame by looking ahead slightly
	targetLookPosition = lookPoint;
	smoothedLookPosition = Vector3.Lerp(smoothedLookPosition, targetLookPosition, rotationSmoothing * Time.deltaTime); //Camera pans to follow target, doesn't snap (Thanks lerp)

	Vector3 directionToTarget = smoothedLookPosition - transform.position;
	if (directionToTarget.magnitude > 0.01f) //Shouldn't get that close
	{
		float dragonRoll = 0f;
		DragonFlightController flightController = dragonHead.GetComponent<DragonFlightController>();
		if (flightController != null)
		{
			dragonRoll = flightController.GetCurrentRoll(); //grab roll from Flight Controller
		}

		Quaternion lookRotation = Quaternion.LookRotation(directionToTarget, Vector3.up); //base 'looking' rotation, towards target and sky
		Quaternion rollRotation = Quaternion.Euler(0f,0f,dragonRoll); //Adds only the roll of the target
		Quaternion targetRotation = lookRotation * rollRotation; //lookRotation puts level to horizon THEN, rollRotation applies current Roll of target.

		transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,rotationSmoothing * Time.deltaTime); //Apply Rotation to camera, Slerp is like lerp but for rotating. Uses cameras current rotation, where it should be facing, then how fast.
	}
}
public void SetDistance (float distance)
{
	followDistance = Mathf.Clamp(distance, minDistance, maxDistance);
}
public void SetHeight(float height)
{
	followHeight = height;
}
public void SetLookAhead(float ahead)
{
	lookOffset.z = ahead;
}
}
