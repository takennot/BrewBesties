using UnityEngine;

public class AnimationShake : MonoBehaviour
{
    public float maxShakeIntensity = 10f; // Maximum intensity of the shake
    public float shakeSpeed = 10.0f; // Speed of the shake
    public float shakeDuration = 1.0f; // Duration of the shake
    [Range(0.5f, 0.99f)] public float intensityRampUp = 0.8f; // 80% threshold for intensity increase

    private Vector3 originalPosition;
    private float shakeTimer = 0.0f;

    private bool shouldShake;

    private void Awake()
    {
        originalPosition = transform.position;
        maxShakeIntensity /= 100f;
    }

    void Update()
    {
        if (!shouldShake)
            return;

        if (shakeTimer <= shakeDuration)
        {
            // Calculate the shake intensity based on the progress of shakeTimer
            float normalizedTime = shakeTimer / shakeDuration;

            float intensity;
            if (normalizedTime <= 0.8f) // Gradually increase intensity to 80% of the duration
            {
                intensity = Mathf.Lerp(0f, maxShakeIntensity, normalizedTime / intensityRampUp);
            } else // Decrease intensity from 80% to 100% of the duration
            {
                intensity = Mathf.Lerp(maxShakeIntensity, 0f, (normalizedTime - intensityRampUp) / (1f - intensityRampUp));
            }

            // Generate random values for shaking
            float offsetX = Random.Range(-intensity, intensity);
            float offsetZ = Random.Range(-intensity, intensity);

            // Calculate the new position with a Perlin noise-based smooth shake effect
            float shakeX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) * offsetX;
            float shakeZ = Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed) * offsetZ;

            // Apply the shake to the object's position
            transform.position = originalPosition + new Vector3(shakeX, transform.position.y, shakeZ);

            shakeTimer += Time.deltaTime;
        } else
        {
            // Reset the object's position after shake duration ends
            transform.position = new Vector3(originalPosition.x, transform.position.y, originalPosition.z);
            shakeTimer = 0.0f;
            shouldShake = false;
        }
    }

    [ContextMenu("Shake")]
    public void Shake()
    {
        if (shouldShake)
            return;

        Debug.Log("Triggered Shake on" + gameObject.name);
        shouldShake = true;
        shakeTimer = 0;
    }
}
