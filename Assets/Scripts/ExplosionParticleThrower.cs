using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExplosionParticles;


namespace ExplosionParticles
{
    public enum SubState
    {
        idle,
        triggered,
        playing,
        done
    }
}


public class ExplosionParticleThrower : MonoBehaviour
{
    public HealthManager helth;
    public GameObject explosion_particles;
    public SubState state;
    public List<AudioClip> explosion_sounds;

    private GameObject particles;

    // Start is called before the first frame update
    void Start()
    {
        state = SubState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == SubState.idle)
        {
            if (helth.get_health() == 0.0f)
            {
                state = SubState.triggered;
            }
        }
        else if (state == SubState.triggered)
        {
            particles = Instantiate(explosion_particles, transform);
            state = SubState.playing;
            AudioClip to_play = explosion_sounds[
                Random.Range(0, explosion_sounds.Count - 1)];
            AudioSource.PlayClipAtPoint(to_play, transform.position);
        }
        else if (state == SubState.playing)
        {
            if (particles == null)
            {
                state = SubState.done;
            }
            else
            {
                Destroy(particles, 
                        particles.GetComponent<ParticleSystem>().main.duration);
            }
        }
    }
}
