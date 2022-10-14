using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;

[Serializable] public enum RoomType { U, UD, UDL, UDLR, UDR, UL, ULR, UR, D, DL, DLR, DR, L, LR, R }
[Serializable] public enum BlockType : int {
    None, 
    Default, 
    Default4x4, 
    Default6x6, 
    Default8x8, 
    Default10x10, 
    Destructible, 
    Danger 
}

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
    static public GameObject Default4x4 { get { return Resources.Load<GameObject>(Path + "Default Cube 4x4"); } }
    static public GameObject Default6x6 { get { return Resources.Load<GameObject>(Path + "Default Cube 6x6"); } }
    static public GameObject Default8x8 { get { return Resources.Load<GameObject>(Path + "Default Cube 8x8"); } }
    static public GameObject Default10x10 { get { return Resources.Load<GameObject>(Path + "Default Cube 10x10"); } }
    static public GameObject Destructible { get { return Resources.Load<GameObject>(Path + "Destructible Cube"); } }
    static public GameObject Danger { get { return Resources.Load<GameObject>(Path + "Danger Cube"); } }
}

#if UNITY_EDITOR
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
                    // Check Scale (Difference between Blocks
                    else if (Layout.GetChild(i).gameObject.transform.localScale.x == 10) TempBlock.Type = BlockType.Default10x10;
                    else if (Layout.GetChild(i).gameObject.transform.localScale.x == 8) TempBlock.Type = BlockType.Default8x8;
                    else if (Layout.GetChild(i).gameObject.transform.localScale.x == 6) TempBlock.Type = BlockType.Default6x6;
                    else if (Layout.GetChild(i).gameObject.transform.localScale.x == 4) TempBlock.Type = BlockType.Default4x4;
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

                if (Block.Type == BlockType.Destructible) CreatedBlock = Instantiate(RoomBlocks.Destructible, Layout);
                else if (Block.Type == BlockType.Danger) CreatedBlock = Instantiate(RoomBlocks.Danger, Layout);
                else if (Block.Type == BlockType.Default10x10) CreatedBlock = Instantiate(RoomBlocks.Default10x10, Layout);
                else if (Block.Type == BlockType.Default8x8) CreatedBlock = Instantiate(RoomBlocks.Default8x8, Layout);
                else if (Block.Type == BlockType.Default6x6) CreatedBlock = Instantiate(RoomBlocks.Default6x6, Layout);
                else if (Block.Type == BlockType.Default4x4) CreatedBlock = Instantiate(RoomBlocks.Default4x4, Layout);
                else CreatedBlock = Instantiate(RoomBlocks.Default, Layout);

                CreatedBlock.transform.parent = Layout.transform;
                CreatedBlock.transform.position = new Vector3(Block.Position.x, 1, Block.Position.y);
                CreatedBlock.transform.rotation = Quaternion.identity;
            }
        }
        else return null;

        return ImportedInformation;
    }
}
#endif