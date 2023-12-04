using UnityEngine;

public class ForceLook : MonoBehaviour
{
    public GameObject objectToLookAt;
    public float rotationSpeed = 90.0f;
    public float maxAngleThreshold = 15f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && objectToLookAt != null)
        {
            PlayerScript playerScript = other.GetComponent<PlayerScript>();
            Vector3 directionToTarget = objectToLookAt.transform.position - other.transform.position;

            // Calculate the angle between the player's forward direction and the direction towards the target
            float angle = Vector3.Angle(other.transform.forward, directionToTarget);

            if (angle <= maxAngleThreshold)
            {
                // Rotate towards the target only when within the angle threshold
                playerScript.SetHasForcedLook(true);
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                other.transform.rotation = Quaternion.RotateTowards(other.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerScript>().SetHasForcedLook(false);
        }
    }
}
