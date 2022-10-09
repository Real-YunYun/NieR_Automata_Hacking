using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;

[Serializable] public enum RoomType { U, UD, UDL, UDLR, UDR, UL, ULR, UR, D, DL, DLR, DR, L, LR, R }
[Serializable] public enum BlockType : int { None = 0, Default = 1, Destructible = 2, Danger = 3 }

[Serializable]
public class Block
{
    public BlockType Type = BlockType.None;
    public Vector2Int Position = Vector2Int.zero;
}

[Serializable]
public class RoomInformation
{
    public RoomType RoomType = RoomType.U;
    public List<Block> Blocks = new List<Block>();
    public List<Enemy> Enemies = new List<Enemy>();
}

public static class RoomBlocks
{
    [Header("Room Blocks Parameters")]
    static private string Path = "Building Blocks/";

    // Blocks
    static public GameObject Default { get { return Resources.Load<GameObject>(Path + "Default Cube"); } }
    static public GameObject Destructible { get { return Resources.Load<GameObject>(Path + "Destructible Cube"); } }
    static public GameObject Danger { get { return Resources.Load<GameObject>(Path + "Danger Cube"); } }
}


public class RoomBuilder : MonoBehaviour
{

    [Header("Room Parameters")]
    [SerializeField] RoomType RoomType = RoomType.U;

    [Header("Saving Room Parameters")]
    [SerializeField] public string SavingFileName = "0";

    [Header("Importing Room Parameters")]
    [SerializeField] public string ImportingFileName = "0";

    public RoomInformation Analyze()
    {
        RoomInformation CurrentRoom = new RoomInformation();
        CurrentRoom.RoomType = RoomType;

        //Getting All Children
        Transform Layout = transform.Find("Layout");
        for (int i = 0; i < Layout.childCount; ++i)
        {
            if (Layout.GetChild(i).gameObject.GetComponent<Entity>() != null)
            {
                if (PrefabUtility.GetPrefabInstanceHandle(Layout.GetChild(i).gameObject) != null) PrefabUtility.UnpackPrefabInstance(Layout.GetChild(i).gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                CurrentRoom.RoomType = RoomType;

                Block TempBlock = new Block();

                if (Layout.GetChild(i).gameObject.CompareTag("Destructible")) TempBlock.Type = BlockType.Destructible;
                else if (Layout.GetChild(i).gameObject.CompareTag("Indestructible"))
                {
                    if (Layout.GetChild(i).gameObject.GetComponent<DangerCube>() != null) TempBlock.Type = BlockType.Danger;
                    else TempBlock.Type = BlockType.Default;
                }
                else TempBlock.Type = BlockType.None;

                // We can use a Vector 2 since we walways know our cubes are going to be at y = 1
                TempBlock.Position = new Vector2Int(
                    (int)Layout.GetChild(i).gameObject.transform.position.x, 
                    (int)Layout.GetChild(i).gameObject.transform.position.z
                    );

                CurrentRoom.Blocks.Add(TempBlock);
            }
            
        }

        return CurrentRoom;
    }

    public RoomInformation Import()
    {
        // Getting Information
        RoomInformation ImportedInformation;
        bool IsPrefabPresent = false;

        if (File.Exists(Application.dataPath + "/Tools/Rooms/" + ImportingFileName + ".txt"))
        {
            string JSONFile = File.ReadAllText(Application.dataPath + "/Tools/Rooms/" + ImportingFileName + ".txt");
            ImportedInformation = JsonUtility.FromJson<RoomInformation>(JSONFile);
        }
        else
        {
            Debug.LogError("File does not exist \"" + Application.dataPath + "/Tools/Rooms/" + ImportingFileName + ".txt\"");
            return null;
        }

        // Getting All Children
        Transform Layout = transform.Find("Layout");

        // Clearing Existing Layout
        GameObject[] LayoutChildren = new GameObject[Layout.childCount];
        for (int i = 0; i < Layout.childCount; ++i) LayoutChildren[i] = Layout.GetChild(i).gameObject;

        foreach (GameObject Child in LayoutChildren)
        {
            if (PrefabUtility.GetPrefabInstanceHandle(Child) != null)
            {
                Debug.LogError("Tried Deleteing Prefab, remove or unpack all prefabs present");
                IsPrefabPresent = true;
            }
            else DestroyImmediate(Child);
        }

        // Instantiating GameObjects
        if (!IsPrefabPresent)
        {
            foreach (Block Block in ImportedInformation.Blocks)
            {
                // Reference to the GameObject we're trying to create
                GameObject CreatedBlock;

                if (Block.Type == BlockType.Destructible) CreatedBlock = Instantiate(RoomBlocks.Destructible, new Vector3(Block.Position.x, 1, Block.Position.y), Quaternion.identity);
                else if (Block.Type == BlockType.Danger) CreatedBlock = Instantiate(RoomBlocks.Danger, new Vector3(Block.Position.x, 1, Block.Position.y), Quaternion.identity);
                else CreatedBlock = Instantiate(RoomBlocks.Default, new Vector3(Block.Position.x, 1, Block.Position.y), Quaternion.identity);

                CreatedBlock.transform.parent = Layout;
            }
        }
        else return null;

        return ImportedInformation;
    }
}