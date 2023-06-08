using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Projectiles;
using Items.Executables;
using Items.Threads;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    private PlayerControls Controls;
    
    #region Possessed Character Components

    #region Delegates and Events

    public delegate void OnPossessedDelegate();
    public event OnPossessedDelegate OnPossessed;
    
    private void OnCharacterPossessed() {
        if (PlayerCharacter != null)
            PlayerCharacter.OnCharacterDisabled += OnCharacterDisabled;

        if (PlayerMovementComponent != null) {
            Controls.Player.Movement.started += OnMovement;
            Controls.Player.Movement.canceled += OnMovement;
        }
    }

    #endregion

    [CanBeNull] private GameObject PlayerObject;
    [CanBeNull] public GameObject Character {
        set { PlayerObject = value; if (OnPossessed != null) OnPossessed(); }
        get => PlayerObject;
    }

    [CanBeNull] public PlayerCharacter PlayerCharacter => PlayerObject.GetComponent<PlayerCharacter>();
    [CanBeNull] public Entity PlayerEntityComponent => PlayerObject.GetComponent<Entity>();
    [CanBeNull] public HealthComponent PlayerHealthComponent => PlayerObject.GetComponent<HealthComponent>();
    [CanBeNull] public ShootingComponent PlayerShootingComponent => PlayerObject.GetComponent<ShootingComponent>();
    [CanBeNull] public MovementComponent PlayerMovementComponent => PlayerObject.GetComponent<MovementComponent>();
    [CanBeNull] public ExectuableComponent PlayerExecutableComponent => PlayerObject.GetComponent<ExectuableComponent>();
    [CanBeNull] public ThreadComponent PlayerThreadComponent => PlayerObject.GetComponent<ThreadComponent>();

    #endregion

    private void Awake() {
        Controls = new PlayerControls();
        OnPossessed += OnCharacterPossessed;
    }

    public GameObject Posses(Entity Possessing) {
        Character = Possessing.gameObject;
        return Character;
    }

    private void OnEnable() {
        Controls.Enable();
    }

    private void OnMovement(InputAction.CallbackContext context) {
        if (PlayerMovementComponent == null) return;

        if (context.started) PlayerMovementComponent.Execute_OnMovementStarted();
        else if (context.canceled) PlayerMovementComponent.Execute_OnMovementEnded();
    }

    private void Update() {
        if (PlayerMovementComponent != null) {
            PlayerMovementComponent.Move(Controls.Player.Movement.ReadValue<Vector2>());
        }
        
        if (PlayerShootingComponent != null) { 
            if (Controls.Player.Fire.IsPressed()) PlayerShootingComponent.Fire();
        }
        
        if (PlayerExecutableComponent != null) {
            if (Controls.Player.Executable1.IsPressed()) PlayerExecutableComponent.TryUseExecutable(1);
            if (Controls.Player.Executable2.IsPressed()) PlayerExecutableComponent.TryUseExecutable(2);
            if (Controls.Player.Executable3.IsPressed()) PlayerExecutableComponent.TryUseExecutable(3);
            if (Controls.Player.Executable4.IsPressed()) PlayerExecutableComponent.TryUseExecutable(4);
        }
    }


    private void OnCharacterDisabled() {
        Controls.Disable();

        if (PlayerCharacter != null)
            PlayerCharacter.OnCharacterDisabled -= OnCharacterDisabled;
        
        if (PlayerMovementComponent != null) {
            Controls.Player.Movement.started -= OnMovement;
            Controls.Player.Movement.canceled -= OnMovement;
        }
    }
}
