using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager: MonoBehaviour
{
    public HealthManager helth;
    public List<AudioClip> crash_sounds;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision other)
    {
        // 50 points of damage incurred by a 1000kg mass moving at 15 m/s, 
        // which is roughly 30 mph
        float kinetic_energy = other.rigidbody.velocity.sqrMagnitude * 
                               other.rigidbody.mass * 0.5f;
        float damage_ratio = 50.0f / 112500.0f;

        helth.inflict_damage(kinetic_energy * damage_ratio, 
                             DamageTypes.physical);

        // Create an AudioSource, play a clip, and then destroy it when it's
        // done.
        Vector3 mean_contact_point = new Vector3(0.0f, 0.0f, 0.0f);
        ContactPoint[] contacts = new ContactPoint[other.contactCount];
        other.GetContacts(contacts);
        foreach (ContactPoint contact in contacts)
        {
            mean_contact_point += contact.point;
        }
        mean_contact_point /= other.contactCount;

        // Pick a random crash sound
        AudioClip to_play = crash_sounds[
            Random.Range(0, crash_sounds.Count - 1)];
        AudioSource.PlayClipAtPoint(to_play, mean_contact_point);
    }

    void OnCollisionStay(Collision other)
    {
        // OnCollisionEnter(other);
    }

}