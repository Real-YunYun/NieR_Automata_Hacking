using UnityEngine;
using UnityEngine.InputSystem;
using Projectiles;

public class PlayerController : Entity
{
    //This GameObject Components
    private CharacterController Controller;
    private GameObject PlayerBody;
    private GameObject Minimap;

    #region Contorller Params

    [Header("Player Controller Parameters")]
    private readonly float TurningSmoothing = 0.1f;
    private float TurnSmoothVelocity;
    private readonly float Gravity = -9.8f;
    [HideInInspector] public bool GravityOn = true;
    private Vector3 Velocity = Vector3.zero;

    #endregion

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
        PlayerBody = GameObject.Find("Player Mesh");
        ProjectileSpawn = transform.Find("Player Mesh/Projectile Spawn");
        ShootingSource = transform.Find("Player Mesh/Projectile Spawn").GetComponent<AudioSource>();
        Minimap = Instantiate(Resources.Load<GameObject>("Player/MiniMapCamera"));
    }

    protected override void Start()
    {
        base.Start();

        //Manually Adding Abilities
        AddExecutable<Nodes>(1);
        AddExecutable<Firewall>(2);
        AddExecutable<Teleport>(3);
        AddExecutable<Overclock>(4);
        
        AddThread<HomingThread>();
        AddThread<TrackingMissle>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            if (_health > 50) _health = 50;

            #region Player Input Devices

            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            Gamepad gamepad = Gamepad.current;

            #endregion

            if (CanMove)
                Move();

            #region Mouse Input

            if (CanShoot && mouse.leftButton.ReadValue() > 0) Fire();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, 0b_1000_0000))
            {
                Vector3 rotation = Quaternion.LookRotation(rayHit.point - transform.position, Vector3.up).eulerAngles;
                rotation.x = rotation.z = 0;
                PlayerBody.transform.rotation = Quaternion.Euler(rotation);
                /* Debug Aim Line
                Debug.DrawLine(rayHit.point, rayHit.point + (Vector3.up * 5f), Color.red);
                Debug.DrawLine(ProjectileSpawn.transform.position, ProjectileSpawn.transform.position + (ProjectileSpawn.transform.forward * 1000f), Color.green);
                */
            }

            #endregion

            #region Keyboard Input For Exectuables

            if (CanExecute)
            {
                if (keyboard.digit1Key.wasPressedThisFrame && !Executables[0].OnCooldown && Executables[0].Usable)
                    UseExecutable(0);
                if (keyboard.digit2Key.wasPressedThisFrame && !Executables[1].OnCooldown && Executables[1].Usable)
                    UseExecutable(1);
                if (keyboard.digit3Key.wasPressedThisFrame && !Executables[2].OnCooldown && Executables[2].Usable)
                    UseExecutable(2);
                if (keyboard.digit4Key.wasPressedThisFrame && !Executables[3].OnCooldown && Executables[3].Usable)
                    UseExecutable(3);
            }

            #endregion
        }
    }

    protected override void Fire()
    {
        if (!FireRateDelay)
        {
            StartCoroutine(ShootingDelay());
            Execute_OnFireStarted();
            Projectile TempProjectile = Instantiate(ProjectilePrefab, ProjectileSpawn.position, ProjectileSpawn.rotation).GetComponent<Projectile>();
            TempProjectile.InitResult(this);
            if (ShootingSource) ShootingSource.Play();
            Execute_OnFireEnded(TempProjectile);
        }
    }

    protected override void Move()
    {
        if (GameManager.Instance.IsGamePaused) return;

        #region Player Input Devices

        Keyboard keyboard = Keyboard.current;

        #endregion

        #region Keyboard Input

        float Forward = (keyboard.wKey.ReadValue() - keyboard.sKey.ReadValue());
        float Side = (keyboard.dKey.ReadValue() - keyboard.aKey.ReadValue());
        Vector3 Direction = new Vector3(Side, 0, Forward).normalized;
        Vector3 MoveDir;

        //Turning the Character Model relative to the Movement Direction
        if (Direction.magnitude >= 0.1f)
        {
            float TurningAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg +
                                 Camera.main.transform.eulerAngles.y;
            float ResultAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TurningAngle, ref TurnSmoothVelocity,
                TurningSmoothing);
            transform.rotation = Quaternion.Euler(0f, ResultAngle, 0f);
            MoveDir = Quaternion.Euler(0f, TurningAngle, 0f) * Vector3.forward;

            Controller.Move(MoveDir * (MoveSpeed * Time.deltaTime));
        }

        if (GravityOn) Controller.Move(new Vector3(0, Gravity * Time.deltaTime, 0));
            
        #endregion
    }

    public override void TakeDamage(int value = 1)
    {
        //Checking if the Player is Invincible
        if (Invincible) return;

        StartCoroutine(InvincibilityFrames());
        Health -= value;
        if (Health <= 0)
        {
            GameManager.Instance.SaveGame();
            GameManager.Instance.LoadGame();
        }
    }
}
