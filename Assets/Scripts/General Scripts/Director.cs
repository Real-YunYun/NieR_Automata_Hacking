using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Director : MonoBehaviour
{
    //Directors private Variables
    private GameObject PlayerCamera;
    private int DesiredLayerMask = 0b_0000_0000_0000_0000_0000_0000_0000_1001;

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
    private float TotalTime = 0f;
    private int RandomEntityIndex = 0;
    private float BuildingTime = 0f;
    private float Credit = 0f;
    private float DifficultyModifier = 0.75f; //Easy: 0.75f, Normal: 1f, Hard: 1.3f
    private float BuildRate = 1f; 
    private float CreditRate = 1f;
    private bool CanSpawn = false;
    RaycastHit hit;
    Vector3 RandomCoordinate;

    //Burst and Burnout System could be inplemented :D

    void Start()
    {
        RandomEntityIndex = Random.Range(0, Entities.Length);
        //RandomCoordinate = new Vector3(Random.Range(-SpawningBounds.x, SpawningBounds.x) + transform.position.x, transform.position.y, Random.Range(-SpawningBounds.y, SpawningBounds.y) + transform.position.z);
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
                    "Director Stats:\n" +
                    "Total Time Elasped: " + TotalTime + "\n" +
                    "Building Time: " + (int)BuildingTime + "/" + BuildTime + "\n" +
                    "Credits: " + (int)Credit + "\n" +
                    "Difficulty Modifier: " + DifficultyModifier + "\n" +
                    "Built Entities: " + SpawnedEntities.Count + " units\n" +
                    "Build Rate: "  + BuildRate + " (/s)\n" +
                    "Credit Rate: "  + CreditRate + " (/s)\n" +
                    "Entities: " + ShowListEntities();
            }
        }

        if (Time.timeScale == 1)
        {
            //Actual Director Algorithm
            TotalTime += 0.005f + Mathf.Pow(Time.deltaTime, 2) * DifficultyModifier;
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
                if (Physics.Raycast(RandomCoordinate, transform.TransformDirection(Vector3.down), out hit, 999f, DesiredLayerMask))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) CanSpawn = true;
                }
            }

            //Spawning Entity
            if (SpawningQueue.Peek().GetComponent<Entity>().Stats.BuildTime <= BuildingTime) StartCoroutine("Spawn");

            CreditRate = (1 + TotalTime * 0.0506f) * 1.15f / 360;
            BuildRate = (1 + TotalTime * 0.0506f) * 1.15f / 360;

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
            BuildingTime = 0;
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
}
