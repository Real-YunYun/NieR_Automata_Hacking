using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NULLPointer : Enemy
{
    [Header("Pointer Parameters")]
    [SerializeField] private float FireRate = 0.1f;
    private GameObject NULL;
    private Transform Player;
    private bool FireRateDelay;
    private Transform Rotator;
    private Transform SpawnTransform;

    void Awake()
    {
        NULL = Resources.Load<GameObject>("Enemies/NULL");
        Rotator = transform.Find("Rotator");
        SpawnTransform = transform.Find("Rotator/NULL Spawn Point");
    }

    void Start()
    {
        Player = GameManager.Instance.PlayerInstance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Rotator.rotation = Quaternion.LookRotation(Player.position, Rotator.forward);
        Fire();
    }

    private void Fire()
    {
        if (!FireRateDelay)
        {
            Instantiate(NULL, SpawnTransform.position, SpawnTransform.rotation);
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
