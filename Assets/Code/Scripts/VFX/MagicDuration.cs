using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDuration : MonoBehaviour
{
    //[SerializeField] public float effectTime = 10; //standar 30;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] bool isfinalyEffct = false;
    [SerializeField] bool isSideEffect = false;


    void Start()
    {
        //ps = GetComponent<ParticleSystem>();
    }
    // note konsgit med main, fungerar inte med get particla

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        /*
        var main = ps.main;
        ParticleSystem.EmissionModule em = ps.emission;

        if (isfinalyEffct)
        {
            //main.startDelay = effectTime;
            //ps.emission.burst.time = effectTime;

            ParticleSystem.Burst burst = ps.emission.GetBurst(0);
            burst.time = effectTime;
            ps.emission.SetBurst(0, burst);

            //em.SetBurst(1 ,new ParticleSystem.Burst(effectTime, 25));
            //main.duration = effectTime +1f;

            return;
        }

        if (isSideEffect)
        {
            main.duration = (effectTime - 0.5f);
            return;
        }

        //main.duration = effectTime;
        main.startLifetime = effectTime;


        //main.startSpeed = (em.rateOverTime.constant / 100f) < 0.1f ? 0.1f : (em.rateOverTime.constant / 100f)
        */
    }

    public void setMagicVariabels(float effectTime)
    {

        //var main = ps.main;
        ParticleSystem.MainModule main = ps.main;
        //ParticleSystem.EmissionModule em = ps.emission;

        if (isfinalyEffct)
        {
            //main.startDelay = effectTime;
            //ps.emission.burst.time = effectTime;

            ParticleSystem.Burst burst = ps.emission.GetBurst(0);
            burst.time = effectTime;
            ps.emission.SetBurst(0, burst);

            //em.SetBurst(1 ,new ParticleSystem.Burst(effectTime, 25));
            //main.duration = effectTime +1f;

            return;
        }

        if (isSideEffect)
        {
            //float newTime = (effectTime - 0.5f);
            //main.duration = newTime;
            return;
        }

        //Debug.Log("Main är " + main);
        //Debug.Log("startLifetime " + main.startLifetime);


        main.startLifetime = effectTime;
        //ps.main = main;
        //ParticleSystem.MainModule.startLifetime s = ps.main.startLifetime;
        //main.duration = effectTime;

        //main.startLifetime.constant = effectTime;
        //var startlifetime = main.startLifetime;
        //startlifetime = effectTime;
        //main.startLifetime = startlifetime;

        //var main = ps.main;
        //ps.startLifetime = effectTime;
        //main.startLifetime = effectTime;

     


    }
}
