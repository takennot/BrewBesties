using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private WS_MagicField magicField;

    [SerializeField] private AudioClip stepOn;    
    [SerializeField] private AudioClip stepOff;
    private AudioSource source;

    [SerializeField] private float plateMoveDownAmount;
    [SerializeField] private float smoothSpeed = 5f;

    private Vector3 plateStartPosition;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        plateStartPosition = gameObject.transform.position;
        targetPosition = plateStartPosition;
    }

    private void Update()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            magicField.SetIsActive(true);
            targetPosition = new Vector3(plateStartPosition.x, plateStartPosition.y - plateMoveDownAmount, plateStartPosition.z);
            source.PlayOneShot(stepOn);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            magicField.SetIsActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            magicField.SetIsActive(false);
            targetPosition = plateStartPosition;
            source.PlayOneShot(stepOff);
        }
    }
}
