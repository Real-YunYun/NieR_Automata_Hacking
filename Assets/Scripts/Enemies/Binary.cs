using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Binary : Entity
{
    [Header("Biary Parameters")]
    [SerializeField] private GameObject ProjectilePrefab;
    private Transform ProjectileSpawn;
    private bool FireRateDelay = false;
    private NavMeshAgent Agent;

    // Start is called before the first frame update
    void Start()
    {
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
            Instantiate(base.DeathParticle, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
        }
        else base.Death();
    }

    private void Fire() { if (!FireRateDelay) StartCoroutine("Delay"); }

    IEnumerator Delay()
    {
        FireRateDelay = true;
        Instantiate(ProjectilePrefab, ProjectileSpawn.position, ProjectileSpawn.rotation);
        yield return new WaitForSeconds(1f);
        FireRateDelay = false;
    }
}
