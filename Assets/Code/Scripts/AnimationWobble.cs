using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWobble : MonoBehaviour
{
    public float wobbleSpeed = 5.0f; 
    public float wobbleMagnitude = 0.1f; 

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position using a sine function to create a wobbling effect
        float newY = initialPosition.y + Mathf.Sin(Time.time * wobbleSpeed) * wobbleMagnitude;

        // Apply the new position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
