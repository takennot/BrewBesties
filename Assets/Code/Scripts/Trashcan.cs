using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashcan : MonoBehaviour
{
    [SerializeField] private AnimationScale animationScale;
    [SerializeField] private CounterState counterState;

    [SerializeField] private AudioClip throwAwayClip;
    [SerializeField] private AudioSource source;

    // Update is called once per frame
    void Update()
    {
        if (counterState.storedItem != null)
        {
            GameObject item = counterState.storedItem;
            counterState.storedItem = null;

            animationScale.ScaleDownAndDestroy(item);

            source.PlayOneShot(throwAwayClip);
        }
    }
}
