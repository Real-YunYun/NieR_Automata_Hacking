using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Entities.Projectiles;
using Items;
using Items.Executables;
using Items.Threads;
using UnityEngine.InputSystem.Users;

namespace Entities {
    [DefaultExecutionOrder(2)]
    public class PlayerCharacter : Entity {
        
        //This GameObject Components
        private GameObject PlayerBody;
        private GameObject Minimap;

        #region Contorller Params
        
        private PlayerControls ControlScheme;

        public Vector2 MovementDirection {
            get {
                Vector2 Direction = ControlScheme.Player.Movement.ReadValue<Vector2>();
                return new Vector3(Direction.x, 0, Direction.y).normalized;
            }
        }
        
        public delegate void OnCharacterDisabledDelegate();
        public event OnCharacterDisabledDelegate OnCharacterDisabled;

        #endregion

        void Awake() {
            ControlScheme = new PlayerControls();
            PlayerBody = GameObject.Find("Player Mesh");
            Minimap = Instantiate(Resources.Load<GameObject>("Player/MiniMapCamera"));

            TryGetComponent(out HealthComponent);
            TryGetComponent(out ExectuableComponent);
            TryGetComponent(out ThreadComponent);
            TryGetComponent(out MovementComponent);
        }

        private void OnEnable() {
            ControlScheme.Enable();
            
            if (HealthComponent == null) return;
            HealthComponent.OnTakeDamage += OnTakeDamage;
            HealthComponent.OnDeath += OnDeath;
        }

        private void Start() {
            //Manually Adding Abilities
            if (ExectuableComponent) {
                ExectuableComponent.AddExecutable<Nodes>(1);
                ExectuableComponent.AddExecutable<Firewall>(2);
                ExectuableComponent.AddExecutable<Teleport>(3);
                ExectuableComponent.AddExecutable<Overclock>(4);
            }

            if (ThreadComponent) {
                //AddThread<HomingThread>();
                ThreadComponent.AddThread<TrackingMissile>();
            }
        }

        // Update is called once per frame
        void Update() {
            if (!GameManager.Instance.IsGamePaused) {

                #region Player Input Devices

                Keyboard keyboard = Keyboard.current;
                Mouse mouse = Mouse.current;
                Gamepad gamepad = Gamepad.current;

                #endregion

                #region Mouse Input
                
                Ray ray = Camera.main.ScreenPointToRay(ControlScheme.Player.Look.ReadValue<Vector2>());
                if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, 0b_1000_0000)) {
                    Vector3 rotation = Quaternion.LookRotation(rayHit.point - transform.position, Vector3.up)
                        .eulerAngles;
                    rotation.x = rotation.z = 0;
                    PlayerBody.transform.rotation = Quaternion.Euler(rotation);
                    
                    ShootingComponent ShootingComponent;
                    if (TryGetComponent(out ShootingComponent)) {
                        /* Debug Aim Line */
                        Vector3 ProjectilePosition = ShootingComponent.GetProjectileSpawn().transform.position;
                        Debug.DrawLine(rayHit.point, rayHit.point + (Vector3.up * 5f), Color.red);
                        Debug.DrawLine(ProjectilePosition,ProjectilePosition + (ShootingComponent.GetProjectileSpawn().transform.forward * 1000f),
                            Color.green);
                    }
                }

                #endregion
            }
        }

        private void OnDisable() { 
            ControlScheme.Disable();

            if (OnCharacterDisabled != null)
                OnCharacterDisabled();
            
            if (HealthComponent == null) return;
            HealthComponent.OnTakeDamage -= OnTakeDamage;
            HealthComponent.OnDeath -= OnDeath;
        }

        private void OnTakeDamage() {
            
        }

        private void OnDeath() {
            GameManager.Instance.SaveGame();
            GameManager.Instance.LoadGame();
        }
    }
}
