using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class HumanController : MonoBehaviour
{
    public Transform player_tform;
    NavMeshAgent agent;
    Animator animator;

    public bool is_moving()
    {
        return agent.velocity.magnitude > 0.025f;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = player_tform.position;
        animator.SetBool("walking", is_moving());
    }
}
