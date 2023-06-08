using System.Collections;
using System.Collections.Generic;
using Entities.Projectiles;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Enemies {
    public class Binary : Enemy {
        [Header("Biary Parameters")] 
        private NavMeshAgent Agent;
        private ShootingComponent ShootingComponent;

        // Start is called before the first frame update
        protected void Start() {
            Agent = GetComponent<NavMeshAgent>();
            ShootingComponent = GetComponent<ShootingComponent>();
        }

        // Update is called once per frame
        void Update() {
            if (Agent.enabled && GameManager.Instance.PlayerControllerInstance.Character != null) {
                Agent.SetDestination(GameManager.Instance.PlayerControllerInstance.Character.transform.position);
                if (Agent.remainingDistance < 15f) ShootingComponent.Fire();
            }
        }
    }
}
