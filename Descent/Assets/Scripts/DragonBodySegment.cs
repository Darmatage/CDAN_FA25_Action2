using UnityEngine;
using System.Collections;
public class DragonBodySegment : MonoBehaviour
{
	[Header("Follow/Smooth Settings")]
	public Transform followTarget; // What the segment follows
	public float segmentDistance = 1f; //Default, declared in the manager
	[Range(0f,1f)]
	public float positionSmoothness = 0.2f; //Smoothing to target position
	[Range(0f,1f)]
	public float rotationSmoothness = 0.15f; //Smoothing to target rotation
	public float minDistance = 0.2f; //Minimum distance
	public float maxDistance = 0.8f; //Max distance. Might need to change if model does.
    

	[Header("Collisions")]
	public bool preventSelfCollision = true; //turns on/off for testing.
	public float selfCollisionRadius = 0.5f; //How large is the self collider
	public bool preventWorldCollision = true; // turns on/off for testing
	public float worldCollisionRadius = 0.7f; //How large is the 'world' collider
	public LayerMask worldCollisionLayers; //What layers does it consider the world
	public float collisionPushStrength = 0.8f; //How strong it pushes away

	[Header("Animations+")]
	public Animator animator;
	public int segmentIndex = 0; //For later delayed animations
	public float animationTimeOffset = 0.2f; //Delay in animation time
    // public string speedParameter = "Speed"; //Animation tree testing (For Blend Tree)
    // public float maxVelocity = 5f; Lower numbers are more sensitive to speed changes.

	public Collider hitboxCollider; //Hitbox collider (for attacks, not physics)

	private Vector3 idealTarget; //Where it's TRYING to go (added for smoothness, testing)
	private Vector3 smoothedPosition; //Stores position moving towards
	private Quaternion smoothedRotation; //Stores rotation
	private Vector3 displayVelocity; //Velocity value for animation
	private bool initialized = false; //Sees if Start has run
	private DragonBodyManager bodyManager; //Reference to DragonBodyManager

	void Start()
	{
		if (hitboxCollider != null) //If hitbox collider exists,
		{
			hitboxCollider.isTrigger = true; //Detects overlap, not physical collision. So detects attacks, but wont block you from going through a wall.
		}
		smoothedPosition = transform.position; //Match current pos
		smoothedRotation = transform.rotation; //Same with rotation
		displayVelocity = Vector3.zero; //Sets to 0,0,0, but can change.
		idealTarget = transform.position; //initialzie
		initialized = true; //Start has run.
		
		bodyManager = GetComponentInParent<DragonBodyManager>(); //Searches gameobject and parents for component, finds script that manages segments
		InitializeAnimationOffset(); //Calls for animation time offset method, so it can run delayed for each piece.
	}

	void InitializeAnimationOffset()
	{
		if (animator == null) return; //Doesn't do anything if there's no animator
		float timeOffset = -segmentIndex * animationTimeOffset; //for the segmentindex, finds the offset. It's negative because otherwise the wave looks opposite the direction it should go. ie. -2x.2 = -.4 seconds
		for (int layer = 0; layer <  animator.layerCount; layer++)//loops through animation layers in animator
		{
			AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(layer); //Finds information about animation on current layer
			if (currentState.length > 0)
			{
				float normalizedTime = (timeOffset%currentState.length)/currentState.length; //normalizes
				if (normalizedTime < 0) normalizedTime = 1f + normalizedTime; //Converts into positive
				animator.Play(currentState.fullPathHash, layer, normalizedTime); //Plays animation, what the state is, the layer, and where in the animation to start
				animator.Update(0f); //Applies time change w/out advancing anim
			}
		}
	}

	void LateUpdate()
	{
		if (followTarget == null) return; //If no target don't run
		if (!initialized) //If start hasn't been run yet
		{
			smoothedPosition = transform.position; 
			smoothedRotation = transform.rotation; 
			displayVelocity = Vector3.zero; 
			idealTarget = transform.position; //initialize if it didn't
			initialized = true; //Basically runs start again just in case
		}
		Vector3 oldPosition = transform.position; //old position before movement
		UpdatePosition(); //Updates posoition
		UpdateRotation(); //Updates rotation

		Vector3 rawVelocity = (transform.position - oldPosition)/Time.deltaTime; //Finds velocity and how far we moved/how fast
		displayVelocity = Vector3.Lerp(displayVelocity,rawVelocity,0.15f); //Moves slowly (Lerp) from old displayvelocity to new one, updates.
		UpdateAnimation();
	}

