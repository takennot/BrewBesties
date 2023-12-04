using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawingPlate : MonoBehaviour
{
    [SerializeField] private Saw saw;

    public bool isTriggered = false;
    [SerializeField] private PlayerScript playerColliding;

    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material standingOnMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerScript player = other.GetComponent<PlayerScript>();
        if (player)
        {
            isTriggered = true;
            playerColliding = player;
            meshRenderer.material = standingOnMaterial;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerScript player = other.GetComponent<PlayerScript>();
        if(player && player == playerColliding)
        {
            saw.StopSawProcess(player);
            isTriggered = false;
            playerColliding = null;
            meshRenderer.material = baseMaterial;
        }
    }

    /* // vad händer om 2 personer är på samma platta???
    private void OnTriggerStay(Collider other)
    {
        PlayerScript player = other.GetComponent<PlayerScript>();
        if (player)
        {
            isTriggered = true;
            playerColliding = player;
        }
    }
    */

    public PlayerScript GetPlayerColliding()
    {
        return playerColliding;
    }

    public Saw GetSaw()
    {
        return saw;
    }
}
