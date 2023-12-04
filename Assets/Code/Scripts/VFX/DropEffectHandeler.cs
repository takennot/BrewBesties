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

    [SerializeField] private Transform particlaPostion;
    [SerializeField] private float secondsToDestory = 5f;
    [SerializeField] private GameObject FinisEffect;
    [SerializeField] private GameObject unFinisEffect; 
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
        effectPlaying = Instantiate(GetEffectToPlay(ingridianse), particlaPostion);
        StartCoroutine(CountTillParticalDestruction());
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

    private IEnumerator CountTillParticalDestruction()
    {
        yield return new WaitForSeconds(secondsToDestory);
        DestroyParticalPlaying();
    }

    private void DestroyParticalPlaying()
    {
        if (effectPlaying != null)
        {
            Destroy(effectPlaying);
        }
    }

    public void playFinisEffect()
    {
        effectPlaying = Instantiate(FinisEffect, particlaPostion);
        StartCoroutine(CountTillParticalDestruction());
    }

    public void playUnfinisEffect()
    {
        effectPlaying = Instantiate(unFinisEffect, particlaPostion);
        StartCoroutine(CountTillParticalDestruction());
    }

}
