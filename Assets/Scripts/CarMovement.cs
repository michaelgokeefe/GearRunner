using UnityEngine;
using Random = UnityEngine.Random;

public class CarMovement : MonoBehaviour
{
    public string StopForThisTagName = "Traffic";
    public float VisibilityMax = 10;
    public float VisibiltyMin = 5;
    public float StartVelocity = 5;
    public float Acceleration = 3f;

    private float visibility;
    private float velocity;
    private float previousDistanceBetweenCars = 0f;
    private bool stopped;
    private float maxVelocity;

	// Use this for initialization
	void Start () {
        visibility = Random.Range(VisibiltyMin, VisibilityMax);
	    velocity = StartVelocity;
	    maxVelocity = StartVelocity;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (IsSlowDownAhead())
	    {
	        DecelerateToStop();
	    }
	    else
	    {
	        AccelerateToMaxVelocity();
	    }

	    MoveForward();
	   
	}

    private bool IsSlowDownAhead()
    {
        RaycastHit raycastHit;
        bool slowDownAhead = false;

        if (Physics.Raycast(transform.position, Vector3.forward, out raycastHit, visibility))
        {
            if (raycastHit.collider.tag == StopForThisTagName)
            {
                if (raycastHit.distance < previousDistanceBetweenCars)
                {
                    Debug.Log("Slow down ahead");
                    slowDownAhead = true;
                }
                previousDistanceBetweenCars = raycastHit.distance;
            }
        }
        return slowDownAhead;
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * velocity);
    }

    private void AccelerateToMaxVelocity()
    {
        if (velocity < maxVelocity)
        {
            velocity += Time.deltaTime * Acceleration;
            Debug.Log("Accelerating");
        }
    }

    private void DecelerateToStop()
    {
        if (velocity > 0)
        {
            velocity -= Time.deltaTime * Acceleration;
            Debug.Log("Decelerating");
        }
        else
        {
            velocity = 0; // Stopped, not reversed 
        }
    }
}
