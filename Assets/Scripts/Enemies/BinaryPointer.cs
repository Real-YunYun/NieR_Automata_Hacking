using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Enemies {
    public class BinaryPointer : Enemy {
        [Header("Pointer Parameters")] 
        [SerializeField] private float SpawningRate = 25f;

        [SerializeField] private GameObject Binary1;
        [SerializeField] private GameObject Binary2;
        private bool SpawnDelay;

        // Update is called once per frame
        private void Awake() {
            InvokeRepeating(nameof(Spawn), 0, SpawningRate);

            TryGetComponent(out HealthComponent);
            if (HealthComponent != null) HealthComponent.OnDeath += OnDeath;

        }

        private void Spawn() {
            if (!SpawnDelay) {
                var SpawnedBinary1 = Instantiate(Binary1,
                    new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                var SpawnedBinary2 = Instantiate(Binary2,
                    new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                SpawnedBinary1.transform.parent = gameObject.transform;
                SpawnedBinary2.transform.parent = gameObject.transform;
                StartCoroutine("SpawningDelay");
            }
        }

        IEnumerator SpawningDelay() {
            SpawnDelay = true;
            yield return new WaitForSeconds(SpawningRate);
            SpawnDelay = false;
        }

        private void OnDeath() {
            Destroy(gameObject);
        }
    }
}
