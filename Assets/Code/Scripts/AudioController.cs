using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip song;
    public AudioClip songStress4min;

    public AudioSource song_source;
    
    public bool playSong;

    // Start is called before the first frame update
    void Start()
    {
        song_source = GetComponent<AudioSource>();
        song_source.clip = song;
        song_source.loop = true;

        if (playSong == true)
        {
            song_source.Stop();
            song_source.Play();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("PlaySong")]
    public void PlaySong()
    {
        song_source.Stop();
        song_source.Play();
    }
}
