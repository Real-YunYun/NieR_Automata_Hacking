using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //This GameObject Components
    private CharacterController Controller;

    [Header("Player Stats")]
    private int _health = 50;
    private int _energy = 25;
    public int Health { get { return _health; } set { _health = value; } }
    public int Energy { get { return _energy; } set { _energy = value; } }

    //Contorller Params
    [Header("Player Controller Parameters")]
    public float MoveSpeed = 10f;
    public float FireRate = 8f;
    private bool FireRateDelay;
    private readonly float TurningSmoothing = 0.1f;
    private float TurnSmoothVelocity;
    private readonly float Gravity = -9.8f;
    [HideInInspector] public bool GravityOn = true;
    private Vector3 Velocity = Vector3.zero;

    //Delegates and Events
    public delegate void OnFireDelegate();
    public event OnFireDelegate OnFireEvent;

    public delegate void OnExecutableAddedDelegate<Executable>(int slot);
    public event OnExecutableAddedDelegate<Executable> OnExecutableAddedEvent;
    
    public delegate void OnExecutableUsedDelegate(int slot);
    public event OnExecutableUsedDelegate OnExecutableUsedEvent;

    //Player Controller Booleans
    [Header("Player Controller Limitations")]
    [SerializeField] private bool CanMove = true;
    [SerializeField] private bool CanShoot = true;
    private bool Invincible { get; set; }

    //Player Children
    [Header("Controller Children")]
    [SerializeField] private GameObject ProjectilePrefab;
    private GameObject PlayerBody;
    private Transform ProjectileSpawn;

    //Abillities
    [Header("Abillites")]
    public Executable[] Executables = new Executable[4];

    [Header("Audio Clips")]
    private AudioSource shootingSource;

    // Start is called before the first frame update
    void Awake()
    {
        Controller = GetComponent<CharacterController>();
        PlayerBody = GameObject.Find("Player Mesh");
        ProjectileSpawn = transform.Find("Player Mesh/Projectile Spawn");
        shootingSource = transform.Find("Player Mesh/Projectile Spawn").GetComponent<AudioSource>();
    }

    void Start()
    {
        for (int i = 1; i < Executables.Length + 1; i++) AddExecutable<NoExecutable>(i);

        //Adding Abilities
        AddExecutable<Nodes>(1);
        AddExecutable<Firewall>(2);
        AddExecutable<Teleport>(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            //Player Input Devices
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            Gamepad gamepad = Gamepad.current;

            if (_health > 50) _health = 50;

            //Keyboard Input
            if (CanMove)
            {
                float Forward;
                float Side;
                if (Gamepad.current != null && GameManager.Instance.UseGamepad)
                {
                    Forward = gamepad.leftStick.up.ReadValue() - gamepad.leftStick.down.ReadValue();
                    Side = gamepad.leftStick.right.ReadValue() - gamepad.leftStick.left.ReadValue();

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

                    //Firing
                    if (CanShoot) if (gamepad.rightTrigger.ReadValue() > 0) Fire();

                    //Right Stick Handling
                    Vector2 aim = gamepad.rightStick.ReadValue();
                    if (aim.x != 0 && aim.y != 0)
                    {
                        Vector3 rotation = new Vector3(aim.x, 0, aim.y);
                        PlayerBody.transform.rotation = Quaternion.LookRotation(rotation);
                    }
                    else PlayerBody.transform.rotation = transform.rotation;

                }
                else
                {
                    Forward = (keyboard.wKey.ReadValue() - keyboard.sKey.ReadValue());
                    Side = (keyboard.dKey.ReadValue() - keyboard.aKey.ReadValue());
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

                    //Abillities Keyboard Input
                    if (keyboard.digit1Key.isPressed && !Executables[0].OnCooldown && Executables[0].Usable)
                    {
                        Executables[0].OnCooldown = true;
                        Executables[0].enabled = true;
                        OnExecutableUsedEvent(0);
                    }
                    if (keyboard.digit2Key.isPressed && !Executables[1].OnCooldown && Executables[1].Usable)
                    {
                        Executables[1].OnCooldown = true;
                        Executables[1].enabled = true;
                        OnExecutableUsedEvent(1);
                    }
                    if (keyboard.digit3Key.isPressed && !Executables[2].OnCooldown && Executables[2].Usable)
                    {
                        Executables[2].OnCooldown = true;
                        Executables[2].enabled = true;
                        OnExecutableUsedEvent(2);
                    }
                    if (keyboard.digit4Key.isPressed && !Executables[3].OnCooldown && Executables[3].Usable)
                    {
                        Executables[3].OnCooldown = true;
                        Executables[3].enabled = true;
                        OnExecutableUsedEvent(3);
                    }

                    //Mouse Input
                    if (CanShoot) if (mouse.leftButton.ReadValue() > 0) Fire();
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, 0b_0100_1000))
                    {
                        Vector3 rotation = Quaternion.LookRotation(raycastHit.point - transform.position, Vector3.up).eulerAngles;
                        rotation.x = rotation.z = 0;
                        PlayerBody.transform.rotation = Quaternion.Euler(rotation);
                    }
                }
            }


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
            shootingSource.Play();
            if (OnFireEvent != null) OnFireEvent();
        }
    }

    IEnumerator ShootingDelay()
    {
        FireRateDelay = true;
        yield return new WaitForSeconds(1f / FireRate);
        FireRateDelay = false;
    }

    public void TakeDamage()
    {
        //Checking if the Player is Invincible
        if (!Invincible)
        {
            StartCoroutine("InvincibilityFrames");
            Health -= 1;
            if (Health <= 0)
            {
                GameManager.Instance.SaveGame();
                GameManager.Instance.LoadGame();
            }
        }
        else return;
    }

    IEnumerator InvincibilityFrames()
    {
        Invincible = true;
        // WaitForSeconds(float duration) where duration is the duration of the Invincibility
        yield return new WaitForSeconds(0.25f);
        Invincible = false;
    }
}
