using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using Entities;
using Entities.Enemies;

#if UNITY_EDITOR
public class RoomBuilder : MonoBehaviour {

    [Header("Auto Handles")]
    
    [Tooltip("This will make any prefabs auto unpacked and then deleted from the RoomBuilder to continuing importing the *.room data")]
    public bool PrefabUnpacking = false;

    [Tooltip("Creates 2 versions of backups, one with pre-saved Dictionary, along with the same data just saved as the *.backup format")]
    public bool BackupDictionaries = true;
    
    [Header("Unique Identifiers")]
    [SerializeField] int RoomID = 0;
    [SerializeField] RoomType RoomType = RoomType.U;
    [SerializeField] RoomSpecialType SpecialType = RoomSpecialType.None;
    
    [Header("Room Gameplay Variables")]
    public bool ChanceToSpawnEnemies = false;
    public List<string> SpecialRoomArguments = new List<string>();

    [Header("Importing Room Parameters")]
    [SerializeField] public int ImportingRoomID;
    [SerializeField] public RoomType ImportingRoomType;

    public RoomInformation Save() {
        RoomInformation CurrentRoom = new RoomInformation();
        CurrentRoom.ID = RoomID;
        CurrentRoom.Type = RoomType;
        CurrentRoom.SpecialType = SpecialType;

        CurrentRoom.ClearedRoom = false;
        CurrentRoom.HasSpawnedEnemies = false;
        CurrentRoom.ChanceToSpawnEnemies = ChanceToSpawnEnemies;
        CurrentRoom.SpecialRoomArguments = SpecialRoomArguments;
        
        Transform Layout = transform.Find("Layout");
        RoomID++;
        
        //Getting All Children
        for (int i = 0; i < Layout.childCount; ++i) {
            // Unpack the GameObject if it's a prefab!
            if (PrefabUtility.GetPrefabInstanceHandle(Layout.GetChild(i).gameObject) != null) 
                PrefabUtility.UnpackPrefabInstance(Layout.GetChild(i).gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            
            // If such object we want to save is an "Entity"
            if (Layout.GetChild(i).gameObject.GetComponent<Entity>() != null) {
                // Checking if this child is an Enemy
                if (Layout.GetChild(i).gameObject.GetComponent<Enemy>() != null) {
                    Enemy TempEnemy = Layout.GetChild(i).gameObject.GetComponent<Enemy>();
                    EnemyTemplate SavingEnemyTemplate = new EnemyTemplate();
                    
                    // Filtering the name!
                    string[] TempEnemyName = TempEnemy.name.Split(" ");
                    if (TempEnemyName[^1] == "(Clone)" || TempEnemyName[^1].Substring(0, 1) == "(") TempEnemyName[^1] = "";

                    // Adding the string back together and adding our separator, then removing the last separator 
                    string FilteredName = "";
                    foreach (string JoiningString in TempEnemyName)
                        FilteredName += JoiningString != "" ? JoiningString + " " : JoiningString;
                    SavingEnemyTemplate.Name = FilteredName.Remove(FilteredName.Length - 1);

                    // Where's it's transform? more like Location and Rotation!
                    SavingEnemyTemplate.Location = TempEnemy.transform.position;
                    SavingEnemyTemplate.Rotation = TempEnemy.transform.rotation.eulerAngles;
                    
                    // Saving the Enemy to the Map!
                    CurrentRoom.Enemies.Add(SavingEnemyTemplate);
                }
                
                // We know it's a block!
                else {
                    Block TempBlock = new Block();

                    // Filtering the name!
                    string[] TempBlockName = Layout.GetChild(i).gameObject.name.Split(" ");
                    if (TempBlockName[^1] == "(Clone)" || TempBlockName[^1].Substring(0, 1) == "(") TempBlockName[^1] = "";

                    // Adding the string back together and adding our separator, then removing the last separator 
                    string FilteredName = "";
                    foreach (string JoiningString in TempBlockName) 
                        FilteredName += JoiningString != "" ? JoiningString + " " : JoiningString;
                    TempBlock.Name = FilteredName.Remove(FilteredName.Length - 1);

                    // We can use a Vector 2 since we always know our cubes are going to be at y = 1
                    TempBlock.Position = new Vector3Int(
                        (int)Layout.GetChild(i).gameObject.transform.position.x,
                        1,
                        (int)Layout.GetChild(i).gameObject.transform.position.z
                    );

                    TempBlock.Rotation = Layout.GetChild(i).gameObject.transform.rotation.eulerAngles;

                    CurrentRoom.Blocks.Add(TempBlock);
                }
            }
        }

        return CurrentRoom;
    }

    public void ClearLayout() {
        // Getting All Children
        Transform Layout = transform.Find("Layout");

        // Clearing Existing Layout
        GameObject[] LayoutChildren = new GameObject[Layout.childCount];
        for (int i = 0; i < Layout.childCount; ++i) LayoutChildren[i] = Layout.GetChild(i).gameObject;

        foreach (GameObject Child in LayoutChildren) {
            if (PrefabUtility.GetPrefabInstanceHandle(Child) != null) {
                if (PrefabUnpacking) PrefabUtility.UnpackPrefabInstance(Child, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                else Debug.LogError("Tried Deleting Prefab, remove or unpack all prefabs present or use AutoHandlePrefabUnpacking");
            }
            else DestroyImmediate(Child);
        }
    }

    public void Filter() {
        RoomDictionary Dictionary = JsonUtility.FromJson<RoomDictionary>(File.ReadAllText(Application.dataPath + "/Dictionary/Rooms.dictionary"));
        Dictionary.FilterAllDictionaries();
    }
    
    public void Import() {
        // Getting Information
        // Read File then get the data!
        RoomDictionary Dictionary = JsonUtility.FromJson<RoomDictionary>(File.ReadAllText(Application.dataPath + "/Dictionary/Rooms.dictionary"));
        RoomInformation ImportedInformation = Dictionary.GetRoomInformation(ImportingRoomID, ImportingRoomType);
        
        // If A valid Index!
        if (ImportedInformation == null) {
            Debug.LogError("Could not load Dictionary or Invalid Index of Importing Room");
            return;
        }

        // Getting All Children
        Transform Layout = transform.Find("Layout");

        // Clearing Existing Layout
        GameObject[] LayoutChildren = new GameObject[Layout.childCount];
        for (int i = 0; i < Layout.childCount; ++i) LayoutChildren[i] = Layout.GetChild(i).gameObject;

        foreach (GameObject Child in LayoutChildren) {
            if (PrefabUtility.GetPrefabInstanceHandle(Child) != null) {
                if (PrefabUnpacking) PrefabUtility.UnpackPrefabInstance(Child, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                else Debug.LogError("Tried Deleting Prefab, remove or unpack all prefabs present or use AutoHandlePrefabUnpacking");
            }
            else DestroyImmediate(Child);
        }

        // Instantiating GameObjects
        
        // Creating Blocks
        foreach (Block Block in ImportedInformation.Blocks) {
            GameObject CreatedBlock = Instantiate(Resources.Load<GameObject>("Building Blocks/" + Block.Name), Block.Position, Quaternion.Euler(Block.Rotation));
            CreatedBlock.name = Block.Name + " (Clone)";
            CreatedBlock.transform.parent = Layout;
        }

        //Creating Enemies
        foreach (EnemyTemplate EnemyToSpawn in ImportedInformation.Enemies) {
            Enemy CreatedEnemy = Instantiate(Resources.Load<Enemy>("Enemies/" + EnemyToSpawn.Name), EnemyToSpawn.Location, Quaternion.Euler(EnemyToSpawn.Rotation));
            CreatedEnemy.name = EnemyToSpawn.Name + " (Clone)";
            CreatedEnemy.transform.parent = Layout;
        }
        
        Debug.ClearDeveloperConsole();
        Debug.Log("Successfully Imported Room { ID: "+ ImportingRoomID + ", Room Type: " + ImportingRoomType + " }");
    }
}
#endif
