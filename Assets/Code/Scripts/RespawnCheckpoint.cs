using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCheckpoint : MonoBehaviour
{
    [SerializeField] Transform checkpointPlayer1;
    [SerializeField] Transform checkpointPlayer2;
    [SerializeField] Transform checkpointPlayer3;
    [SerializeField] Transform checkpointPlayer4;
    private KillboxManager killboxManager;
    // Start is called before the first frame update
    void Start()
    {
        killboxManager = FindAnyObjectByType<KillboxManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            switch (other.GetComponent<PlayerScript>().playerType)
            {
                case PlayerScript.PlayerType.PlayerOne:
                    killboxManager.SetSpawnpoint(checkpointPlayer1, 1);
                    break;
                case PlayerScript.PlayerType.PlayerTwo:
                    killboxManager.SetSpawnpoint(checkpointPlayer2, 2);
                    break;
                case PlayerScript.PlayerType.PlayerThree:
                    killboxManager.SetSpawnpoint(checkpointPlayer3, 3);
                    break;
                case PlayerScript.PlayerType.PlayerFour:
                    killboxManager.SetSpawnpoint(checkpointPlayer4, 4);
                    break;
                default:
                    break;
            }  
        }
    }
}
