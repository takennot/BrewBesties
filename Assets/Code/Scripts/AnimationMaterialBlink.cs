using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AnimationMaterialBlink : MonoBehaviour
{
    [SerializeField] private Renderer rend; // Reference to the Renderer component of your GameObject
    [SerializeField] private AudioSource source;

    private Material mat;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;
        originalColor = mat.GetColor("_DepthWater1"); // Assuming the parameter name is DepthWater1
    }

    public void PlayBlinkOnce()
    {
        source.PlayOneShot(source.clip);
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        float duration = 0.17f; // Duration of the blink effect
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = Mathf.PingPong(elapsedTime, duration) / duration; // PingPong for smoother transition
            Color currentColor = Color.Lerp(originalColor, new(originalColor.r, originalColor.g, originalColor.b, 1), t);

            mat.SetColor("_DepthWater1", currentColor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mat.SetColor("_DepthWater1", originalColor); // Ensure the final color is the original color
    }
}