	void UpdatePosition()
	{

        //Marked out as I test a smoother one, and left in for archival purposes
        //Vector3 directionToTarget = followTarget.position - smoothedPosition; //smooths direction to target  
        //Vector3 desiredPosition = followTarget.position - directionToTarget.normalized * segmentDistance; //Finds position it wants behind target.
        //if (directionToTarget.magnitude > 0.001f)
        //{
        //    desiredPosition = followTarget.position - directionToTarget.normalized * segmentDistance;
        //}
        //smoothedPosition = Vector3.Lerp(smoothedPosition,desiredPosition,positionSmoothness);
        //Vector3 finalDirection = followTarget.position - smoothedPosition; //Finds direction after smoothing
		//Gradually moves towards desired position
        //float finalDistance = finalDirection.magnitude;//distance to target after smoothing
		// (From "Vector3 directionToFollow -> if (finalDistance < minDistance)" is where this script part used to go. Script went from calculating where you should be based on smoothed position, which was a weird loop,
		// since smoothedPosition was calculated also based on smoothed. Basically the target would change based on where the part was and thats a nono, also lead to weird jittering. Now it only follows a spot which has a defined place.)

		 Vector3 directionToFollow = followTarget.position - idealTarget; //Tracks followtarget at segment distance

        if (directionToFollow.magnitude > 0.001f)
        {
            idealTarget = followTarget.position - directionToFollow.normalized * segmentDistance;
        }

		smoothedPosition = Vector3.Lerp(transform.position, idealTarget, positionSmoothness); //Moves towards 'ideal target'

		Vector3 finalDirection = followTarget.position - smoothedPosition; //constraint applied of smoothing where follow
		float finalDistance = finalDirection.magnitude;

		if (finalDistance < minDistance) //if we're too close
		{
			smoothedPosition = followTarget.position - finalDirection.normalized * minDistance;
		}
		else if (finalDistance > maxDistance) //if we're too far
		{
			smoothedPosition = followTarget.position - finalDirection.normalized * maxDistance;
		}

		//Collisions

		if (preventWorldCollision) //Earlier bool, set bool for testing
		{
			smoothedPosition = CheckWorldCollision(smoothedPosition); //gives method the smoothedPosition and gets it back
		}
		if (preventSelfCollision) //same but with self
		{
			smoothedPosition = CheckSelfCollision(smoothedPosition);
		}
		transform.position = smoothedPosition;//Actually moves gameobject due to where smoothedPosition is in the end.
	}

	Vector3 CheckWorldCollision(Vector3 targetPosition)
	{
		Vector3 currentPos = transform.position; //current position
		Vector3 moveDirection = (targetPosition-currentPos).normalized; //Finds were we're trying tp gp
		float moveDistance = Vector3.Distance(currentPos, targetPosition);//How far we're trying to go

		if (moveDistance < 0.001f) return targetPosition; //Skip check if not moving that much.

		RaycastHit hit; //variable to store collision info
		if (Physics.SphereCast(currentPos,worldCollisionRadius,moveDirection, out hit,moveDistance, worldCollisionLayers)) //from starting point, cast's a sphere thats worldCollisionRadius radius wide, which way it's being cast (movedirection), puts collision data into hit variable, how far to cast, and which layers to check.
        {
            float safeDistance = Mathf.Max(0,hit.distance-worldCollisionRadius*0.1f); //how far until wall is hit, with buffer. never negative (mathf.max)
            Vector3 safePosition = currentPos + moveDirection * safeDistance; //Move as close as posible
            safePosition += hit.normal * worldCollisionRadius * collisionPushStrength; //in direction perpendicular to surface, pushes away 
            return safePosition;
        }
        return targetPosition; //if no collision return oroginal
    }

    Vector3 CheckSelfCollision(Vector3 targetPosition)
    {
        if (bodyManager == null) return targetPosition; //no manager then no self collision.
        foreach (DragonBodySegment otherSegment in bodyManager.bodySegments) //For each loop, checks each one in it
        {
	        if (otherSegment == this) continue; //no collision with self, next loop iteration
	        if (otherSegment == null) continue; //if no segment skip

	        int indexDifference = Mathf.Abs(otherSegment.segmentIndex - segmentIndex); //finds distance in chain (ie segment 5 checking segment 3 = 2)

	        if (indexDifference <=1) continue; //skips adjacent segments bc those should overlap some
	        Vector3 toOther = otherSegment.transform.position - targetPosition; //Vector from this segments position to other
	        float distance = toOther.magnitude; //distance between them
	        if (distance < selfCollisionRadius * 2f) //if too close
	        {
	        	Vector3 pushDirection = -toOther.normalized; //Flips direction from other segment
	        	float pushAmount = (selfCollisionRadius * 2f) - distance;// finds overlap amount
	        	targetPosition += pushDirection*pushAmount * 0.5f; //Pushes position away from overlap
	        }
        }
		return targetPosition;
	}


	void UpdateRotation()
	{
     
        

		Vector3 directionToTarget = (followTarget.position - transform.position).normalized; //Pointing to target
		if (directionToTarget.magnitude>0.01f) //only rotate if there's enough change
		{
			Quaternion lookRotation = Quaternion.LookRotation(directionToTarget); //rotation towards target
			Vector3 targetUp = followTarget.up; //Up direction of target
			Vector3 projectedUp = Vector3.ProjectOnPlane(targetUp, directionToTarget);// projects the up vector onto plane perpendicular to forward
			if (projectedUp.magnitude > 0.01f)
			{
				lookRotation = Quaternion.LookRotation(directionToTarget,projectedUp); //creates rotation with up and looking
			}
			smoothedRotation = Quaternion.Slerp(smoothedRotation, lookRotation, rotationSmoothness); // slrps rotation
		}
		transform.rotation = smoothedRotation; // applies rotation
	}

	void UpdateAnimation() //Reduced since blendtrees don't work with delayed animations
	{
		if (animator == null) return; //skip if no animator
		//float speed = displayVelocity.magnitude;//gets velocity magnitude
        //animator.SetFloat(speedParameter,Mathf.Clamp01(speed/maxVelocity)); //normalizes speed, then limits result, and updates animator parameter 

    }

void OnDrawGizmos() //Test
{
	if (!Application.isPlaying || followTarget ==null) return; //only during play mode and if it exists draws line
	float distance = Vector3.Distance(transform.position,followTarget.position);
	Gizmos.DrawLine(transform.position,followTarget.position);
}
}
