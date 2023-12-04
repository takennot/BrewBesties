using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVFXParameter : MonoBehaviour
{
    [SerializeField] public float particlasEmmison = 30; //standar 30;
    private ParticleSystem ps;
    float size;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        /*
        ParticleSystem ps = GetComponent<ParticleSystem>();
        em = ps.emission;
        em.enabled = true;

        em.rateOverTime = 20.0f;
        */

        /* em.SetBursts(
             new ParticleSystem.Burst[]{
                 new ParticleSystem.Burst(2.0f, 100),
                 new ParticleSystem.Burst(4.0f, 100)
             });
        */
        var main = ps.main;
        size = main.startSizeMultiplier;
    }

// Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        var em = ps.emission;
        em.enabled = true;

        em.rateOverTime = (particlasEmmison/2f);

        var main = ps.main;
        main.gravityModifier = em.rateOverTime.constant > 10f ? -0.02f : -0.025f;
        main.startSizeMultiplier = em.rateOverTime.constant > 15f ? ((particlasEmmison/2)/10) : size;
        

        //main.startSpeed = (em.rateOverTime.constant / 100f) < 0.1f ? 0.1f : (em.rateOverTime.constant / 100f)
    }

}
