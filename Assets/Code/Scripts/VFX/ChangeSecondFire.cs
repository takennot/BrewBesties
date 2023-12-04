using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSecondFire : MonoBehaviour
{
    [SerializeField] ParticleSystem mainFire;
    // Start is called before the first frame update
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        var em = ps.emission;
        em.enabled = true;
        var emMain = mainFire.emission;
        float emisson = emMain.rateOverTime.constant/ 3f;

        em.rateOverTime = emisson;

       
    }

}
