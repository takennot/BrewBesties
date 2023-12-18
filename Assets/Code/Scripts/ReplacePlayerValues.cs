using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacePlayerValues : MonoBehaviour
{
    private PlayerScript[] players;

    public bool needsTriggerForMovement;

    [Header("Player Strength, 0 = original")]
    public float horizontalThrowForcePlayerExtra = 0;
    public float verticalThrowForcePlayerExtra = 0;
    public float horizontalThrowForceItemExtra = 0;
    public float verticalThrowForceItemExtra = 0;

    private float originalHorizontalThrowForcePlayer = 0;
    private float originalVerticalThrowForcePlayer = 0;
    private float originalHorizontalThrowForceItem = 0;
    private float originalVerticalThrowForceItem = 0;

    [Header("Movement")]
    public bool changeMovement = true;
    public bool isSlippery = true;
    public float acceleration = 0.95f;
    public float extraSpeed = 2;

    private float originalSpeed;
    private bool originalIsSlippery;
    private float originalAcceleration;

    [Header("Exit Values")]
    public bool hasExitValues;
    public ReplacePlayerValues replacePlayerValues;

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

        originalHorizontalThrowForcePlayer = players[0].horizontalThrowForcePlayer;
        originalVerticalThrowForcePlayer = players[0].verticalThrowForcePlayer;
        originalHorizontalThrowForceItem = players[0].horizontalThrowForce;
        originalVerticalThrowForceItem = players[0].verticalThrowForce;

        if (!needsTriggerForMovement)
        {
            UpdateValues();
        }
    }


    private void UpdateValues()
    {

        foreach(PlayerScript player in players)
        {
            if(changeMovement)
            {
                player.isSlippery = isSlippery;
                player.accelerationRate = acceleration;
                player.playerSpeed = originalSpeed + extraSpeed;
            }
            player.horizontalThrowForce = originalHorizontalThrowForceItem + horizontalThrowForceItemExtra;
            player.verticalThrowForce = originalVerticalThrowForceItem + verticalThrowForceItemExtra;
            player.horizontalThrowForcePlayer = originalHorizontalThrowForcePlayer + horizontalThrowForcePlayerExtra;
            player.verticalThrowForcePlayer = originalVerticalThrowForcePlayer + verticalThrowForcePlayerExtra;
        }         
    }

    [ContextMenu("Update Values")]
    private void UpdateValues(PlayerScript player)
    {
        if (changeMovement)
        {
            player.isSlippery = isSlippery;
            player.accelerationRate = acceleration;
            player.playerSpeed = originalSpeed + extraSpeed;
        }
        player.horizontalThrowForce = originalHorizontalThrowForceItem + horizontalThrowForceItemExtra;
        player.verticalThrowForce = originalVerticalThrowForceItem + verticalThrowForceItemExtra;
        player.horizontalThrowForcePlayer = originalHorizontalThrowForcePlayer + horizontalThrowForcePlayerExtra;
        player.verticalThrowForcePlayer = originalVerticalThrowForcePlayer + verticalThrowForcePlayerExtra;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            UpdateValues(other.gameObject.GetComponent<PlayerScript>());
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hasExitValues)
            {
                other.gameObject.GetComponent<PlayerScript>().isSlippery = replacePlayerValues.isSlippery;
                other.gameObject.GetComponent<PlayerScript>().accelerationRate = replacePlayerValues.acceleration;
                other.gameObject.GetComponent<PlayerScript>().playerSpeed = replacePlayerValues.originalSpeed + replacePlayerValues.extraSpeed;
            } else
            {
                other.gameObject.GetComponent<PlayerScript>().isSlippery = originalIsSlippery;
                other.gameObject.GetComponent<PlayerScript>().accelerationRate = originalAcceleration;
                other.gameObject.GetComponent<PlayerScript>().playerSpeed = originalSpeed;
            }
        }
        
    }
    private void OnTriggerStay(Collider other)
    {
        timer += Time.deltaTime;
        if(timer > 0.15f)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                UpdateValues(other.gameObject.GetComponent<PlayerScript>());
            }
            timer = 0;

        }

    }
}        
