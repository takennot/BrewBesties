using System.Collections.Generic;
using UnityEngine;

public class TriggerCount : MonoBehaviour
{
    private HashSet<int> playersInTrigger = new HashSet<int>();

    public int PlayerCount => playersInTrigger.Count; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playersInTrigger.Contains(other.gameObject.GetInstanceID()))
        {
            playersInTrigger.Add(other.gameObject.GetInstanceID()); 
            Debug.Log("Player entered. Player count: " + PlayerCount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playersInTrigger.Contains(other.gameObject.GetInstanceID()))
        {
            playersInTrigger.Remove(other.gameObject.GetInstanceID());
            Debug.Log("Player exited. Player count: " + PlayerCount);
        }
    }
}
