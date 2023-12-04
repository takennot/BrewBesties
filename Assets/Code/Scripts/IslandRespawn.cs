using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandRespawn : MonoBehaviour
{
    [SerializeField] Transform islandSpawnpoint;
    [SerializeField] GameObject killboxManager;
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
        if (other.GetComponent<PlayerScript>())
        {
            switch (other.GetComponent<PlayerScript>().playerType)
            {
                case PlayerScript.PlayerType.PlayerOne:
                    killboxManager.GetComponent<KillboxManager>().SetSpawnpoint(islandSpawnpoint, 1);
                    break;
                case PlayerScript.PlayerType.PlayerTwo:
                    killboxManager.GetComponent<KillboxManager>().SetSpawnpoint(islandSpawnpoint, 2);
                    break;
                case PlayerScript.PlayerType.PlayerThree:
                    killboxManager.GetComponent<KillboxManager>().SetSpawnpoint(islandSpawnpoint, 3);
                    break;
                case PlayerScript.PlayerType.PlayerFour:
                    killboxManager.GetComponent<KillboxManager>().SetSpawnpoint(islandSpawnpoint, 4);
                    break;
                default:
                    break;
            }
            
        }
    }
}
