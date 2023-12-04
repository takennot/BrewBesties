using System.Collections;
using UnityEngine;

public class AnimationRotateInterval : MonoBehaviour
{
    public float rotationSpeed = 90f; // Degrees per second
    [SerializeField] private float rotationDelaySeconds = 5f;

    private float totalRotation = 0f;

    void Start()
    {
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            float remainingRotation = 90f - totalRotation;

            float angleThisFrame = Mathf.Min(rotationSpeed * Time.deltaTime, remainingRotation);
            transform.Rotate(Vector3.up, angleThisFrame);
            totalRotation += angleThisFrame;

            if (Mathf.Approximately(totalRotation, 90f))
            {
                yield return new WaitForSeconds(rotationDelaySeconds);
                totalRotation = 0f; // Reset for the next rotation
            }

            yield return null;
        }
    }
}