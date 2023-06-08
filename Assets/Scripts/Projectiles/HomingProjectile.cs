using System;
using System.Collections;
using UnityEngine;
using Entities;
using Entities.Projectiles;

public class HomingProjectile : Projectile
{
    [Header("Homing Projectile Variables")]
    [SerializeField] private float TrackingRadius = 5f;
    [SerializeField] private float TurningForce = 200f;
    [SerializeField] private float Delay = 1f;
    private const float TurningPity = 10f;
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
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/M_Homing_Projectile");
        InvokeRepeating(nameof(TrackUpdate), 0f, 0.01f);
    }

    void TrackUpdate()
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
        Collider[] Collider = Physics.OverlapSphere(transform.position, TrackingRadius, 0b_1_0000_0000);

        if (Collider.Length > 0) {
            if (gameObject.GetComponent<Collider>() != Collider[0] 
                && Collider[0].gameObject.GetComponent<Entity>() 
                && Collider[0].gameObject.GetComponent<Entity>() != Result.Instigator
                && !Collider[0].gameObject.CompareTag("Indestructible"))
            {
                TargetAcquired = true;
                TargetTransform = Collider[0].gameObject.transform;
                Destroy(gameObject, Delay);
            }
        }
    }
}
