using System.Collections.Generic;
using UnityEngine;

public class TriggerCount : MonoBehaviour
{
    private HashSet<int> playersInTrigger = new HashSet<int>();
    
    
    //This is a weird thing to prevent bugs
    private bool isFirstPlayerEntered = true; // Flag to track the first player entry
    private float timer;


    public int PlayerCount => playersInTrigger.Count;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playersInTrigger.Contains(other.gameObject.GetInstanceID()))
        {
            int previousCount = playersInTrigger.Count;
            playersInTrigger.Add(other.gameObject.GetInstanceID());
            Debug.Log("Player entered. Player count: " + playersInTrigger.Count);
            Debug.Log(Time.frameCount);

            if (!isFirstPlayerEntered && previousCount < playersInTrigger.Count
                && TryGetComponent(out AnimationMaterialBlink materialBlink))
            {
                materialBlink.PlayBlinkOnce();
            }

            if (isFirstPlayerEntered) // Toggle the flag for subsequent players
            {
                isFirstPlayerEntered = false;
            }
        }
    }

    private void Update()
    {

        //weird bug prevention
        if(PlayerCount == 0)
        {
            timer += Time.deltaTime;
            if(timer > 1.5f)
            {
                isFirstPlayerEntered = true;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playersInTrigger.Contains(other.gameObject.GetInstanceID()))
        {
            playersInTrigger.Remove(other.gameObject.GetInstanceID());
            Debug.Log("Player exited. Player count: " + PlayerCount);
            Debug.Log(Time.frameCount);
        }
    }
}
