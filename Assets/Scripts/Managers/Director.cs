using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Entities;
using Entities.Enemies;
using Random = UnityEngine.Random;

public enum BountyType
{
    Kill,          //Kill x Entities in a time-frame
    Earn,           //Earn x Bits in a time-frame
    //Survival,       //Survive x Seconds
}

public struct Bounty {   
    //Bounty Parameters
    public BountyType Type;     //Type of Bounty this is
    public int Current;         //Current Goal to Target
    public int Target;          //Target to achieve
    public float DeltaTime;     //Change in Time since the bounty was made
    public float Duration;      //How long does this Bounty's rewards persist
    public bool Expired;        //Has it expired
    public bool Completed;      //Has it been completed (EXE is vulnerable)
    public int Reward;          //In Bits

    public Bounty(BountyType type = BountyType.Kill, int current = 0, int target = 0, float timeissued = 0f, float deltatime = 0, float duration = 0, bool expired = false, bool completed = false, int reward = 0) {
        Type = type;
        Current = current;
        Target = target;
        DeltaTime = deltatime;
        Duration = duration;
        Expired = expired;
        Completed = completed;
        Reward = reward;
    }

    public void GenerateBounty(float RunTime = 0)
    {
        Type = RandomType<BountyType>();

        float TimeInMins = RunTime % 60;

        float TimeFactor = 0.0506f * GameManager.Instance.DifficultyModifier;
        float StageFactor = Mathf.Pow(1.15f, GameManager.Instance.StangeCount);
        float DifficultyCoe = (1 + TimeInMins * TimeFactor) * StageFactor;

        switch (Type)
        {
            case BountyType.Kill: 
                Current = 0; 
                Target = Random.Range(5, 10);
                DeltaTime = 0;
                Duration = Random.Range(20, 120);
                Reward = (int)(Random.Range((int)(80 * DifficultyCoe), (int)(160 * DifficultyCoe)) * GameManager.Instance.DifficultyModifier);
                break;
            case BountyType.Earn:
                Current = 0;
                Target = Random.Range(10, 30);
                DeltaTime = 0;
                Duration = Random.Range(20, 60);
                Reward = Random.Range((int)(80 * DifficultyCoe), (int)(160 * DifficultyCoe));
                break;
        }
    }

    public void HandleBounty(GameObject entity) {
        if (Type == BountyType.Kill) {
            Current++;
            if (Current >= Target) {
                Completed = true;
                if (!Expired) GameManager.Instance.Data.Bits += Reward;
                DeltaTime = 0;
            }
        }
        if (Type == BountyType.Earn) {
            Current += (int)entity.GetComponent<Enemy>().Stats.BuildCost;
            if (Current >= Target)
            {
                Completed = true;
                if (!Expired) GameManager.Instance.Data.Bits += Reward;
                DeltaTime = 0;
            }
        }
    }

    private static T RandomType<T>() {
        System.Array values = System.Enum.GetValues(typeof(T));
        System.Random random = new System.Random();
        return (T)values.GetValue(random.Next(values.Length));
    }

}


public class Director : MonoBehaviour {
    //Directors private Variables
    private GameObject PlayerCamera;

    [Header("Director Parameters")]
    [Tooltip("Enable Debugging for the Player to see Director's values")]
    [SerializeField] private bool Debugging;
    [Tooltip("This is the Spawing Absolute Values that the Director attempts to spawn Entities around the Player in (+/- X, +/- Y)")]
    [SerializeField] private Vector2 SpawningBounds;
    [Tooltip("This is what the Director will spawn")]
    [SerializeField] private GameObject[] Entities;    

    [Tooltip("Current Queue for Director to spawn")]
    private Queue<GameObject> SpawningQueue = new Queue<GameObject>();
    [Tooltip("Max Queue amount the Director will remember to spawn")]
    [SerializeField] private int MaxQueue = 15;

