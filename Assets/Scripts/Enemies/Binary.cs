using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Binary : Enemy
{
    [Header("Biary Parameters")]
    private NavMeshAgent Agent;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        Agent = GetComponent<NavMeshAgent>();
        ProjectileSpawn = transform.Find("Projectile Spawn");
    }

    // Update is called once per frame
    void Update()
    {
        if (Agent.enabled)
        {
            Agent.SetDestination(GameManager.Instance.PlayerInstance.transform.position);
            if (Agent.remainingDistance < 15f) Fire();
        }
    }

    public override void Death()
    {
        if (transform.parent)
        {
            Instantiate(base.DeathParticle, gameObject.transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else base.Death();
    }
}
