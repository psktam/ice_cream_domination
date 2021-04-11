using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;


public class MusicController : MonoBehaviour
{
    public AudioSource emitter;
    public List<AudioClip> tracks;
    int clip_idx;
    bool playing;
    bool last_input;

    RisingEdge next_track;
    RisingEdge prev_track;

    // Start is called before the first frame update
    void Start()
    {
        playing = false;
        last_input = false;
        clip_idx = 0;
        emitter.Stop();
        next_track = new RisingEdge(false);
        prev_track = new RisingEdge(false);
    }

    void manage_tracks()
    {
        // Don't want to deal with C# negative modulo
        if (Input.GetButtonDown("Next Track"))
        {
            clip_idx += 1;
        }
        if (Input.GetButtonDown("Prev Track"))
        {
            clip_idx -= 1;
        }

        if (clip_idx < 0)
        {
            clip_idx = tracks.Count - 1;
        }
        if (clip_idx >= tracks.Count)
        {
            clip_idx = 0;
        }
        if (emitter.clip != tracks[clip_idx])
        {
            emitter.clip = tracks[clip_idx];
            if (playing) 
            {
                emitter.Play();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        manage_tracks();
        bool curr_input = Input.GetButton("Music");
        bool rising_edge = curr_input && !last_input;
        bool falling_edge = !curr_input && last_input;
        last_input = curr_input;

        if (rising_edge && !playing)
        {
            emitter.Play();
            playing = true;
        }
        else if (rising_edge && playing)
        {
            emitter.Stop();
            playing = false;
        }
    }
}