    [Tooltip("Max amount of Entities the Director will spawn")]
    [SerializeField] private int MaxEntities = 20;
    [Tooltip("Current Entities Spawned")]
    public List<GameObject> SpawnedEntities = new List<GameObject>();

    //Conditional Variables
    private GameObject ExecutableObject;
    public Bounty CurrentBounty;
    private float DifficultyTime = 0f;
    private float RunTime = 0f;
    private int RandomEntityIndex = 0;
    private float BuildingTime = 0f;
    private float Credit = 0f;
    private float BuildRate = 1f; 
    private float CreditRate = 1f;
    private bool CanSpawn = false;
    RaycastHit Hit;
    [SerializeField] Vector3 RandomCoordinate;

    void Start() {
        RandomEntityIndex = Random.Range(0, Entities.Length);
        
        //Burnout System
        // Credit = 1.15f * GameManager.Instance.DifficultyModifier;
        // BuildingTime = 1.15f * GameManager.Instance.DifficultyModifier;
    }

    void OnEnable() {
        Vector3 ExecutableObjectSpawn;
        RaycastHit Hit;
        CurrentBounty.GenerateBounty(RunTime);

        GameObject PlayerObject = GameManager.Instance.PlayerControllerInstance.gameObject;

        while (!ExecutableObject && PlayerObject != null) {
            ExecutableObjectSpawn = new Vector3(
                Random.Range(PlayerObject.transform.position.x - SpawningBounds.x - 2.5f, PlayerObject.transform.position.x + SpawningBounds.x + 2.5f), 
                transform.position.y, 
                Random.Range(PlayerObject.transform.position.z - SpawningBounds.y - 2.5f, PlayerObject.transform.position.z + SpawningBounds.y + 2.5f));
            
            if (Physics.Raycast(ExecutableObjectSpawn, transform.TransformDirection(Vector3.down), out Hit, 999f, 0b_1000)) {
                if (Hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                    Hit.point = new Vector3(Hit.point.x, Hit.point.y + 1, Hit.point.z);
                    ExecutableObject = Instantiate(Resources.Load<GameObject>("Enemies/EXE"), Hit.point, Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
        GameObject PlayerObject = GameManager.Instance.PlayerControllerInstance.gameObject;
        
        //UI Debugging stuff
        if (Debugging) {
            if (GameManager.Instance.MainCameraInstance) PlayerCamera = GameManager.Instance.MainCameraInstance;
            else Debug.LogError("Could not find 'PlayerCamera'");

            if (PlayerCamera) {
                string BuildTime;
                if (SpawningQueue.Count != 0) BuildTime = SpawningQueue.Peek().GetComponent<Enemy>().Stats.BuildCost.ToString();
                else BuildTime = "0";
                GameObject UI_Director = PlayerCamera.transform.Find("UI Canvas/Director Stats").gameObject;
                UI_Director.SetActive(true);
                UI_Director.transform.Find("Text").GetComponent<Text>().text =
                    "Director Stats:\n" +
                    "Run Time: " + RunTime + "\n" +
                    "Difficulty Time Elasped: " + DifficultyTime + "\n" +
                    "Building Time: " + (int)BuildingTime + "/" + BuildTime + "\n" +
                    "Credits: " + (int)Credit + "\n" +
                    "Difficulty Modifier: " + GameManager.Instance.DifficultyModifier + "\n" +
                    "Built Entities: " + SpawnedEntities.Count + " units\n" +
                    "Build Rate: " + BuildRate + " (/s)\n" +
                    "Credit Rate: " + CreditRate + " (/s)\n";
            }
        }

        RunTime += Time.deltaTime;
        if (Math.Abs(Time.timeScale - 1) < 0.05f) {
            //Actual Director Algorithm
            DifficultyTime += 0.005f + Mathf.Pow(Time.deltaTime, 2) * GameManager.Instance.DifficultyModifier;
            if (ExecutableObject) {
                //Enqueuing Entites
                if (SpawningQueue.Count < MaxQueue) {
                    RandomEntityIndex = Random.Range(0, Entities.Length);
                    SpawningQueue.Enqueue(Entities[RandomEntityIndex]);
                }

                if (Credit >= SpawningQueue.Peek().GetComponent<Enemy>().Stats.BuildCost) Credit -= SpawningQueue.Peek().GetComponent<Enemy>().Stats.BuildCost;

                //Spawning Entity
                if (SpawningQueue.Peek().GetComponent<Enemy>().Stats.BuildTime <= BuildingTime) StartCoroutine(Spawn());

                CreditRate = (1.5f + DifficultyTime * 0.106f) * 1.15f / 3600;
                BuildRate = (1.5f + DifficultyTime * 0.106f) * 1.15f / 3600;

                Credit += CreditRate;
                BuildingTime += BuildRate;

                if (!CurrentBounty.Expired && !CurrentBounty.Completed) CurrentBounty.DeltaTime += Time.deltaTime;

                if (CurrentBounty.Duration - CurrentBounty.DeltaTime <= 0) CurrentBounty.Expired = true;
                if (CurrentBounty.Completed) {
                    ExecutableObject.transform.Find("Shield").gameObject.SetActive(false);
                    HealthComponent EXEHealthComponent;
                    if (ExecutableObject.transform.GetChild(0).TryGetComponent(out EXEHealthComponent))
                        EXEHealthComponent.Invincible = false;
                }
            }
        }

        if (!ExecutableObject) this.enabled = false;

        if (Keyboard.current.equalsKey.wasPressedThisFrame) GameManager.Instance.DifficultyModifier++;
        if (Keyboard.current.minusKey.wasPressedThisFrame) GameManager.Instance.DifficultyModifier--;
    }

    public void OnEntityDeath(GameObject entity) {
        if (!CurrentBounty.Completed || !CurrentBounty.Expired) CurrentBounty.HandleBounty(entity);
        GameManager.Instance.Data.KillCount++;
        SpawnedEntities.Remove(SpawnedEntities.Find(GetHashCode => entity));
    }

    IEnumerator Spawn() {
        if (SpawnedEntities.Count < MaxEntities) {
            BuildingTime -= SpawningQueue.Peek().GetComponent<Enemy>().Stats.BuildTime;
            SpawnRandomCoordinate();
            RandomCoordinate.y = Hit.point.y + 1;
            var Entity = Instantiate(SpawningQueue.Peek(), RandomCoordinate, Quaternion.identity);
            Entity.GetComponent<Enemy>().OnEnemyDeath += OnEntityDeath;
            SpawnedEntities.Add(Entity);
            SpawningQueue.Dequeue();
        }
        yield return new WaitForSeconds(0.5f);
        CanSpawn = false;
    }

    private void SpawnRandomCoordinate() {
        GameObject PlayerObject = GameManager.Instance.PlayerInstance;

        while (!CanSpawn) {
            RandomCoordinate = new Vector3(
                Random.Range(PlayerObject.transform.position.x - SpawningBounds.x - 2.5f,
                    PlayerObject.transform.position.x + SpawningBounds.x + 2.5f),
                transform.position.y,
                Random.Range(PlayerObject.transform.position.z - SpawningBounds.y - 2.5f,
                    PlayerObject.transform.position.z + SpawningBounds.y + 2.5f));
            
            if (Physics.Raycast(RandomCoordinate, transform.TransformDirection(Vector3.down), out Hit, 999f, 0b_1000)) {
                if (Hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) CanSpawn = true;
            }
        }
    }

    private string ShowListEntities() {
        string List = "";
        for (int i = 0; i < Entities.Length; i++) {
            List += Entities[i].name + " ";
        }
        return List;
    }

    public Bounty GetBounty() { return CurrentBounty; }
    public float GetDifficultyTime() { return DifficultyTime; }
    public float GeRunTime() { return RunTime; }
}
