using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class EngineSoundControl : MonoBehaviour
{
    public float inertia;
    public AudioMixer mixer;
    public string engine_sound_pitch;
    public string engine_sound_vol;
    public float min_pitch_multiplier;
    public float max_pitch_multiplier;
    public float min_vol;
    public float max_vol;
    public float idle;
    public float redline;
    float rpm;
    float rate;
    float vol_rate;

    void Start()
    {
        rpm = idle;
        rate = (max_pitch_multiplier - min_pitch_multiplier) / (redline - idle);
        vol_rate = (max_vol - min_vol) / (redline - idle);
    }

    // Update is called once per frame
    void Update()
    {
        mixer.SetFloat(engine_sound_pitch, 
                       rate * (rpm - idle) + min_pitch_multiplier);
        mixer.SetFloat(engine_sound_vol, 
                       vol_rate * (rpm - idle) + min_vol);
    }

    void FixedUpdate()
    {
        float asymp = redline;
        float fwd_input = Input.GetAxis("Vertical");
        if (fwd_input <= 0.0f)
        {
            asymp = idle;
        }

        rpm = (asymp - rpm) * Time.fixedDeltaTime / inertia + rpm;
    }
}
