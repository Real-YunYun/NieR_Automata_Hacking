using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NULLPointer : Entity
{
    [Header("Pointer Parameters")]
    [SerializeField] private float FireRate = 0.1f;
    [SerializeField] private GameObject NULL;
    private Transform Player;
    private bool FireRateDelay;
    private Transform Rotator;
    private Transform SpawnTransform;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.Instance.PlayerInstance.transform;
        Rotator = transform.Find("Spawner");
        SpawnTransform = transform.Find("Spawner/NULL Spawn Point");
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
