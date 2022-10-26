using System.Collections;
using UnityEngine;

public class HomingProjectile : Projectile
{
    [Header("Homing Porjectile Variables")]
    [SerializeField] private float TrackingRadius = 5f;
    [SerializeField] private float TurningForce = 100f;
    [SerializeField] private float Delay = 1f;
    private const float TurningPity = 5f;
    private const float MaxVelocity = 300f;
    private bool TargetAquired = false;
    private Transform TargetTransform;

    void FixedUpdate()
    {
        if (!TargetAquired) Track();
        if (TargetAquired && TargetTransform) EvaluateRadialForce();
        if (TargetAquired && !TargetTransform) Destroy(gameObject);
    }

    private void EvaluateRadialForce()
    {
        TurningForce += TurningPity;
        Vector3 RadialDirection = (TargetTransform.position - transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(Time.deltaTime * TurningForce * RadialDirection, ForceMode.VelocityChange);
        GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, MaxVelocity);
        transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
    }

    private void Track()
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position, TrackingRadius, 0b_0100_0000_0000);

        foreach (Collider collider in Colliders)
        {
            if (collider.gameObject.GetComponent<Enemy>())
            {
                TargetAquired = true;
                TargetTransform = collider.gameObject.transform;
                StartCoroutine("DestroyDelay");
            }
        }
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(Delay);
        Destroy(gameObject);
    }
}
