using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpreadingFireState;


namespace SpreadingFireState
{
    public enum SubState
    {
        idle,
        triggered,
        burning,
        dying,
        dead
    }
}


public class SpreadingFire : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem fire_particles;
    public ParticleSystem smoke_particles;
    public HealthManager helth;
    public AudioClip fire_sound;

    // When health hits this percentage of max health, start creating the 
    // fire
    public float trigger_ratio;
    public float max_fire_rate;
    public float max_smoke_rate;
    public float duration_after_death;

    private ParticleSystem spawned_flames;
    private ParticleSystem spawned_smoke;
    private SubState state;
    private float radius;
    private AudioSource fire_sound_emitter;

    void Start()
    {
        spawned_flames = null;
        spawned_smoke = null;
        state = SubState.idle;
        radius = 0.0f;
    }

    public SubState get_state()
    {
        return state;
    }

    // Update is called once per frame
    void Update()
    {
        float health_ratio = helth.get_health() / helth.max_health;
        if (state == SubState.idle)
        {
            if (health_ratio <= trigger_ratio)
            {
                state = SubState.triggered;
            }
        }
        else if (state == SubState.triggered)
        {
            Bounds bounds = GetComponentInChildren<MeshRenderer>().bounds;
            radius = Mathf.Min(bounds.extents.x, bounds.extents.z);
            // Shift the center down to the "floor"
            Transform render_tform = GetComponentInChildren<Transform>();
            Vector3 flame_pos = bounds.center - 
                                render_tform.up * 0.5f * bounds.extents.y;

            spawned_flames = Instantiate<ParticleSystem>(
                fire_particles, flame_pos, 
                new Quaternion());
            spawned_smoke = Instantiate<ParticleSystem>(
                fire_particles, flame_pos + transform.up * 0.1f,
                new Quaternion());

            var flame_shape = spawned_flames.shape;
            flame_shape.radius = radius;

            var smoke_shape = spawned_smoke.shape;
            smoke_shape.radius = radius;

            fire_sound_emitter = gameObject.AddComponent<AudioSource>();
            fire_sound_emitter.clip = fire_sound;
            fire_sound_emitter.loop = true;
            fire_sound_emitter.spatialBlend = 1.0f;
            
            fire_sound_emitter.Play();
            
            state = SubState.burning;
        }
        else if (state == SubState.burning)
        {
            var flame_emit = spawned_flames.emission;
            var smoke_emit = spawned_smoke.emission;

            float new_rate = (trigger_ratio - health_ratio) / trigger_ratio;
            flame_emit.rateOverTime = new_rate * max_fire_rate;
            smoke_emit.rateOverTime = new_rate * max_smoke_rate;

            if (helth.get_health() == 0.0f)
            {
                state = SubState.dying;
            }

            
        }
        else if (state == SubState.dying)
        {
            Destroy(spawned_flames, duration_after_death);
            Destroy(spawned_smoke, duration_after_death + 1.0f);
            fire_sound_emitter.Stop();
            if ((spawned_flames == null) && (spawned_smoke == null))
            {
                state = SubState.dead;
            }
        }
    }
}
