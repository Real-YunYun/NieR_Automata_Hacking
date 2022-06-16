using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum BountyType
{
    Kills,          //Kill x Entities in a time-frame
    Earn,           //Earn x Bits in a time-frame
    Survival,       //Survive x Seconds

}

public struct Bounty
{   
    public BountyType Type;     //Type of Bounty this is
    public int Current;         //Current Goal to Target
    public int Target;          //Target to achieve
    public float TimeIssued;    //When (In Director's time) was issued
    public float Duration;      //How long does this Bounty's rewards persist
    public bool Expired;        //Has it expired
    public bool Completed;      //Has it been completed (EXE is vulnerable)
    public int Reward;          //In Bits

    public void GenerateBounty(float TimeInMins = 0.0167f)
    {
        Type = RandomType<BountyType>();

        float TimeFactor = 0.0506f * GameManager.Instance.DifficultyModifier;
        float StageFactor = Mathf.Pow(1.15f, GameManager.Instance.StangeCount);
        float DifficultyCoe = (1 + TimeInMins * TimeFactor) * StageFactor;

        switch (Type)
        {
            case BountyType.Kills: 
                Current = 0; 
                Target = Random.Range(5, 25);
                TimeIssued = Time.time;
                Duration = Random.Range(20, 120);
                Reward = (int)(Random.Range((int)(80 * DifficultyCoe), (int)(160 * DifficultyCoe)) * GameManager.Instance.DifficultyModifier);
                break;
            case BountyType.Earn:
                Current = 0;
                Target = Random.Range(10, 30);
                TimeIssued = Time.time;
                Duration = Random.Range(20, 60);
                Reward = Random.Range((int)(80 * DifficultyCoe), (int)(160 * DifficultyCoe));
                break;
            case BountyType.Survival:
                Current = 0;
                Target = Random.Range(30, 60);
                TimeIssued = Time.time;
                Duration = Target;
                Reward = Random.Range((int)(Target * DifficultyCoe), (int)(120 * DifficultyCoe));
                break;
        }
    }
    private static T RandomType<T>()
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        System.Random random = new System.Random();
        return (T)values.GetValue(random.Next(values.Length));
    }

}


public class Director : MonoBehaviour
{
    //Directors private Variables
    private GameObject PlayerCamera;

    [Header("Director Parameters")]
    [Tooltip("Enable Debugging for the Player to see Director's values")]
    [SerializeField] private bool Debugging;
    [Tooltip("This is the Spawing Absolute Values that the Director attemps to spawn Entities around the Player in (+/- X, +/- Y)")]
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
    private float TotalTime = 0f;
    private int RandomEntityIndex = 0;
    private float BuildingTime = 0f;
    private float Credit = 0f;
    private float BuildRate = 1f; 
    private float CreditRate = 1f;
    private int StageCount = 0;
    private bool CanSpawn = false;
    RaycastHit hit;
    Vector3 RandomCoordinate;

    //Burst and Burnout System could be inplemented :D

