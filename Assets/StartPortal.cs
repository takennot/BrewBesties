using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPortal : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] ParticleSystem[] gameobejctToremove;
    [SerializeField] float time = 1;

    void Start()
    {
        StartCoroutine(StopLopping(time));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StopLopping(float time)
    {
        yield return new WaitForSeconds(time);
        ParticleSystem.MainModule main1 = gameobejctToremove[0].main;
        ParticleSystem.MainModule main2 = gameobejctToremove[1].main;
        ParticleSystem.MainModule main3 = gameobejctToremove[2].main;

        main1.loop = false;
        main2.loop = false;
        main3.loop = false; 
    }

   
}
