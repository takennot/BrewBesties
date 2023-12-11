using UnityEngine;

public class TravelBetweenPoints : MonoBehaviour
{
    public GameObject objectToMove; // GameObject to move
    public Transform pointA; // Define the starting point
    public Transform pointB; // Define the ending point
    public Transform stopPoint; // Define the point to stop at
    public float speed = 2.0f; // Define the movement speed

    private Transform destination;
    private bool isMoving = true;

    void Start()
    {
        objectToMove.transform.position = pointA.position;
        SetDestination(pointB);
    }

    void Update()
    {
        if (!isMoving)
            return;

        // Calculate the direction and move the object
        Vector3 direction = destination.position - objectToMove.transform.position;
        float distanceToMove = speed * Time.deltaTime;

        // Check if the object has reached or passed the destination
        if (direction.magnitude <= distanceToMove)
        {
            // Move to the next destination
            if (destination == pointA)
                SetDestination(pointB);
            else if (destination == pointB)
                SetDestination(pointA);
            else if (destination == stopPoint)
            {
                isMoving = false; // Reached the stop point, stop moving
                objectToMove.transform.position = stopPoint.position; // Move to the exact stop point
            }
        }

        // Move the object
        objectToMove.transform.Translate(direction.normalized * distanceToMove, Space.World);
    }

    void SetDestination(Transform dest)
    {
        destination = dest;
    }

    public void MoveToStopPoint()
    {
        isMoving = true; // Start moving
        SetDestination(stopPoint); // Move to the stop point
    }
}
