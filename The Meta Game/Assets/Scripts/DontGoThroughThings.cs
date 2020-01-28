// This script is a modification of the script by Daniel Brauer and Adrian
// source: https://wiki.unity3d.com/index.php/DontGoThroughThings

using UnityEngine;
using System.Collections;
 
public class DontGoThroughThings : MonoBehaviour
{
       // Careful when setting this to true - it might cause double
       // events to be fired - but it won't pass through the trigger
       public bool sendTriggerMessage = false; 	
 
	public LayerMask layerMask = -1; //make sure we aren't in this layer 
	public float skinWidth = 0.1f; //probably doesn't need to be changed 
 
	private float minimumExtent; 
	private float partialExtent; 
	private float sqrMinimumExtent; 
	private Vector3 previousPosition; 
	private Rigidbody2D rb;
	private Collider2D[] cols;
 
	//initialize values 
	private void Start() 
	{ 
	   rb = GetComponent<Rigidbody>();
	   cols = GetComponentsInChildren<Collider>();
	   previousPosition = rb.position;
	   minimumExtent = MinColBounds();
	   partialExtent = minimumExtent * (1.0f - skinWidth); 
	   sqrMinimumExtent = minimumExtent * minimumExtent; 
	} 
 
	private void FixedUpdate() 
	{ 
	   //have we moved more than our minimum extent? 
	   Vector3 movementThisStep = myRigidbody.position - previousPosition; 
	   float movementSqrMagnitude = movementThisStep.sqrMagnitude;
 
	   if (movementSqrMagnitude > sqrMinimumExtent) 
		{ 
	      float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
	      RaycastHit hitInfo; 
 
	      //check for obstructions we might have missed 
	      if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
              {
                 if (!hitInfo.collider)
                     return;
 
                 if (hitInfo.collider.isTrigger) 
                     hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);
 
                 if (!hitInfo.collider.isTrigger)
                     myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent; 
 
              }
	   } 
 
	   previousPosition = myRigidbody.position; 
	}

	private float MinColBounds()
	{
		float min = Mathf.Infinity;
		foreach(Collider2D col in cols)
		{
			float minBound = Mathf.Min(col.bounds.extents.x, col.bounds.extents.y)
			if (minBound < min)
			{
				min = minBound;
			}
		}

		return min;
	}
}