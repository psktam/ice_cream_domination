using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    public Collider building_collider;
    public ParticleSystem explode_particles;
    public float max_health;

    public float health
    {
        get {
            return _health;
        }
    }

    private float _health;

    void OnCollisionEnter(Collision other)
    {
        // 50 points of damage incurred by a 1000kg mass moving at 15 m/s, which 
        // is roughly 30 mph.
        float kinetic_energy = other.rigidbody.velocity.sqrMagnitude 
                               * other.rigidbody.mass * 0.5f;
        float damage_ratio = 50.0f / 112500.0f;

        _health -= kinetic_energy * damage_ratio;
    }

    void OnCollisionStay(Collision other)
    {
        OnCollisionEnter(other);
    }

    // Start is called before the first frame update
    void Start()
    {
        _health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (health <= 0.0f){
            killme();
        }
    }

    void killme()
    {
        explode_particles.Play();
        Destroy(gameObject, explode_particles.main.duration);
    }
}
