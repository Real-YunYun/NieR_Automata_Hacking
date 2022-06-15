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

    public delegate void OnAbilityAddedDelegate<Ability>(int slot);
    public event OnAbilityAddedDelegate<Ability> OnAbilityAddedEvent;
    
    public delegate void OnAbilityUsedDelegate(int slot);
    public event OnAbilityUsedDelegate OnAbilityUsedEvent;

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
    public Ability[] Abilities = new Ability[4];

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
        for (int i = 1; i < Abilities.Length + 1; i++) AddAbility<NoAbility>(i);

        //Adding Abilities
        AddAbility<Nodes>(1);
        AddAbility<Firewall>(2);
        AddAbility<Teleport>(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            //Player Input Devices
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;

            //Keyboard Input
            if (CanMove)
            {
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

                //Abillities Input
                if (keyboard.digit1Key.isPressed && !Abilities[0].OnCooldown && Abilities[0].Usable)
                {
                    Abilities[0].OnCooldown = true;
                    Abilities[0].enabled = true;
                    OnAbilityUsedEvent(0);
                }
                if (keyboard.digit2Key.isPressed && !Abilities[1].OnCooldown && Abilities[1].Usable)
                {
                    Abilities[1].OnCooldown = true;
                    Abilities[1].enabled = true;
                    OnAbilityUsedEvent(1);
                }
                if (keyboard.digit3Key.isPressed && !Abilities[2].OnCooldown && Abilities[2].Usable)
                {
                    Abilities[2].OnCooldown = true;
                    Abilities[2].enabled = true;
                    OnAbilityUsedEvent(2);
                }
                if (keyboard.digit4Key.isPressed && !Abilities[3].OnCooldown && Abilities[3].Usable)
                {
                    Abilities[3].OnCooldown = true;
                    Abilities[3].enabled = true;
                    OnAbilityUsedEvent(3);
                }

                if (GravityOn) Controller.Move(new Vector3(0, Gravity * Time.deltaTime, 0));
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

    public void AddAbility<T>(int slot) where T : Ability
    {
        Destroy(Abilities[slot - 1]);
        Abilities[slot - 1] = gameObject.AddComponent<T>();
        if (OnAbilityAddedEvent != null) OnAbilityAddedEvent(slot - 1);
        Abilities[slot - 1].enabled = false;
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
