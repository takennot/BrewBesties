using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashcan : MonoBehaviour
{
    [SerializeField] private AnimationScale animationScale;
    [SerializeField] private CounterState counterState;

    [SerializeField] private AudioClip throwAwayClip;
    [SerializeField] private AudioSource source;
    [SerializeField] private GameObject throwAwayEffekt;

    // Update is called once per frame
    void Update()
    {
        if (counterState.storedItem != null)
        {
            GameObject item = counterState.storedItem;
            counterState.storedItem = null;

            if(throwAwayEffekt != null)
            {
                Transform postion = item.transform;
                Instantiate(throwAwayEffekt, postion);
                //item.transform
            }
            animationScale.ScaleDownAndDestroy(item);

            source.PlayOneShot(throwAwayClip);
        }
    }
}
