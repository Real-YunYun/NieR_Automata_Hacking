using System.Collections.Generic;
using UnityEngine;
using Items.Threads;
using Items.Threads.EventBased.Proc;
using Projectiles;

public class TrackingMissle : Proc
{
    [Header("Proc Parameters")] 
    protected new float ChanceToProc = 1f;
    private List<Projectile> HandlingProjectiles = new();

    protected override void Awake()
    {
        Stats.Name = "";
        Stats.Description = "DEBUG ONLY";
        Stats.Sprite = "Player/UI Images/None";
        Stats.Duration = 0f;
        Stats.Cooldown = 0f;
        Stats.Upkeep = 0f;

        Type = EThreadBehaviour.EventBased;
    }
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        Owner.OnFireEnded += HandleProjectile;
    }

    private void HandleProjectile(Projectile SpawnedProjectile)
    {
        HandlingProjectiles.Add(SpawnedProjectile);
        HandlingProjectiles[^1].OnProjectileHit += NewTryProc;
    }

    protected override void TryProc() { }
    protected override void OnProc() { }

    private void NewTryProc(Entity HitEntity)
    {
        if (Random.value <= ChanceToProc) NewOnProc(HitEntity);
    }

    private void NewOnProc(Entity HitEntity)
    {
        Execute_OnThreadStarted();
        Vector3 RandomDirection = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y);
        HomingProjectile ProcedProjectile = 
            Instantiate(
                Resources.Load<GameObject>("Projectiles/Homing Projectile"), 
                Owner.gameObject.transform.position + RandomDirection * 1.5f, 
                Quaternion.identity
            ).GetComponent<HomingProjectile>();
        ProcedProjectile.InitResult(Owner);
        ProcedProjectile.ChangeDirection(RandomDirection);
        ProcedProjectile.SetTargetTransform(HitEntity.transform);
    }

    private void OnDisable()
    {
        Owner.OnFireEnded += HandleProjectile;
    }
}
