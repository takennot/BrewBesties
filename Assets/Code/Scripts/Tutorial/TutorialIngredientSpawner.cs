using UnityEngine;

public class TutorialIngredientSpawner : MonoBehaviour
{
    public GameObject[] ingredients;
    public float minSpawnInterval = 2f; // Minimum time between spawns
    public float maxSpawnInterval = 7f; // Maximum time between spawns
    public Vector3 spawnPointVariance = Vector3.one; // Variance in spawn point
    private Transform spawnPoint; // Where the objects will be spawned
    public Vector3 velocityDirection = Vector3.forward; // Direction of the constant velocity
    public Vector3 velocityVariance = Vector3.one; // Amount of randomness in velocity direction
    public float minTorqueForce = 0.04f; // Minimum torque force to apply rotation
    public float maxTorqueForce = 0.11f; // Maximum torque force to apply rotation

    private float timer; // Timer to track spawning intervals

    void Start()
    {
        // Set the timer initially to a random value within the specified range
        timer = Random.Range(minSpawnInterval, maxSpawnInterval);
        spawnPoint = transform;
    }

    void Update()
    {
        // Decrement the timer
        timer -= Time.deltaTime;

        // If the timer reaches zero or below, spawn an object and reset the timer
        if (timer <= 0)
        {
            SpawnIngredient();

            // Set the timer to a new random value within the specified range
            timer = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnIngredient()
    {
        // Randomly select an ingredient from the array
        int randomIndex = Random.Range(0, ingredients.Length);
        GameObject ingredientPrefab = ingredients[randomIndex];

        // Calculate the spawn position with variance
        Vector3 randomizedSpawnPoint = spawnPoint.position + new Vector3(
            Random.Range(-spawnPointVariance.x, spawnPointVariance.x),
            Random.Range(-spawnPointVariance.y, spawnPointVariance.y),
            Random.Range(-spawnPointVariance.z, spawnPointVariance.z)
        );

        // Spawn the selected ingredient at the randomized spawn point
        GameObject spawnedIngredient = Instantiate(ingredientPrefab, randomizedSpawnPoint, Quaternion.identity);

        // Get the Rigidbody component
        Rigidbody rb = spawnedIngredient.GetComponent<Rigidbody>();

        // Calculate the random direction within the variance range
        Vector3 randomizedDirection = velocityDirection + new Vector3(
            Random.Range(-velocityVariance.x, velocityVariance.x),
            Random.Range(-velocityVariance.y, velocityVariance.y),
            Random.Range(-velocityVariance.z, velocityVariance.z)
        );

        // Set the normalized direction as the velocity of the Rigidbody directly
        if (rb != null)
        {
            rb.velocity = randomizedDirection.normalized;

            // Apply torque force to create rotational movement
            rb.AddTorque(Random.onUnitSphere * Random.Range(minTorqueForce, maxTorqueForce), ForceMode.Impulse);
        }
    }
}
