using UnityEngine;

public class CameraCheatDebug : MonoBehaviour
{
    public float movementSpeed = 5f; 
    public float fastMovementSpeed = 10f;
    public float rotationSpeed = 25f;
    private bool isControlEnabled = false;
    private float originalRotationSpeed;

    private CameraSway cameraSway;

    private void Start()
    {
        originalRotationSpeed = rotationSpeed;

        cameraSway = GetComponent<CameraSway>();
           
    }

    void Update()
    {
        // Toggle control with keypad 0
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            isControlEnabled = !isControlEnabled;
            Debug.Log("Camera Fly Mode: " + isControlEnabled);
            if(cameraSway != null)
                cameraSway.enabled = !isControlEnabled;
        }

        if (isControlEnabled)
        {
            float horizontalInput = 0f;
            float verticalInput = 0f;
            float verticalSpeed = movementSpeed;
            float rotationSpeed = originalRotationSpeed;

            // Detta var det enda som fungerad "Vertical" osv fungerade inte lol
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                verticalInput = 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                verticalInput = -1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                horizontalInput = -1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                horizontalInput = 1f;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                verticalSpeed = fastMovementSpeed;
                rotationSpeed = rotationSpeed * 2;
            } 

            Vector3 translation = new Vector3(horizontalInput, 0f, verticalInput) * verticalSpeed * Time.deltaTime;
            transform.Translate(translation, Space.World);

            //Rotate
            if (Input.GetKey(KeyCode.Mouse0)) 
                transform.Rotate(Vector3.left, +rotationSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.Mouse1)) 
                transform.Rotate(Vector3.left, -rotationSpeed * Time.deltaTime);

            // Move the camera up and down on the Y-axis with Q and E keys
            if (Input.GetKey(KeyCode.Q))
                transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime, Space.World);

            if (Input.GetKey(KeyCode.E))
                transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime, Space.World);
        }
    }
}
