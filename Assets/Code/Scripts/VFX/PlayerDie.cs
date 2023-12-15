using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    [SerializeField] GameObject[] deathEffects;
    bool playOnce = true;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerScript>() == true)
        {
            Debug.Log("hittade spelaren");
            GameObject effektToPlay;
            switch (other.gameObject.GetComponent<PlayerScript>().GetPlayerTypeString())
            {
                case("p1"):
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
            }
           
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerScript>() == true)
        {
            playOnce = false;
        }
    }
}
