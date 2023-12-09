using UnityEngine;

public class TravelBetweenPoints : MonoBehaviour
{
    public GameObject objectToMove; // GameObject to move
    public Transform pointA; // Define the starting point
    public Transform pointB; // Define the ending point
    public float speed = 2.0f; // Define the movement speed

    private Transform destination;
    private bool isMovingTowardsB = true;

    void Start()
    {
        objectToMove.transform.position = pointA.transform.position;
        SetDestination(pointB);
    }

    void Update()
    {
        // Calculate the direction and move the object
        Vector3 direction = destination.position - objectToMove.transform.position;
        float distanceToMove = speed * Time.deltaTime;

        // Check if the object has reached or passed the destination
        if (direction.magnitude <= distanceToMove)
        {
            // Change destination and direction
            if (isMovingTowardsB)
                SetDestination(pointA);
            else
                SetDestination(pointB);

            isMovingTowardsB = !isMovingTowardsB;
        }

        // Move the object
        objectToMove.transform.Translate(direction.normalized * distanceToMove, Space.World);
    }

    void SetDestination(Transform dest)
    {
        destination = dest;
    }
}
