using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Executables;
using Threads;

public class PlayerController : Entity
{
    //This GameObject Components
    private CharacterController Controller;

    [Header("Player Stats")]
    // new private int _health = 50;
    private int _energy = 25;
    // public int Health { get { return _health; } set { _health = value; } }
    public int Energy { get { return _energy; } set { _energy = value; } }

    #region Contorller Params 
    [Header("Player Controller Parameters")]
    public float MoveSpeed = 10f;
    public float FireRate = 8f;
    private bool FireRateDelay;
    private readonly float TurningSmoothing = 0.1f;
    private float TurnSmoothVelocity;
    private readonly float Gravity = -9.8f;
    [HideInInspector] public bool GravityOn = true;
    private Vector3 Velocity = Vector3.zero;
    private GameObject Minimap;
    //private GameObject AimingLine;
    #endregion

    #region Delegates and Events
    public delegate void OnFireDelegate();
    public event OnFireDelegate OnFireEvent;

    public delegate void OnExecutableAddedDelegate<Executable>(int slot);
    public event OnExecutableAddedDelegate<Executable> OnExecutableAddedEvent;
    
    public delegate void OnExecutableUsedDelegate(int slot);
    public event OnExecutableUsedDelegate OnExecutableUsedEvent;
    #endregion

    #region Player Controller Booleans
    [Header("Player Controller Limitations")]
    [SerializeField] private bool CanMove = true;
    [SerializeField] private bool CanShoot = true;
    [SerializeField] private bool CanExecute = true;
    new private bool Invincible { get; set; }
    #endregion

    #region Player Children
    [Header("Controller Children")]
    [SerializeField] private GameObject ProjectilePrefab;
    private GameObject PlayerBody;
    private Transform ProjectileSpawn;
    #endregion

    #region Abillities
    [Header("Abillites")]
    public Executable[] Executables = new Executable[4];
    #endregion

