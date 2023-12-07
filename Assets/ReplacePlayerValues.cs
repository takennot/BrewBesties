using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacePlayerValues : MonoBehaviour
{
    private PlayerScript[] players;

    [Header("Movement")]
    public bool isSlippery;
    public float acceleration;
    public float extraSpeed;
    
    private float originalSpeed;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(0.1f);
        players = FindObjectsOfType<PlayerScript>();

        originalSpeed = players[0].playerSpeed;

        foreach (PlayerScript player in players)
        {
            player.isSlippery = isSlippery;
            player.accelerationRate = acceleration;
            player.playerSpeed = originalSpeed += extraSpeed;
        }
    }

    [ContextMenu("Update Values")]
    private void UpdateValues()
    {
        foreach (PlayerScript player in players)
        {
            player.isSlippery = isSlippery;
            player.accelerationRate = acceleration;
            player.playerSpeed = originalSpeed += extraSpeed;
        }
    }
    
}
