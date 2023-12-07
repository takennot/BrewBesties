using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform platform1;
    public Transform platform2;

    public float moveDistance = 2.5f;
    public float moveDuration = 1.0f; // Time to move up or down
    public float delayBetweenMovements = 10.0f; // Time between movements

    [Header("Animation Shake")]
    [SerializeField] private float startShakeSecondsBeforeMoving = 8f;
    [SerializeField] private float maxShakeIntensity = 15f; 
    [SerializeField] private float shakeSpeed = 1.0f; 
    [SerializeField] private float shakeDuration = 11.0f;
    [Range(0.5f, 0.99f)] public float intensityRampUp = 0.8f;

    private AnimationShake animShake1;
    private AnimationShake animShake2;

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

        animShake1.maxShakeIntensity = maxShakeIntensity / 100;
        animShake1.shakeSpeed = shakeSpeed;
        animShake1.shakeDuration = shakeDuration;
        animShake1.intensityRampUp = intensityRampUp;

        animShake2.maxShakeIntensity = maxShakeIntensity / 100;
        animShake2.shakeSpeed = shakeSpeed;
        animShake2.shakeDuration = shakeDuration;
        animShake2.intensityRampUp = intensityRampUp;

        if (animShake1 == null || animShake2 == null)
            Debug.LogWarning("AnimationShake component not found on platform1 or platform2");

        // Ensure platforms' initial positions are set correctly
        platform1.position = new Vector3(platform1.position.x, platform1StartY, platform1.position.z);
        platform2.position = new Vector3(platform2.position.x, platform2StartY - moveDistance, platform2.position.z);


    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= delayBetweenMovements - startShakeSecondsBeforeMoving)
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
                StartCoroutine(MovePlatform(platform1, platform1StartY - moveDistance, moveDuration));
                StartCoroutine(MovePlatform(platform2, platform2StartY, moveDuration));
            } else
            {
                StartCoroutine(MovePlatform(platform1, platform1StartY, moveDuration));
                StartCoroutine(MovePlatform(platform2, platform2StartY - moveDistance, moveDuration));
            }

            platform1Up = !platform1Up;
        }
    }

    IEnumerator MovePlatform(Transform platform, float targetY, float duration)
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