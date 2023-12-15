using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongReplace : MonoBehaviour
{
    //THIS SCRIPT SHOULD PROBABLY NOT BE USED IN FINAL VERSION OF GAME
    private AudioController audioController;
    public AudioClip song;
    private AudioSource source;

    [Range(0, 1)]
    public float volume = 0.3f;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(DelayStart());
    }
    
    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(0.2f);
        audioController = FindObjectOfType<AudioController>();
        if (audioController == null)
        {
            GameObject newAudioController = new GameObject("AudioController");
            audioController = newAudioController.AddComponent<AudioController>();
            source = newAudioController.AddComponent<AudioSource>();
            audioController.song_source = source;
        }

        audioController.song = song;
        audioController.song_source.volume = volume;

        yield return new WaitForSeconds(0.2f);
        audioController.PlaySong();
    }
}
