using System;
using System.Collections.Generic;
using UnityEngine;
using Entities.Enemies;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class Room : MonoBehaviour {
    [SerializeField] public bool StarterRoom = false;

    public RoomInformation Information {
        set => _RoomInformation = value;
        get => _RoomInformation;
    }

    [SerializeField] private RoomInformation _RoomInformation;
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
        
        if (!StarterRoom) BuildRoom();
    }
    
    #endregion 
    
    #region Room Spawners / Handlers

    private void BuildRoom() {
        // Get Room Information for making the room
        List<RoomInformation> GottenInformation = LevelManager.Instance.Dictionary.GetList(_RoomInformation.Type);
        int RandomAccessor = Random.Range(0, GottenInformation.Count);
        if (GottenInformation.Count == 0) return;
        _RoomInformation = GottenInformation[RandomAccessor];
        
        // Creating the Rooms building blocks
        Transform Layout = transform.Find("Layout");
        Layout.transform.parent = transform;

        // Instantiating GameObjects
        if (_RoomInformation != null) {
            foreach (Block Block in _RoomInformation.Blocks) {
                GameObject CreatedBlock = Instantiate(Resources.Load<GameObject>("Building Blocks/" + Block.Name), Block.Position + transform.position, Quaternion.Euler(Block.Rotation));
                CreatedBlock.name = Block.Name + " (Clone)";
                CreatedBlock.transform.parent = Layout;
            }
        }
    }

    private void SpawnEnemies() {
        _RoomInformation.HasSpawnedEnemies = true;
        if (StarterRoom) return;
        Transform Layout = transform.Find("Layout");

        if (_RoomInformation.Enemies.Count == 0) return;
        
        // should rooms be randomly spawn enemies? perhaps have a toggle? so best of both worlds?
        
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
        if (other.gameObject != GameManager.Instance.PlayerControllerInstance.Character && GameManager.Instance.MainCameraInstance) return;
        GameManager.Instance.MainCameraInstance.GetComponent<MainCamera>().UpdateRoomInformation(_RoomInformation, this);

        if (!_RoomInformation.HasSpawnedEnemies) SpawnEnemies();
    }

    #endregion

}
