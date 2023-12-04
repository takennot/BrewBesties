using System.Collections;
using UnityEngine;

public class Prototype_SpawnAndMove : MonoBehaviour
{
    public GameObject[] objectPrefabs; // Array of prefabs of objects to spawn
    public Transform[] waypoints; // Waypoints defining the path
    public float spawnInterval = 2f; // Interval between spawns in seconds
    public float speed = 5f; // Speed of the object along the path

    void Start()
    {
        // Start the object spawning coroutine
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            // Randomly choose an object prefab from the array
            int randomIndex = Random.Range(0, objectPrefabs.Length);
            GameObject selectedPrefab = objectPrefabs[randomIndex];

            // Spawn a new object with an initial rotation of 90 degrees around the Y-axis
            GameObject newObject = Instantiate(selectedPrefab, waypoints[0].position, Quaternion.Euler(0f, 90f, 0f));

            // Set up object movement along the custom path
            StartCoroutine(MoveAlongPath(newObject));

            // Wait for the specified spawn interval before spawning the next object
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator MoveAlongPath(GameObject obj)
    {
        for (int i = 1; i < waypoints.Length; i++)
        {
            float distance = Vector3.Distance(obj.transform.position, waypoints[i].position);
            float duration = distance / speed;

            float elapsedTime = 0f;
            Vector3 startPos = obj.transform.position;

            while (elapsedTime < duration)
            {
                obj.transform.position = Vector3.Lerp(startPos, waypoints[i].position, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        Destroy(obj); // Destroy the object when it reaches the end of the path
    }
}
