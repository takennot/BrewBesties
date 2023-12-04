using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropEffectHandeler : MonoBehaviour
{
    private GameObject effectPlaying;
    
    [SerializeField] private GameObject monsterEyeEffect;
    [SerializeField] private GameObject mushroomEffect;
    [SerializeField] private GameObject pixiDust;

    [SerializeField] private Transform particlePostion;
    [SerializeField] private float secondsToDestory = 5f;
    [SerializeField] private GameObject finishEffect;
    [SerializeField] private GameObject unFinishEffect; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayEffect(string ingridianse)
    {
        effectPlaying = Instantiate(GetEffectToPlay(ingridianse), particlePostion);
        StartCoroutine(CountTillParticleDestruction());
    }

    private GameObject GetEffectToPlay(string ingridianse)
    {
        GameObject effect = null;
        switch (ingridianse)
        {
            case "mushroom":
                effect = mushroomEffect;
                break;
            case "monstereye":
                effect = monsterEyeEffect;
                break;
            case "pixiedust":
                effect= pixiDust;
                break;
            default:
                effect = mushroomEffect;
                break;
        }
        return effect;
    }

    private IEnumerator CountTillParticleDestruction()
    {
        yield return new WaitForSeconds(secondsToDestory);
        DestroyParticlePlaying();
    }

    private void DestroyParticlePlaying()
    {
        if (effectPlaying != null)
        {
            Destroy(effectPlaying);
        }
    }

    public void PlayFinishEffect()
    {
        effectPlaying = Instantiate(finishEffect, particlePostion);
        StartCoroutine(CountTillParticleDestruction());
    }

    public void PlayUnFinishEffect()
    {
        effectPlaying = Instantiate(unFinishEffect, particlePostion);
        StartCoroutine(CountTillParticleDestruction());
    }

}
