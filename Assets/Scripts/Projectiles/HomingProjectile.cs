using System;
using System.Collections;
using UnityEngine;
using Projectiles;

public class HomingProjectile : Projectile
{
    [Header("Homing Projectile Variables")]
    [SerializeField] private float TrackingRadius = 5f;
    [SerializeField] private float TurningForce = 100f;
    [SerializeField] private float Delay = 1f;
    private const float TurningPity = 5f;
    private const float MaxVelocity = 300f;
    private bool TargetAcquired = false;
    
    private Transform TargetTransform;
    public void SetTargetTransform(Transform Target)
    {
        TargetAcquired = true;
        TargetTransform = Target;
    }

    protected override void OnEnable()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/M_Enemy_Projectile1");
    }

    void FixedUpdate()
    {
        if (!TargetAcquired) Track();
        if (TargetAcquired && TargetTransform) EvaluateRadialForce();
        if (TargetAcquired && !TargetTransform) Destroy(gameObject);
    }

    private void EvaluateRadialForce()
    {
        TurningForce += TurningPity;
        Vector3 RadialDirection = (TargetTransform.position - transform.position).normalized;
        Rigidbody.AddForce(Time.deltaTime * TurningForce * RadialDirection, ForceMode.VelocityChange);
        Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, MaxVelocity);
        transform.rotation = Quaternion.LookRotation(Rigidbody.velocity);
    }

    private void Track()
    {
        Collider[] Colliders = new Collider[12];
        var size = Physics.OverlapSphereNonAlloc(transform.position, TrackingRadius, Colliders);

        for (int i = 0; i < size; i++)
        {
            if (gameObject.GetComponent<Collider>() != Colliders[i] 
                && Colliders[i].gameObject.GetComponent<Entity>() 
                && Colliders[i].gameObject.GetComponent<Entity>() != Result.Instigator
                && !Colliders[i].gameObject.CompareTag("Indestructible"))
            {
                TargetAcquired = true;
                TargetTransform = Colliders[i].gameObject.transform;
                StartCoroutine(DestroyDelay());
            }
        }
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(Delay);
        Destroy(gameObject);
    }
}
