using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public enum RoomType { D, DL, DLR, DR, L, LR, R, U, UD, UDL, UDLR, UDR, UL, ULR, UR }

[RequireComponent(typeof(BoxCollider))]
public class Room : MonoBehaviour {
    
    [SerializeField] private bool isSpawnRoom;
    [SerializeField] private bool hasSpawnedEnemies;
    [SerializeField] public bool StarterRoom = false;

    public RoomInformation Information {
        set => _RoomInformation = value;
    }

    [SerializeField] private RoomInformation _RoomInformation;
    public RoomType Type = RoomType.D;
    private List<Enemy> Enemies = new List<Enemy>();

    // Room Entering Mechanics, not for Endless mode, for campaign
    private BoxCollider InRoomCollider;

    #region Init Functions

    #if UNITY_EDITOR || UNITY_EDITOR64
    private void OnValidate() {
        TryGetComponent(out InRoomCollider);
       
        if (!InRoomCollider) return;
        InRoomCollider.isTrigger = true;
        InRoomCollider.size = new Vector3(50, 20, 50);
        InRoomCollider.center = new Vector3(0, 5, 0);
    }
    #endif
    
    private void Awake()  {
        TryGetComponent(out InRoomCollider);
        
        if (!InRoomCollider) return;
        InRoomCollider.isTrigger = true;
        InRoomCollider.size = new Vector3(50, 20, 50);
        InRoomCollider.center = new Vector3(0, 5, 0);
        
        BuildRoom();
    }
    
    #endregion 
    
    #region Room Spawners / Handlers

    private static List<RoomInformation> GetList(RoomDictionary Dictionary, RoomType Type) {
        if (Type == RoomType.D) return Dictionary.D;
        if (Type == RoomType.DL) return Dictionary.DL;
        if (Type == RoomType.DLR) return Dictionary.DLR;
        if (Type == RoomType.DR) return Dictionary.DR;
        if (Type == RoomType.L) return Dictionary.L;
        if (Type == RoomType.LR) return Dictionary.LR;
        if (Type == RoomType.R) return Dictionary.R;
        if (Type == RoomType.U) return Dictionary.U;
        if (Type == RoomType.UD) return Dictionary.UD;
        if (Type == RoomType.UDL) return Dictionary.UDL;
        if (Type == RoomType.UDLR) return Dictionary.UDLR;
        if (Type == RoomType.UDR) return Dictionary.UDR;
        if (Type == RoomType.UL) return Dictionary.UL;
        if (Type == RoomType.ULR) return Dictionary.ULR;
        if (Type == RoomType.UR) return Dictionary.UR;
        return null;
    }

    private void BuildRoom() {
        if (StarterRoom) return;
        
        // Get Room Information for making the room
        List<RoomInformation> GottenInformation = GetList(LevelManager.Instance.Dictionary, Type);
        int RandomAccessor = Random.Range(0, GottenInformation.Count);
        if (GottenInformation.Count == 0) return;
        _RoomInformation = GottenInformation[RandomAccessor];
        
        // Creating the Rooms building blocks
        Transform Layout = transform.Find("Layout");
        Layout.transform.parent = transform;

        // Instantiating GameObjects
        if (_RoomInformation != null) {
            foreach (Block Block in _RoomInformation.Blocks) {
                GameObject CreatedBlock = Instantiate(Resources.Load<GameObject>("Building Blocks/" + Block.Name), Block.Position + transform.position, Quaternion.identity);
                CreatedBlock.name = Block.Name + " (Clone)";
                CreatedBlock.transform.parent = Layout;
            }
        }
    }

    private void SpawnEnemies() {
        hasSpawnedEnemies = true;
        if (StarterRoom) return;
        Transform Layout = transform.Find("Layout");

        Debug.Log("Spawned Enemies");
        if (_RoomInformation.Enemies.Count == 0) return;
        
        //Create Enemies
        foreach (EnemyTemplate EnemyToSpawn in _RoomInformation.Enemies) {
            Enemy CreatedEnemy = Instantiate(Resources.Load<Enemy>("Enemies/" + EnemyToSpawn.Name), EnemyToSpawn.Location + transform.position, Quaternion.Euler(EnemyToSpawn.Rotation));
            CreatedEnemy.name = EnemyToSpawn.Name + " (Clone)";
            CreatedEnemy.transform.parent = Layout;
        }
    }
    
    #endregion

    #region Handling Room Enteries and Exits

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject != GameManager.Instance.PlayerInstance) return;
        if (!hasSpawnedEnemies) SpawnEnemies();
    }

    #endregion

}
