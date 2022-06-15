using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : Entity
{
    [Header("Pointer Parameters")]
    [SerializeField] private float FireRate = 2f;
    [SerializeField] private GameObject Projectile1;
    [SerializeField] private GameObject Projectile2;
    private bool FireRateDelay;
    private Transform OrbTransform;
    private Transform[] Orbs;

    // Start is called before the first frame update
    void Start()
    {
        OrbTransform = transform.Find("Cylinder/Orbs");
        Orbs = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        OrbTransform.Rotate(new Vector3(0, 100 * Time.deltaTime, 0), Space.Self);
        Fire();
    }
    private void Fire()
    {
        if (!FireRateDelay)
        {
            for (var i = 3; i < Orbs.Length; i++)
            {
                if (i % 2 == 0) Instantiate(Projectile1, Orbs[i].position, Orbs[i].rotation);
                else Instantiate(Projectile2, Orbs[i].position, Orbs[i].rotation);
            }
            StartCoroutine("ShootingDelay");
        }
    }

    IEnumerator ShootingDelay()
    {
        FireRateDelay = true;
        yield return new WaitForSeconds(1f / FireRate);
        FireRateDelay = false;
    }
}
