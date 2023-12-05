using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class KillboxManager : MonoBehaviour
{
    //[SerializeField] public GameObject player;
    // spawnpoints
    [SerializeField] private Transform spawnpoint1;
    [SerializeField] private Transform spawnpoint2;
    [SerializeField] private Transform spawnpoint3;
    [SerializeField] private Transform spawnpoint4;
    [SerializeField] private float cooldown = 3;

    // player images
    [SerializeField] private Image imagePlayer1;
    [SerializeField] private Image imagePlayer2;
    [SerializeField] private Image imagePlayer3;
    [SerializeField] private Image imagePlayer4;

    // text timer
    [SerializeField] private TMP_Text timerPlayer1;
    [SerializeField] private TMP_Text timerPlayer2;
    [SerializeField] private TMP_Text timerPlayer3;
    [SerializeField] private TMP_Text timerPlayer4;

    // player death timers
    private float count1;
    private float count2;
    private float count3;
    private float count4;

    // Is Player Dead booleans
    private bool isPlayer1Dead;
    private bool isPlayer2Dead;
    private bool isPlayer3Dead;
    private bool isPlayer4Dead;

    // respawn VFX
    [SerializeField] public GameObject respawnVFX;
    [HideInInspector] public GameObject respawnVFXInstance;

    // Start is called before the first frame update
    void Start()
    {
        imagePlayer1.enabled = false;
        timerPlayer1.enabled = false;

        imagePlayer2.enabled = false;
        timerPlayer2.enabled = false;

        imagePlayer3.enabled = false;
        timerPlayer3.enabled = false;

        imagePlayer4.enabled = false;
        timerPlayer4.enabled = false;

        count1 = cooldown;
        count2 = cooldown;
        count3 = cooldown;
        count4 = cooldown;

        isPlayer1Dead = false;
        isPlayer2Dead = false;
        isPlayer3Dead = false;
        isPlayer4Dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayer1Dead)
        {
            count1 -= Time.deltaTime;
            timerPlayer1.SetText((Mathf.FloorToInt(count1 % 60) + 1).ToString());
        }
        if (isPlayer2Dead)
        {
            count2 -= Time.deltaTime;
            timerPlayer2.SetText((Mathf.FloorToInt(count2 % 60) + 1).ToString());
        }
        if(isPlayer3Dead)
        {
            count3 -= Time.deltaTime;
            timerPlayer3.SetText((Mathf.FloorToInt(count3 % 60) + 1).ToString());
        }
        if (isPlayer4Dead)
        {
            count4 -= Time.deltaTime;
            timerPlayer4.SetText((Mathf.FloorToInt(count4 % 60) + 1).ToString());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Killbox triggered");
        if (other.GetComponent<PlayerScript>())
        {
            Debug.Log("Killbox triggered player");
            if (other.gameObject.GetComponent<PlayerScript>().GetObjectInHands() != null)
            {
                Debug.Log("Killbox found something in player hands");
                if (other.gameObject.GetComponent<PlayerScript>().GetObjectInHands().GetComponent<PlayerScript>())
                {
                    Debug.Log("Killbox found a player in player hands");
                    other.gameObject.GetComponent<PlayerScript>().DropPlayer(false);
                    if (other.gameObject.GetComponent<PlayerScript>().GetHoldingState() != PlayerStateMashineHandle.HoldingState.HoldingNothing)
                    {
                        other.gameObject.GetComponent<PlayerScript>().Drop(false);
                        Debug.Log("Killbox dropped player in hands");
                    }
                }
                if (other.gameObject.GetComponent<PlayerScript>().GetObjectInHands().GetComponent<Item>())
                {
                    Debug.Log("Killbox found an item in hands");
                    other.gameObject.GetComponent<PlayerScript>().Drop(false);
                }
            }

            other.gameObject.GetComponent<PlayerScript>().Die();
            StartCoroutine(RespawnWithCooldown(other));
            Debug.Log("Killbox respawning player");
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

    IEnumerator RespawnWithCooldown(Collider other)
    {
        switch (other.gameObject.GetComponent<PlayerScript>().playerType)
        {
            case PlayerScript.PlayerType.PlayerOne:
                Debug.Log("Killbox found player1");
                imagePlayer1.enabled = true;
                timerPlayer1.enabled = true;
                isPlayer1Dead = true;
                yield return new WaitForSecondsRealtime(cooldown);
                other.gameObject.GetComponent<PlayerScript>().Respawn(spawnpoint1);
                respawnVFXInstance = Instantiate(respawnVFX, spawnpoint1);
                Destroy(respawnVFXInstance, 1);
                imagePlayer1.enabled = false;
                timerPlayer1.enabled = false;
                isPlayer1Dead = false;
                count1 = cooldown;
                break;
            case PlayerScript.PlayerType.PlayerTwo:
                Debug.Log("Killbox found player2");
                imagePlayer2.enabled = true;
                timerPlayer2.enabled = true;
                isPlayer2Dead = true;
                yield return new WaitForSecondsRealtime(cooldown);
                other.gameObject.GetComponent<PlayerScript>().Respawn(spawnpoint2);
                respawnVFXInstance = Instantiate(respawnVFX, spawnpoint2);
                Destroy(respawnVFXInstance, 1);
                imagePlayer2.enabled = false;
                timerPlayer2.enabled = false;
                isPlayer2Dead = false;
                count2 = cooldown;
                break;
            case PlayerScript.PlayerType.PlayerThree:
                Debug.Log("Killbox found player3");
                imagePlayer3.enabled = true;
                timerPlayer3.enabled = true;
                isPlayer3Dead = true;
                yield return new WaitForSecondsRealtime(cooldown);
                other.gameObject.GetComponent<PlayerScript>().Respawn(spawnpoint3);
                respawnVFXInstance = Instantiate(respawnVFX, spawnpoint3);
                Destroy(respawnVFXInstance, 1);
                imagePlayer3.enabled = false;
                timerPlayer3.enabled = false;
                isPlayer3Dead = false;
                count3 = cooldown;
                break;
            case PlayerScript.PlayerType.PlayerFour:
                Debug.Log("Killbox found player4");
                imagePlayer4.enabled = true;
                timerPlayer4.enabled = true;
                isPlayer4Dead = true;
                yield return new WaitForSecondsRealtime(cooldown);
                other.gameObject.GetComponent<PlayerScript>().Respawn(spawnpoint4);
                respawnVFXInstance = Instantiate(respawnVFX, spawnpoint4);
                Destroy(respawnVFXInstance, 1);
                imagePlayer4.enabled = false;
                timerPlayer4.enabled = false;
                isPlayer4Dead = false;
                count4 = cooldown;
                break;
        }
    }


    /// <summary>
    /// Returns a spawnpoint that is based on id. Possible values are only 1, 2, 3 or 4.
    /// </summary>
    /// <param name="id">Spawnpoint to return</param>
    /// <returns>Transform object</returns>
    public Transform GetSpawnpoint(int id)
    {
        switch (id)
        {
            case 1:
                return spawnpoint1;
            case 2:
                return spawnpoint2;
            case 3:
                return spawnpoint3;
            case 4:
                return spawnpoint4;
            default: return null;
        }
    }

    /// <summary>
    /// Sets spawnpoint to respawn player to
    /// </summary>
    /// <param name="spawnpoint">A spawnpoint to respawn player into</param>
    /// <param name="id">Player ID to set spawnpoint for. Can only be 1, 2, 3 or 4</param>
    public void SetSpawnpoint(Transform spawnpoint, int id)
    {
        switch (id)
        {
            case 1:
                spawnpoint1 = spawnpoint;
                break;
            case 2:
                spawnpoint2 = spawnpoint;
                break;
            case 3:
                spawnpoint3 = spawnpoint;
                break;
            case 4:
                spawnpoint4 = spawnpoint;
                break;
            default:
                break;
        }
    }
}
