using UnityEngine;

public class CameraSway : MonoBehaviour
{
    [Range(0, 1.5f)]
    public float swayAmount = 0.5f;
    [Range(0, 2f)]
    public float swaySpeed = 0.5f; 

    private Vector3 initialPosition;
    private Vector2 currentSway;

    [Header("Sway directoin")]
    [SerializeField] private bool swayXY = false;
    [SerializeField] private bool swayXZ = false;
    [SerializeField] private bool swayYZ = true;

    void Start()
    {
        initialPosition = transform.position; 
    }

    void Update()
    {
        if(swayXY)
        {
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayY = Mathf.Cos(Time.time * swaySpeed) * swayAmount;

            transform.position = initialPosition + new Vector3(swayX, swayY, 0);

            currentSway = new Vector2(swayX, swayY);

        } else if (swayXZ)
        {
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayZ = Mathf.Cos(Time.time * swaySpeed) * swayAmount;

            transform.position = initialPosition + new Vector3(swayX, 0, swayZ);

            currentSway = new Vector2(swayX, swayZ);
        } else if (swayYZ)
        {
            float swayY = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayZ = Mathf.Cos(Time.time * swaySpeed) * swayAmount;

            transform.position = initialPosition + new Vector3(0, swayY, swayZ);

            currentSway = new Vector2(swayY, swayZ);
        }

    }

    public Vector2 GetCurrentSway()
    {
        return currentSway;
    }
}
