using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamager : MonoBehaviour
{
    public ExplosionParticleThrower thrower;
    public float damage_radius;
    public float min_damage;
    public float max_damage;
    public float explosion_force;
    private bool triggered;

    // Start is called before the first frame update
    void Start()
    {
        triggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!triggered && thrower.state == ExplosionParticles.SubState.playing)
        {
            float to_inflict = Random.Range(min_damage, max_damage);
            Collider[] hits = Physics.OverlapSphere(
                transform.position, damage_radius);
            var helths = new HashSet<HealthManager>();
            HashSet<Collider> affected_colliders = new HashSet<Collider>();
            foreach (var hit in hits)
            {
                var other = hit.gameObject;
                var other_helth = HealthManager.find_helth(other);
                if (other == this.gameObject) continue;
                if (other_helth == null) continue;
                helths.Add(other_helth);
                affected_colliders.Add(hit);
            }

            foreach (var other_helth in helths)
            {
                var other = other_helth.gameObject;
                float distance_to_hit_r2 = (
                    transform.position - other.transform.position).sqrMagnitude;
                float damage = to_inflict / distance_to_hit_r2;
                other_helth.inflict_damage(to_inflict, DamageTypes.explosive);
            }

            foreach (var hit in affected_colliders)
            {
                Rigidbody other_rb = hit.attachedRigidbody;
                other_rb.AddExplosionForce(explosion_force, transform.position,
                                           damage_radius);
            }

            // Throw out that damage
            triggered = true;
        }
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, damage_radius);
    }
}