    void Start()
    {
        RandomEntityIndex = Random.Range(0, Entities.Length);
        Vector3 ExecutableObjectSpawn;
        RaycastHit hit;

        CurrentBounty.GenerateBounty();

        while (!ExecutableObject)
        {
            ExecutableObjectSpawn = new Vector3(Random.Range(GameManager.Instance.PlayerInstance.transform.position.x - SpawningBounds.x - 2.5f, GameManager.Instance.PlayerInstance.transform.position.x + SpawningBounds.x + 2.5f), transform.position.y, Random.Range(GameManager.Instance.PlayerInstance.transform.position.z - SpawningBounds.y - 2.5f, GameManager.Instance.PlayerInstance.transform.position.z + SpawningBounds.y + 2.5f));
            if (Physics.Raycast(ExecutableObjectSpawn, transform.TransformDirection(Vector3.down), out hit, 999f, 0b_1000))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    hit.point = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z);
                    ExecutableObject = Instantiate(Resources.Load<GameObject>("Enemies/EXE"), hit.point, Quaternion.identity);
                }
            }
        }
        
        //Burnout System
        Credit = 13f * 1.15f * GameManager.Instance.DifficultyModifier;
        BuildingTime = 13f * 1.15f * GameManager.Instance.DifficultyModifier;
    }

    // Update is called once per frame
    void Update()
    {
        //UI stuff
        if (Debugging)
        {
            if (GameManager.Instance.MainCameraInstance) PlayerCamera = GameManager.Instance.MainCameraInstance;
            else Debug.LogError("Could not find 'PlayerCamera'");

            if (PlayerCamera)
            {
                string BuildTime;
                if (SpawningQueue.Count != 0) BuildTime = SpawningQueue.Peek().GetComponent<Entity>().Stats.BuildCost.ToString();
                else BuildTime = "0";
                GameObject UI_Director = PlayerCamera.transform.Find("UI Canvas/Director Stats").gameObject;
                UI_Director.SetActive(true);
                UI_Director.transform.Find("Text").GetComponent<Text>().text = 
                    "Bounty: \n" +
                    "Current Bounty: "+ CurrentBounty.Type + "\n" +
                    "Bounty Objective: " + CurrentBounty.Current + "/"+ CurrentBounty.Target + "\n" +
                    "Bounty Reward: " + CurrentBounty.Reward + "\n" +
                    "Bounty Completed?: " + CurrentBounty.Completed + "\n" +
                    "Director Stats:\n" +
                    "Total Time Elasped: " + TotalTime + "\n" +
                    "Building Time: " + (int)BuildingTime + "/" + BuildTime + "\n" +
                    "Credits: " + (int)Credit + "\n" +
                    "Difficulty Modifier: " + GameManager.Instance.DifficultyModifier + "\n" +
                    "Built Entities: " + SpawnedEntities.Count + " units\n" +
                    "Build Rate: "  + BuildRate + " (/s)\n" +
                    "Credit Rate: "  + CreditRate + " (/s)\n" +
                    "Entities: " + ShowListEntities();
            }
        }

        if (Time.timeScale == 1 && ExecutableObject)
        {
            //Actual Director Algorithm
            TotalTime += 0.005f + Mathf.Pow(Time.deltaTime, 2) * GameManager.Instance.DifficultyModifier;
            //Enqueuing Entites
            if (SpawningQueue.Count < MaxQueue)
            {
                RandomEntityIndex = Random.Range(0, Entities.Length);
                SpawningQueue.Enqueue(Entities[RandomEntityIndex]);
            }

            if (Credit >= SpawningQueue.Peek().GetComponent<Entity>().Stats.BuildCost) Credit -= SpawningQueue.Peek().GetComponent<Entity>().Stats.BuildCost;

            if (!CanSpawn)
            {
                RandomCoordinate = new Vector3(Random.Range(GameManager.Instance.PlayerInstance.transform.position.x - SpawningBounds.x - 2.5f, GameManager.Instance.PlayerInstance.transform.position.x + SpawningBounds.x + 2.5f), transform.position.y, Random.Range(GameManager.Instance.PlayerInstance.transform.position.z - SpawningBounds.y - 2.5f, GameManager.Instance.PlayerInstance.transform.position.z + SpawningBounds.y + 2.5f));
                if (Physics.Raycast(RandomCoordinate, transform.TransformDirection(Vector3.down), out hit, 999f, 0b_1000))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) CanSpawn = true;
                }
            }

            //Spawning Entity
            if (SpawningQueue.Peek().GetComponent<Entity>().Stats.BuildTime <= BuildingTime) StartCoroutine("Spawn");

            CreditRate = (1 + TotalTime * 0.0506f) * 1.15f / 3600;
            BuildRate = (1 + TotalTime * 0.0506f) * 1.15f / 3600;

            Credit += CreditRate;
            BuildingTime += BuildRate;
        }
    }

    public void OnEntityDeath(GameObject entity)
    {
        GameManager.Instance.Data.KillCount++;
        SpawnedEntities.Remove(SpawnedEntities.Find(GetHashCode => entity));
    }

    IEnumerator Spawn()
    {
        if (SpawnedEntities.Count < MaxEntities)
        {
            BuildingTime -= SpawningQueue.Peek().GetComponent<Entity>().Stats.BuildTime;
            RandomCoordinate.y = hit.point.y + 1;
            var entity = Instantiate(SpawningQueue.Peek(), RandomCoordinate, Quaternion.identity);
            entity.GetComponent<Entity>().OnDeathEvent += OnEntityDeath;
            SpawnedEntities.Add(entity);
            SpawningQueue.Dequeue();
        }
        yield return new WaitForSeconds(0.5f);
        CanSpawn = false;
    }

    private string ShowListEntities()
    {
        string List = "";
        for (int i = 0; i < Entities.Length; i++)
        {
            List += Entities[i].name + " ";
        }
        return List;
    }

    public int GetStage() { return StageCount; }
}
