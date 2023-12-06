using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private WS_MagicField[] magicFields;

    [SerializeField] private AudioClip stepOn;
    [SerializeField] private AudioClip stepOff;
    private AudioSource source;

    [SerializeField] private float plateMoveDownAmount;
    [SerializeField] private float smoothSpeed = 5f;

    private Vector3 plateStartPosition;
    private Vector3 targetPosition;

    private int playersOnPlate = 0;

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
            playersOnPlate++;
            foreach (WS_MagicField magicField in magicFields)
            {
                magicField.ReverseState(false);
            }
            targetPosition = new Vector3(plateStartPosition.x, plateStartPosition.y - plateMoveDownAmount, plateStartPosition.z);
            source.PlayOneShot(stepOn);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersOnPlate--;
            if (playersOnPlate <= 0)
            {
                foreach (WS_MagicField magicField in magicFields)
                {
                    magicField.ReverseState(true);
                }
                targetPosition = plateStartPosition;
                source.PlayOneShot(stepOff);
            }
        }
    }
}