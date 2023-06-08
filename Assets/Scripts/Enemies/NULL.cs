using System.Collections;
using System.Collections.Generic;
using Entities.Projectiles;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Enemies {
    [RequireComponent(typeof(NavMeshAgent), typeof(BoxCollider))]
    public class NULL : Enemy {
        [Header("NULL Parameters")] private NavMeshAgent Agent;
        private Rigidbody Rigidbody;
        private Transform Player;

        // Start is called before the first frame update
        protected void Start() {
            Agent = GetComponent<NavMeshAgent>();
            Rigidbody = GetComponent<Rigidbody>();
            if (GameManager.Instance.PlayerControllerInstance.Character != null)
                Player = GameManager.Instance.PlayerControllerInstance.Character.transform;
            Agent.SetDestination(Player.position);
        }

        // Update is called once per frame
        void Update() {
            if (Agent.enabled) {
                Agent.SetDestination(Player.position);
                if (Agent.remainingDistance < 3f) {
                    Rigidbody.velocity = Agent.velocity;
                    Agent.enabled = false;
                    transform.Rotate(transform.forward);
                }
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                HitResult Result;
                other.gameObject.GetComponent<HealthComponent>().TakeDamage(1.0f, this, out Result);
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Indestructible")) Destroy(gameObject);
        }
    }
}
