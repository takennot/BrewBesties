using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStateMashineHandle;

public class PlayerDie : MonoBehaviour
{
    [SerializeField] GameObject[] deathEffects;
    bool playOnce = true;
    PlayerScript playersScript;

    // Start is called before the first frame update

    private void Update()
    {
        if (playersScript.GetPlayerState() == PlayerState.Dead)
        {
            playOnce = true;
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerScript>() == true)
        {
            playersScript = other.gameObject.GetComponent<PlayerScript>();
            //Debug.Log("hittade spelaren");
            GameObject effektToPlay;
            switch (playersScript.GetPlayerTypeString())
            {
                case ("p1"):
                    effektToPlay = deathEffects[0];
                    break;
                case ("p2"):
                    effektToPlay = deathEffects[1];
                    break;
                case ("p3"):
                    effektToPlay = deathEffects[2];
                    break;
                case ("p4"):
                    effektToPlay = deathEffects[3];
                    break;
                default:
                    effektToPlay = deathEffects[0];
                    break;

            }

            if (playOnce)
            {
                GameObject gb = Instantiate(effektToPlay);
                gb.transform.position = other.gameObject.transform.position;
                Destroy(gb, 5);
                playOnce = false;
            }

        }
    }

    /*
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerScript>() == true)
        {
            playOnce = false;
        }
    }
    */
}
