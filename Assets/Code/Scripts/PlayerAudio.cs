using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource sourceFootsteps;
    [Header("Default Pitch Variance")]
    [SerializeField] private float maxPitch = 1.175f;
    [SerializeField] private float minPitch = 0.825f;

    [Header("Pick Up")]
    [SerializeField] private AudioClip pickUpSound;
    [Range(0, 1)]
    [SerializeField] private float pickUpVolume = 0.5f;

    [Header("Drop")]
    [SerializeField] private AudioClip dropSound;
    [Range(0, 1)]
    [SerializeField] private float dropVolume = 0.5f;

    [Header("Drag")]
    [SerializeField] private AudioClip dragSound;
    [Range(0, 1)]
    [SerializeField] private float dragVolume = 0.5f;
    [SerializeField] private Vector2 dragPitchRange = new Vector2(0.9f, 1.1f);

    [Header("Grab")]
    [SerializeField] private AudioClip grabSound;
    [Range(0, 1)]
    [SerializeField] private float grabVolume = 0.5f;

    [Header("Throw")]
    [SerializeField] private AudioClip throwSound;
    [Range(0, 1)]
    [SerializeField] private float throwVolume = 0.5f;

    [Header("Footstep")]
    [SerializeField] private AudioClip footstepSound;
    [Range(0, 1)]
    [SerializeField] private float footstepVolume = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        sourceFootsteps.Pause();
        sourceFootsteps.volume = footstepVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RandomPitch()
    {
        source.pitch = Random.Range(minPitch, maxPitch);
    }

    public void RandomPitch(Vector2 randomPitch)
    {
        source.pitch = Random.Range(randomPitch.x, randomPitch.y);
    }

    public void PlayPickUp()
    {
        RandomPitch();
        source.PlayOneShot(pickUpSound, pickUpVolume);
    }

    public void PlayDrop()
    {
        RandomPitch();
        source.PlayOneShot(dropSound, dropVolume);
    }

    public void PlayDrag()
    {
        if (dragSound != null && source.isPlaying && source.clip == dragSound)
        {
            Debug.Log("Drag sound already playing");
            source.Stop();
        }
        source.clip = dragSound;
        RandomPitch(dragPitchRange);
        source.PlayOneShot(dragSound, dragVolume);
    }

    public void PlayGrab()
    {
        RandomPitch();
        source.PlayOneShot(grabSound, grabVolume);
    }

    public void PlayThrow()
    {
        RandomPitch();
        source.PlayOneShot(throwSound, throwVolume);
    }

    public void PlayFootstep(bool enabled) 
    {
        if(enabled)
        {
            sourceFootsteps.clip = footstepSound;
            sourceFootsteps.UnPause();
        }
        else
        {
            sourceFootsteps.Pause();
        }
    }


}
