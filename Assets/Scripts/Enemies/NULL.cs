using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(BoxCollider))]
public class NULL : Entity
{
    [Header("NULL Parameters")]
    private NavMeshAgent Agent;
    private Rigidbody Rigidbody;
    private Transform Player;

    // Start is called before the first frame update
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Rigidbody = GetComponent<Rigidbody>();
        Player = GameManager.Instance.PlayerInstance.transform;
        Agent.SetDestination(Player.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Agent.enabled)
        {
            Agent.SetDestination(Player.position);
            if (Agent.remainingDistance < 3f)
            {
                Rigidbody.velocity = Agent.velocity;
                Agent.enabled = false;
                transform.Rotate(transform.forward);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage();
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Indestructible")) Destroy(gameObject);
    }
}