    [Header("Audio Clips")]
    private AudioSource ShootingSource;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
        PlayerBody = GameObject.Find("Player Mesh");
        ProjectileSpawn = transform.Find("Player Mesh/Projectile Spawn");
        ShootingSource = transform.Find("Player Mesh/Projectile Spawn").GetComponent<AudioSource>();
        Minimap = Instantiate(Resources.Load<GameObject>("Player/MiniMapCamera"));
    }

    void Start()
    {
        for (int i = 1; i < Executables.Length + 1; i++) AddExecutable<NoExecutable>(i);

        //Adding Abilities
        AddExecutable<Nodes>(1);
        AddExecutable<Firewall>(2);
        AddExecutable<Teleport>(3);
        AddExecutable<Overclock>(4);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            #region Player Input Devices
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            Gamepad gamepad = Gamepad.current;
            #endregion

            if (_health > 50) _health = 50;

            if (CanMove)
            {
                #region Keyboard Input  
                float Forward = (keyboard.wKey.ReadValue() - keyboard.sKey.ReadValue());
                float Side = (keyboard.dKey.ReadValue() - keyboard.aKey.ReadValue());
                Vector3 Direction = new Vector3(Side, 0, Forward).normalized;
                Vector3 MoveDir;

                //Turning the Character Model relative to the Movement Direction
                if (Direction.magnitude >= 0.1f)
                {
                    float TurningAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    float ResultAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TurningAngle, ref TurnSmoothVelocity, TurningSmoothing);
                    transform.rotation = Quaternion.Euler(0f, ResultAngle, 0f);
                    MoveDir = Quaternion.Euler(0f, TurningAngle, 0f) * Vector3.forward;

                    Controller.Move(MoveDir * MoveSpeed * Time.deltaTime);
                }
                #endregion

                #region Keyboard Input For Exectuables
                //Abillities Keyboard Input
                if (keyboard.digit1Key.wasPressedThisFrame && !Executables[0].OnCooldown && Executables[0].Usable) UseExecutable(0);
                if (keyboard.digit2Key.wasPressedThisFrame && !Executables[1].OnCooldown && Executables[1].Usable) UseExecutable(1);
                if (keyboard.digit3Key.wasPressedThisFrame && !Executables[2].OnCooldown && Executables[2].Usable) UseExecutable(2);
                if (keyboard.digit4Key.wasPressedThisFrame && !Executables[3].OnCooldown && Executables[3].Usable) UseExecutable(3);
                #endregion

                #region Mouse Input
                if (CanShoot) if (mouse.leftButton.ReadValue() > 0) Fire();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, 0b_1000_0000))
                {
                    Vector3 rotation = Quaternion.LookRotation(rayHit.point - transform.position, Vector3.up).eulerAngles;
                    rotation.x = rotation.z = 0;
                    PlayerBody.transform.rotation = Quaternion.Euler(rotation);

                    #region Debugging Aiming Line
                    #if UNITY_EDITOR_WIN
                        Debug.DrawLine(rayHit.point, rayHit.point + (Vector3.up * 5f), Color.red);
                        Debug.DrawLine(ProjectileSpawn.transform.position, ProjectileSpawn.transform.position + (ProjectileSpawn.transform.forward * 1000f), Color.green);
                    #endif
                    #endregion
                }
                /*
                if (CanShoot && mouse.rightButton.ReadValue() > 0)
                {
                    if (!AimingLine) AimingLine = Instantiate(Resources.Load<GameObject>("Player/Aiming Line"), ProjectileSpawn.position, ProjectileSpawn.rotation);
                    RaycastHit hit;
                    LineRenderer LineRendererComp = AimingLine.GetComponent<LineRenderer>();
                    if (Physics.Raycast(ProjectileSpawn.position, ProjectileSpawn.forward, out hit, Mathf.Infinity))
                    {
                        LineRendererComp.positionCount = Mathf.Abs((int)(hit.point - ProjectileSpawn.position).magnitude);
                        for (int i = 1; i < LineRendererComp.positionCount; i++) LineRendererComp.SetPosition(i, new Vector3(0, 0, 1f * i));
                        AimingLine.transform.parent = ProjectileSpawn;
                    }
                } else Destroy(AimingLine);
                */
            }
            #endregion

            if (GravityOn) Controller.Move(new Vector3(0, Gravity * Time.deltaTime, 0));
        }
    }

    public void AddExecutable<T>(int slot) where T : Executable
    {
        Destroy(Executables[slot - 1]);
        Executables[slot - 1] = gameObject.AddComponent<T>();
        if (OnExecutableAddedEvent != null) OnExecutableAddedEvent(slot - 1);
        Executables[slot - 1].enabled = false;
    }

    private void Fire()
    {
        if (!FireRateDelay)
        {
            StartCoroutine("ShootingDelay");
            Instantiate(ProjectilePrefab, ProjectileSpawn.position, ProjectileSpawn.rotation);
            ShootingSource.Play();
            if (OnFireEvent != null) OnFireEvent();
        }
    }

    IEnumerator ShootingDelay()
    {
        FireRateDelay = true;
        yield return new WaitForSeconds(1f / FireRate);
        FireRateDelay = false;
    }

    public override void TakeDamage(int value = 1)
    {
        //Checking if the Player is Invincible
        if (!Invincible)
        {
            StartCoroutine("InvincibilityFrames");
            Health -= value;
            if (Health <= 0)
            {
                GameManager.Instance.SaveGame();
                GameManager.Instance.LoadGame();
            }
        }
        else return;
    }

    private void UseExecutable(int slot)
    {
        if (CanExecute)
        {
            Executables[slot].OnCooldown = true;
            Executables[slot].enabled = true;
            OnExecutableUsedEvent(slot);
        }
    }

    #region Player Projectile Funcitons
    public void ChangeProjectile(GameObject Projectile)
    {
        ProjectilePrefab = Projectile;
    }

    public GameObject GetProjectile()
    {
        return ProjectilePrefab;
    }

    #endregion

    #region Player Controller Boolean Parameters
    public void SetCanExecute(bool state = true) { CanExecute = state; }
    public void SetCanShoot(bool state = true) { CanShoot = state; }
    public void SetMoveShoot(bool state = true) { CanMove = state; }
    #endregion

    IEnumerator InvincibilityFrames()
    {
        Invincible = true;
        // WaitForSeconds(float duration) where duration is the duration of the Invincibility
        yield return new WaitForSeconds(0.25f);
        Invincible = false;
    }
}
