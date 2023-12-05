using UnityEngine;

public class CameraSway : MonoBehaviour
{
    [Range(0, 1.5f)]
    public float swayAmount = 0.5f;
    [Range(0, 2f)]
    public float swaySpeed = 0.5f; 

    private Vector3 initialPosition;
    private Vector2 currentSway;

    void Start()
    {
        initialPosition = transform.position; 
    }

    void Update()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayY = Mathf.Cos(Time.time * swaySpeed) * swayAmount;

        transform.position = initialPosition + new Vector3(swayX, swayY, 0);

        currentSway = new Vector2(swayX, swayY);
    }

    public Vector2 GetCurrentSway()
    {
        return currentSway;
    }
}
