using UnityEngine;

public class AnimationSpriteCycler : MonoBehaviour
{
    public Sprite[] sprites; // Array to hold your sprites
    public float cycleInterval = 1.0f; // Time interval between sprite changes

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;
    private float timer = 0.0f;

    public bool useModifiedThirdSprite = false; // Flag to check if the third sprite is modified
    public Color modifiedColor = Color.blue; // Color for the third sprite
    public Vector3 modifiedScale = new Vector3(1.5f, 1.5f, 1.0f); // Scale for the third sprite

    private Vector3 originalScale;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = spriteRenderer.gameObject.transform.localScale;

        if (sprites.Length > 0 && spriteRenderer != null)
        {
            // Set the initial sprite
            spriteRenderer.sprite = sprites[0];
        } else
        {
            Debug.LogError("Sprites array is empty or SpriteRenderer is not attached!");
            enabled = false; // Disable the script if there's an issue
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cycleInterval)
        {
            timer = 0.0f;
            currentIndex = (currentIndex + 1) % sprites.Length;
            spriteRenderer.sprite = sprites[currentIndex];

            if(useModifiedThirdSprite)
            {
                if (currentIndex == 2)
                {
                    // Apply modified color and scale to the third sprite
                    spriteRenderer.color = modifiedColor;
                    transform.localScale = modifiedScale;
                } else
                {
                    // Reset color and scale for other sprites
                    spriteRenderer.color = Color.white;
                    transform.localScale = originalScale;
                }
            }


        }
    }
}
