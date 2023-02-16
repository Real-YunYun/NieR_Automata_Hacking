using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NULLPointer : Enemy
{
    [Header("Pointer Parameters")]
    private GameObject NULL;
    private Transform Player;
    private Transform Rotator;
    private Transform SpawnTransform;

    void Awake()
    {
        NULL = Resources.Load<GameObject>("Enemies/NULL");
        Rotator = transform.Find("Rotator");
        SpawnTransform = transform.Find("Rotator/NULL Spawn Point");
    }

    protected override void Start()
    {
        base.Start();
        Player = GameManager.Instance.PlayerInstance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Rotator.rotation = Quaternion.LookRotation(Player.position, Rotator.forward);
        Fire();
    }

    protected override void Fire()
    {
        if (!FireRateDelay)
        {
            Instantiate(NULL, SpawnTransform.position, SpawnTransform.rotation);
            StartCoroutine(ShootingDelay());
        }
    }
}
