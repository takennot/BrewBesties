using UnityEngine;

public class CollisionCustom : MonoBehaviour
{
    public float minPushForce = 0.6f; // Minimum push force
    public float maxPushForce = 4f; // Maximum push force
    public float pushCooldown = 0.5f; // Cooldown time for the push

    private float timer = 0;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && timer <= 0)
        {
            PlayerScript playerScript = other.GetComponent<PlayerScript>();

            // Get the direction the player is moving
            Vector3 playerDirection = playerScript.GetMoveDirection().normalized;

            // Get the magnitude of the player's movement
            float playerSpeed = playerScript.GetMovementSpeed();

            // Get the direction from the center of the trigger to the player
            Vector3 triggerToPlayer = other.transform.position - transform.position;
            triggerToPlayer.y = 0; // Ignore vertical displacement

            // Push in the direction the player is moving, scaled by speed
            Vector3 pushDirection = playerDirection * playerSpeed;

            // Apply the push force to the object based on the speed
            float pushForce = Mathf.Lerp(minPushForce, maxPushForce, playerSpeed);
            Rigidbody objectRb = GetComponent<Rigidbody>();
            if (objectRb != null)
            {
                objectRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }

            // Start the cooldown timer
            timer = pushCooldown;
        }
    }
}
