using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : Enemy
{
    [Header("Pointer Parameters")]
    [SerializeField] private new float FireRate = 2f;
    [SerializeField] private GameObject Projectile1;
    [SerializeField] private GameObject Projectile2;
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
    
    protected override void Fire()
    {
        if (!FireRateDelay)
        {
            for (var i = 0; i < Orbs.Length; i++)
            {
                if (Orbs[i])
                {
                    if (i == 0 || i == 2) Instantiate(Projectile1, Orbs[i].position, Orbs[i].rotation);
                    else Instantiate(Projectile2, Orbs[i].position, Orbs[i].rotation);
                    Execute_OnFire();
                }
            }
            StartCoroutine(ShootingDelay());
        }
    }
}
