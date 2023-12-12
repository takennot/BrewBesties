using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacePlayerValues : MonoBehaviour
{
    private PlayerScript[] players;

    public bool needsTrigger;
    
    [Header("Movement")]
    public bool isSlippery = true;
    public float acceleration = 0.95f;
    public float extraSpeed = 2;

    private float originalSpeed;
    private bool originalIsSlippery;
    private float originalAcceleration;

    private float timer;

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
        originalIsSlippery = players[0].isSlippery;
        originalAcceleration = players[0].accelerationRate;

        if (!needsTrigger)
        {
            UpdateValues();
        }
    }

    [ContextMenu("Update Values")]
    private void UpdateValues()
    {
        foreach (PlayerScript player in players)
        {
            player.isSlippery = isSlippery;
            player.accelerationRate = acceleration;
            player.playerSpeed = originalSpeed + extraSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        UpdateValues();
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (PlayerScript player in players)
        {
            player.isSlippery = originalIsSlippery;
            player.accelerationRate = originalAcceleration;
            player.playerSpeed = originalSpeed;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        timer += Time.deltaTime;
        if(timer > 0.15f)
        {
            timer = 0;
            UpdateValues();
        }
    }
}        
