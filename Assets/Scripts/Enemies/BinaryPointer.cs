using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryPointer : Entity
{
    [Header("Pointer Parameters")]
    [SerializeField] private float SpawningRate = 25f;
    [SerializeField] private GameObject Binary1;
    [SerializeField] private GameObject Binary2;
    private bool SpawnDelay;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Spawn();
    }
    private void Spawn()
    {
        if (!SpawnDelay)
        {
            var SpawnedBinary1 = Instantiate(Binary1, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            var SpawnedBinary2 = Instantiate(Binary2, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            SpawnedBinary1.transform.parent = gameObject.transform;
            SpawnedBinary2.transform.parent = gameObject.transform;
            StartCoroutine("SpawningDelay");
        }
    }

    IEnumerator SpawningDelay()
    {
        SpawnDelay = true;
        yield return new WaitForSeconds(SpawningRate);
        SpawnDelay = false;
    }
}
