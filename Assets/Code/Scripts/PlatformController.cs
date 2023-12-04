using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform platform1;
    public Transform platform2;
    public AnimationShake animShake1;
    public AnimationShake animShake2;
    public float moveDistance = 2.5f;
    public float moveDuration = 1.0f; // Time to move up or down
    public float delayBetweenMovements = 10.0f; // Time between movements

    private float platform1StartY;
    private float platform2StartY;
    private float timer = 0.0f;
    private bool isMoving = false;
    private bool platform1Up = true; // Start with platform1 up

    void Start()
    {
        platform1StartY = platform1.position.y;
        platform2StartY = platform2.position.y;

        // Set the second platform to start in the down position
        platform2.position = new Vector3(platform2.position.x, platform2StartY - moveDistance, platform2.position.z);

        animShake1 = platform1.GetComponent<AnimationShake>();
        animShake2 = platform2.GetComponent<AnimationShake>();

        if (animShake1 == null || animShake2 == null)
            Debug.LogWarning("AnimationShake component not found on platform1 or platform2");

        // Ensure platforms' initial positions are set correctly
        platform1.position = new Vector3(platform1.position.x, platform1StartY, platform1.position.z);
        platform2.position = new Vector3(platform2.position.x, platform2StartY - moveDistance, platform2.position.z);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= delayBetweenMovements - 6f)
        {
            if (platform1Up)
                animShake1.Shake();
            else
                animShake2.Shake();
        }

        if (!isMoving && timer >= delayBetweenMovements)
        {
            isMoving = true;
            timer = 0.0f;

            if (platform1Up)
            {
                StartCoroutine(MovePlatform(platform1, platform1StartY - moveDistance, moveDuration, animShake1));
                StartCoroutine(MovePlatform(platform2, platform2StartY, moveDuration, animShake2));
            } else
            {
                StartCoroutine(MovePlatform(platform1, platform1StartY, moveDuration, animShake1));
                StartCoroutine(MovePlatform(platform2, platform2StartY - moveDistance, moveDuration, animShake2));
            }

            platform1Up = !platform1Up;
        }
    }

    IEnumerator MovePlatform(Transform platform, float targetY, float duration, AnimationShake shakeScript)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = platform.position;
        Vector3 finalPos = new Vector3(platform.position.x, targetY, platform.position.z);

        while (elapsedTime < duration)
        {
            float newY = Mathf.Lerp(startingPos.y, finalPos.y, elapsedTime / duration);
            platform.position = new Vector3(platform.position.x, newY, platform.position.z);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        platform.position = finalPos;

        isMoving = false;
    }
}