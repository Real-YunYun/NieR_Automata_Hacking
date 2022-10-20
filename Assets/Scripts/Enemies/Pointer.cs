using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : Enemy
{
    [Header("Pointer Parameters")]
    [SerializeField] private float FireRate = 2f;
    [SerializeField] private GameObject Projectile1;
    [SerializeField] private GameObject Projectile2;
    private bool FireRateDelay;
    private Transform OrbTransform;
    private Transform[] Orbs = new Transform[4];

    void Awake()
    {
        OrbTransform = transform.Find("Cylinder/Orbs");
        for (int i = 0; i < OrbTransform.childCount; i++) Orbs[i] = OrbTransform.GetChild(i).transform;
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
            for (var i = 0; i < Orbs.Length - 1; i++)
            {
                if (Orbs[i])
                {
                    if (i == 1 || i == 3) Instantiate(Projectile1, Orbs[i].position, Orbs[i].rotation);
                    else Instantiate(Projectile2, Orbs[i].position, Orbs[i].rotation);
                }
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
