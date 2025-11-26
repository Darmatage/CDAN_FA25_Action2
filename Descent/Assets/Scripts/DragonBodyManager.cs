using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragonBodyManager : MonoBehaviour
{

	[Header("Prefabs")]
	public GameObject bodySegmentPrefab;
	public GameObject neckSegmentPrefab;
	public GameObject frontLegSegmentPrefab;
	public GameObject backLegSegmentPrefab;
	//prefabs for all the parts main body

	[Header("tail")]
	public GameObject tailSegmentPrefab;
	public float tailStartScale = 1f; //100% size
	public float tailEndScale = 0.2f; //Last tail sefment
	public GameObject tailEndPrefab; //Last part
	public AnimationCurve tailScaleCurve = AnimationCurve.Linear(0,0,1,1); //Controls how it tapers


	[Header("Body Composition")]
	public int neckSegmentCount = 1;
	public bool useFrontLegs = true;
	public bool useBackLegs = true;
	public int bodySegmentCount = 5;
	public int tailSegmentCount = 20;
	public bool useTailEnd = true;

	[Header("Spacing")]
	public float neckSpacing = 0.4f;
	public float bodySpacing = 0.4f;
	public float tailSpacing = 0.4f;

	[Header("Other")]
	public float animationTimeOffset = 0.2f;
	public Transform dragonHead;
	public List<DragonBodySegment> bodySegments = new List<DragonBodySegment>();

	void Start()
	{
		if (dragonHead == null)
		{
			dragonHead = transform; //Uses own gameobject if no head assigned
		}
		Invoke("CreateDragonBody",0.1f);//Calls method after delay
	}

	void CreateDragonBody()
	{
		for (int i =0; i < neckSegmentCount; i++) //Neck 
		{
			GameObject prefab = neckSegmentPrefab !=null ? neckSegmentPrefab : bodySegmentPrefab; //Loops to create neck segments, if no neck segment uses bodySegment.
			AddSegment(prefab, neckSpacing, $"Neck_{i}"); //String interpolation, Neck_0, Neck_1, etc.
		}

		if (useFrontLegs && frontLegSegmentPrefab != null) // Front legs
		{
			AddSegment(frontLegSegmentPrefab, bodySpacing, "FrontLegs");
		}

		for (int i =0; i < bodySegmentCount; i++) // Body
		{
			AddSegment(bodySegmentPrefab,bodySpacing,$"Body_{i}");
		}
		
		if (useBackLegs && backLegSegmentPrefab !=null)
		{
			AddSegment(backLegSegmentPrefab,bodySpacing,"BackLegs");
		}
		
		CreateTailSegments();
	}
	
	void CreateTailSegments()
	{
		for (int i = 0; i < tailSegmentCount; i++)
		{
			GameObject prefab = tailSegmentPrefab != null? tailSegmentPrefab : bodySegmentPrefab; //if no tail use body
		
			float t = (float)i / (tailSegmentCount - 1); //0-1 normalized
float curveValue = tailScaleCurve.Evaluate(t); //Apply curve
float scale = Mathf.Lerp(tailStartScale,tailEndScale, curveValue); //interpolates from start to end with curvevalue

AddSegment(prefab,tailSpacing,$"Tail_{i+1}",scale);
}
if (useTailEnd && tailEndPrefab != null) //tail end exist
{
	AddSegment(tailEndPrefab,tailSpacing, "TailEnd", tailEndScale * 0.3f); //Scales tip down
}
	}

	void AddSegment(GameObject prefab, float spacing, string segmentName, float scale = 1f)
	{
		if (prefab == null) prefab = bodySegmentPrefab; //if not set, set to body
		if (prefab == null) return; //if none don't do anything
		
		GameObject newSegmentObj = Instantiate(prefab); //spawns
		newSegmentObj.transform.SetParent(transform); //becomes parent of new segment
		newSegmentObj.transform.localScale = new Vector3(scale,scale,1f);//1f stays constant
		DragonBodySegment newSegment = newSegmentObj.GetComponent<DragonBodySegment>(); //Gets the dragonbodysegment script		
		if (newSegment == null)
		{
			Destroy(newSegmentObj);
			return; //if missing destroy object, prevents issue
		}

		Transform targetToFollow;
		Vector3 spawnPosition;
		Quaternion spawnRotation; //Declares what new segment follows and where it spawns

		if (bodySegments.Count == 0)
		{
			targetToFollow = dragonHead;
			spawnPosition = dragonHead.position - dragonHead.forward * spacing;
			spawnRotation = dragonHead.rotation; // Follows head if theres no segments, behind.
		}

		else
		{
			Transform previousSegment = bodySegments[bodySegments.Count-1].transform; //Get's last index
			targetToFollow = previousSegment;
			spawnPosition = previousSegment.position - previousSegment.forward*spacing; //Spawns behind previous segment
			spawnRotation = previousSegment.rotation; //previous segments rotation
		}

		newSegment.followTarget = targetToFollow;//What segment follows
		newSegment.segmentDistance=spacing;//*scale; how far behind, if *scale is included scales by size too
		newSegment.segmentIndex=bodySegments.Count; //assigns index number, after giving list size
		newSegment.animationTimeOffset = animationTimeOffset; //gives time offset for animation
		newSegmentObj.transform.position = spawnPosition;
		newSegmentObj.transform.rotation = spawnRotation;//positions and rotates
		newSegmentObj.name=segmentName; //renames gameobject in hierarchy
		bodySegments.Add(newSegment); //adds to list
	}
	
	public void AddSegmentDuring()
	{
		AddSegment(bodySegmentPrefab, bodySpacing, $"Added_{bodySegments.Count}");
	}
	
	public void RemoveSegment()
	{
		if (bodySegments.Count <= 5) return; //Can't go below 5
		DragonBodySegment lastSegment = bodySegments[bodySegments.Count-1]; //Last one in list
		bodySegments.RemoveAt(bodySegments.Count-1); //Removes from list
		Destroy(lastSegment.gameObject); //Destroys
	}

	void Update()
	{
		// if (Input.GetKeyDown(KeyCode.M)) Testing, can be for other counts
		//{
		//	AddSegmentDuring();
		//}
		//if (Input.GetKeyDown(KeyCode.N))
		//{
		//	RemoveSegment();
		//}
	}
}
