using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tack this on with a SpreadingFire to inflict environmental damage in a 
// certain area.

public class FireDamager : MonoBehaviour
{
    public SpreadingFire fire;
    public float damage_radius;

    // Damage inflicted per cycle will be randomly chosen between the two values
    public float max_damage_rate;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (fire.get_state() == SpreadingFireState.SubState.burning)
        {
            float helth_ratio = fire.helth.get_health() / fire.helth.max_health;
            float ratio = (fire.trigger_ratio - helth_ratio) / 
                          fire.trigger_ratio;

            Collider[] hits = Physics.OverlapSphere(
                transform.position, damage_radius);
            var helths = new HashSet<HealthManager>();
            foreach (var hit in hits)
            {
                var other = hit.gameObject;
                var other_helth = HealthManager.find_helth(other);
                if (other_helth == null) continue;
                helths.Add(other_helth);
            }
            foreach (var other_helth in helths)
            {
                var other = other_helth.gameObject;
                float distance_to_hit_r2 = (
                    transform.position - other.transform.position).sqrMagnitude;
                float damage = max_damage_rate;
                if (distance_to_hit_r2 > 0.1)
                {
                    damage /= distance_to_hit_r2;
                }
                other_helth.inflict_damage(damage, DamageTypes.heat);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damage_radius);
    }
}
