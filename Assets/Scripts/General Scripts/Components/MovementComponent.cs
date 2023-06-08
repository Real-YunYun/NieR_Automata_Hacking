using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine;

namespace Entities {
    public enum MovementController {
        PlayerController = 0,
        EnemyController = 1
    }
    
    public class MovementComponent : MonoBehaviour {

        [Header("Movement Parameters")] 
        public MovementController MovementController;
        public float MoveSpeed = 10f;

        [Header("Player Controller Parameters")]
        private float TurnSmoothVelocity;
        private readonly float Gravity = -9.8f;
        [HideInInspector] public bool GravityOn = true;
        private Vector3 Velocity = Vector3.zero;

        
        #region Delegates and Events

        public delegate void OnMovementStartedDelegate();

        public delegate void OnMovementEndedDelegate();

        public event OnMovementStartedDelegate OnMovementStarted;
        public event OnMovementEndedDelegate OnMovementEnded;

        public void Execute_OnMovementStarted() {
            if (OnMovementStarted != null) OnMovementStarted();
        }

        public void Execute_OnMovementEnded() {
            if (OnMovementEnded != null) OnMovementEnded();
        }

        #endregion

        private void Update() {

            if (MovementController == MovementController.PlayerController) {
                CharacterController Controller;
                if (!TryGetComponent(out Controller)) return;
                if (GravityOn) Controller.Move(new Vector3(0, Gravity * Time.deltaTime, 0));
            }
        }

        // For AI to traverse
        public virtual void Move(Vector3 TargetPosition) {
            NavMeshAgent Agent;
            if (!TryGetComponent(out Agent)) return;
            if (Agent.enabled && GameManager.Instance.PlayerControllerInstance.PlayerCharacter != null) 
                Agent.SetDestination(GameManager.Instance.PlayerControllerInstance.PlayerCharacter.transform.position);
        }

        // For Player Controllability
        public virtual void Move(Vector2 Direction2D) {
            Vector3 Direction3D = new Vector3(Direction2D.x, 0, Direction2D.y);
            Vector3 MoveDir;

            CharacterController Controller;
            if (!TryGetComponent(out Controller)) return;

            //Turning the Character Model relative to the Movement Direction
            if (Direction3D.magnitude >= 0.1f) {
                float TurningAngle = Mathf.Atan2(Direction3D.x, Direction3D.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                float ResultAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TurningAngle, ref TurnSmoothVelocity, 0.1f);
                transform.rotation = Quaternion.Euler(0f, ResultAngle, 0f);
                MoveDir = Quaternion.Euler(0f, TurningAngle, 0f) * Vector3.forward;

                Controller.Move(MoveDir * (MoveSpeed * Time.deltaTime));
            }
        }
    }
}
