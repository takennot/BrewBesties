using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnvironment : MonoBehaviour
{
    public AudioClip[] soundEffects; // Array of sound effects
    public Vector2 delayRange; // Minimum and maximum delay between sound effects
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    public AudioSource source;

    void Start()
    {
        StartCoroutine(PlaySoundEffects());
    }

    IEnumerator PlaySoundEffects()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(delayRange.x, delayRange.y));
            PlayRandomSoundEffect();
        }
    }

    void PlayRandomSoundEffect()
    {
        if (soundEffects.Length > 0)
        {
            int randomIndex = Random.Range(0, soundEffects.Length);
            float randomPitch = Random.Range(pitchRange.x, pitchRange.y);
            source.pitch = randomPitch;
            source.PlayOneShot(soundEffects[randomIndex]);
        }
    }
}