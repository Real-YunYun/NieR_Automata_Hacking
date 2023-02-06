using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Executables;
using Threads;

public class Entity : MonoBehaviour
{
    [Header("Entity Parameters")]
    [Header("Stats")]
    [Tooltip("Dictates if this Entity is Invincible")]
    [SerializeField] protected bool Invincible = false;
    
    [Tooltip("Dictates this Entity Health")]
    [SerializeField] protected int _health = 50;
    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }
    
    [Tooltip("Dictates this Entity Energy")]
    [SerializeField] protected int _energy = 25;
    public int Energy
    {
        get { return _energy; }
        set { _energy = value; }
    }
    
    public float MoveSpeed = 10f;
    public float FireRate = 8f;
    protected bool FireRateDelay;
    
    #region Basic Unity Functions

    protected virtual void Start()
    {
        for (int i = 1; i < Executables.Length + 1; i++) AddExecutable<NoExecutable>(i);
    }
    
    #endregion

    #region Player Controller Booleans

    [Header("Controller Limitations")]
    [SerializeField] protected bool CanMove = true;
    [SerializeField] protected bool CanShoot = true;
    [SerializeField] protected bool CanExecute = true;

    public void SetCanExecute(bool state = true) { CanExecute = state; }
    public void SetCanShoot(bool state = true) { CanShoot = state; }
    public void SetMoveShoot(bool state = true) { CanMove = state; }

    #endregion

    #region Basic Mechanics
    
    #region Movement
    
    #region Delegates and Events
    
    public delegate void OnMovementStartedDelegate();
    public delegate void OnMovementTickDelegate();
    public delegate void OnMovementEndedDelegate();

    public event OnMovementStartedDelegate OnMovementStarted;
    public event OnMovementTickDelegate OnMovementTick;
    public event OnMovementEndedDelegate OnMovementEnded;

    #endregion

    protected virtual void Move() { }
    
    #endregion
    
    #region Firing
    
    [Header("Controller Children")] 
    [SerializeField] protected Transform ProjectileSpawn;
    [SerializeField] protected GameObject ProjectilePrefab;
    
    [Header("Audio Clips")] 
    [SerializeField] protected AudioSource ShootingSource;
    
    #region Delegates and Events
    
    public delegate void OnFireDelegate();
    public event OnFireDelegate OnFire;

    // Make for Each Event!
    public void Execute_OnFire() { if (OnFire != null) OnFire(); }

    #endregion

    protected virtual void Fire()
    {
        if (!FireRateDelay)
        {
            StartCoroutine(ShootingDelay());
            Instantiate(ProjectilePrefab, ProjectileSpawn.position, ProjectileSpawn.rotation);
            if (ShootingSource) ShootingSource.Play();
            if (OnFire != null) OnFire();
        }
    }
    
    protected IEnumerator ShootingDelay()
    {
        FireRateDelay = true;
        yield return new WaitForSeconds(1f / FireRate);
        FireRateDelay = false;
    }

    #region Projectile Funcitons
    
    public void ChangeProjectile(GameObject Projectile) { ProjectilePrefab = Projectile; }
    public GameObject GetProjectile() { return ProjectilePrefab; }

    #endregion
    
    #endregion

    #endregion
    
    #region Executables

    [Header("Abillites")]
    public Executable[] Executables = new Executable[4];

    #region Delegates and Events
    
    public delegate void OnExecutableAddedDelegate<Executable>(int slot);
    public delegate void OnExecutableUsedDelegate(int slot);
    
    public event OnExecutableAddedDelegate<Executable> OnExecutableAdded;
    public event OnExecutableUsedDelegate OnExecutableUsed;

    #endregion

    public void AddExecutable<T>(int slot) where T : Executable
    {
        Destroy(Executables[slot - 1]);
        Executables[slot - 1] = gameObject.AddComponent<T>();
        if (OnExecutableAdded != null) OnExecutableAdded(slot - 1);
        Executables[slot - 1].enabled = false;
    }

    protected void UseExecutable(int slot)
    {
        Executables[slot].OnCooldown = true;
        Executables[slot].enabled = true;
        if (OnExecutableUsed != null) OnExecutableUsed(slot);
    }
    
    #endregion
    
    #region Threads

    public List<Thread> Threads = new();
    
    #region Delegates and Events
    
    public delegate void OnThreadAddedDelegate<Thread>();
    public delegate void OnThreadUsedDelegate();
    
    public event OnThreadAddedDelegate<Thread> OnThreadAdded;
    public event OnThreadUsedDelegate OnThreadUsed;

    #endregion
    
    public void AddThread<T>() where T : Thread
    {
        if (Threads != null) Threads.Add(gameObject.AddComponent<T>());
        if (OnThreadAdded != null) OnThreadAdded();
    }
    
    public void UseThread()
    {
        if (OnThreadUsed != null) OnThreadUsed();
    }
    
    #endregion
    
    #region Damage and Deaths

    #region Delegates and Events
    
    public delegate void OnEntityTakeDamageDelegate();
    public delegate void OnEntityDeathDelegate();
    
    public event OnEntityTakeDamageDelegate OnEntityTakeDamage;
    public event OnEntityDeathDelegate OnEntityDeath;
    
    #endregion
    
    public virtual void TakeDamage(int value = 1)
    {
        if (!Invincible)
        {
            if (OnEntityTakeDamage != null) OnEntityTakeDamage();
            Health -= value;
            if (Health <= 0) Death();
        }
    }

    public virtual void Death()
    {
        if (OnEntityDeath != null) OnEntityDeath();
        Destroy(gameObject);
    }

    protected IEnumerator InvincibilityFrames()
    {
        Invincible = true;
        // WaitForSeconds(float duration) where duration is the duration of the Invincibility
        yield return new WaitForSeconds(0.25f);
        Invincible = false;
    }
    
    #endregion
}
