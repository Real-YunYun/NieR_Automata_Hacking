using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class RoomTemplate
{
    [Header("Room Template Parameters")]
    static private string Path = "Ground/";
    static private int RoomBias = 2;

    // Bridges
    static public GameObject BUD { get { return Resources.Load<GameObject>(Path + "BUD"); } }
    static public GameObject BLR { get { return Resources.Load<GameObject>(Path + "BLR"); } }

    // Single
    static public GameObject N { get { return Resources.Load<GameObject>(Path + "N"); } }
    static public GameObject U { get { return Resources.Load<GameObject>(Path + "U"); } }
    static public GameObject D { get { return Resources.Load<GameObject>(Path + "D"); } }
    static public GameObject L { get { return Resources.Load<GameObject>(Path + "L"); } }
    static public GameObject R { get { return Resources.Load<GameObject>(Path + "R"); } }

    // Double
    static public GameObject UD { get { return Resources.Load<GameObject>(Path + "UD"); } }
    static public GameObject UL { get { return Resources.Load<GameObject>(Path + "UL"); } }
    static public GameObject UR { get { return Resources.Load<GameObject>(Path + "UR"); } }
    static public GameObject DL { get { return Resources.Load<GameObject>(Path + "DL"); } }
    static public GameObject DR { get { return Resources.Load<GameObject>(Path + "DR"); } }
    static public GameObject LR { get { return Resources.Load<GameObject>(Path + "LR"); } }

    // Triple
    static public GameObject UDL { get { return Resources.Load<GameObject>(Path + "UDL"); } }
    static public GameObject UDR { get { return Resources.Load<GameObject>(Path + "UDR"); } }
    static public GameObject ULR { get { return Resources.Load<GameObject>(Path + "ULR"); } }
    static public GameObject DLR { get { return Resources.Load<GameObject>(Path + "DLR"); } }

    // Quad
    static public GameObject UDLR { get { return Resources.Load<GameObject>(Path + "UDLR"); } }

    // Rooms (Non Alphabetical)
    static public GameObject None = N;
    static public GameObject[] UpRooms = { U, UD, UDL, UDLR, UDR, UL, ULR, UR };
    static public GameObject[] DownRooms = { D, DL, DLR, DR, UD, UDL, UDLR, UDR };
    static public GameObject[] LeftRooms = { L, DL, DLR, LR, UDL, UDLR, UL, ULR };
    static public GameObject[] RightRooms = { R, DLR, DR, LR, UDLR, UDR, ULR, UR };

    // Extra Subrooms (Bridges, Secret Rooms, etc.)
    static public GameObject[] Bridges = { BUD, BLR };

    // Starter Room Criteria
    static public GameObject[] StarterRooms = { U, UD, UL, UR, D, DL, DR, L, LR, R };

    //Random Room Getters
    static public GameObject Up
    {
        get
        {
            if (Random.Range(0, RoomBias) == 0) return U;
            else return UpRooms[Random.Range(0, UpRooms.Length)];
        }
    }

    static public GameObject Down
    {
        get
        {
            if (Random.Range(0, RoomBias) == 0) return D;
            else return DownRooms[Random.Range(0, DownRooms.Length)];
        }
    }

    static public GameObject Left
    {
        get
        {
            if (Random.Range(0, RoomBias) == 0) return L;
            else return LeftRooms[Random.Range(0, LeftRooms.Length)];
        }
    }

    static public GameObject Right
    {
        get
        {
            if (Random.Range(0, RoomBias) == 0) return R;
            else return RightRooms[Random.Range(0, RightRooms.Length)];
        }
    }

    // Random Starter Room Getter
    static public GameObject StarterRoom { get { return StarterRooms[Random.Range(0, StarterRooms.Length)]; } }

}

[System.Serializable]
public class RoomDictionary
{
    public List<RoomInformation> D = new List<RoomInformation>();
    public List<RoomInformation> DL = new List<RoomInformation>();
    public List<RoomInformation> DLR = new List<RoomInformation>();
    public List<RoomInformation> DR = new List<RoomInformation>();
    public List<RoomInformation> L = new List<RoomInformation>();
    public List<RoomInformation> LR = new List<RoomInformation>();
    public List<RoomInformation> R = new List<RoomInformation>();
    public List<RoomInformation> U = new List<RoomInformation>();
    public List<RoomInformation> UD = new List<RoomInformation>();
    public List<RoomInformation> UDL = new List<RoomInformation>();
    public List<RoomInformation> UDLR = new List<RoomInformation>();
    public List<RoomInformation> UDR = new List<RoomInformation>();
    public List<RoomInformation> UL = new List<RoomInformation>();
    public List<RoomInformation> ULR = new List<RoomInformation>();
    public List<RoomInformation> UR = new List<RoomInformation>();    
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

[DefaultExecutionOrder(1)]
public class LevelManager : MonoBehaviour
{
    [Header("Generation Parameters")]
    private RoomDictionary Dictionary;

    [Header("Level Manager Parameters")]
    public int GenerateSeed = -1;
    public bool CanStart = true;
    private GameObject StartingRoom;

    [Header("Importing/Overwriting Parameters")]
    public TextAsset RoomFile;
    public int ImportingDictionaryIndex = -1;

    [Header("Removing Parameters")]
    public RoomType RemovingRoomType = RoomType.U;
    public int RemovingDictionaryIndex = 0;


    static public void ImportRoom(TextAsset Asset)
    {
        RoomInformation ImportedInformation = JsonUtility.FromJson<RoomInformation>(Asset.ToString());
        Debug.Log("RoomType: " + ImportedInformation.RoomType);
    } 

    void Awake()
    {
        if (CanStart)
        {
            if (GenerateSeed != -1 && GenerateSeed >= 0) Random.InitState(GenerateSeed);
            StartingRoom = Instantiate(RoomTemplate.StarterRoom, Vector3.zero, Quaternion.identity);
            StartingRoom.transform.parent = transform;
        }
    }

    public void ImportInformation()
    {
        // Importing to the last of the List
        if (RoomFile == null)
        {
            Debug.LogError("Uploaded file field is null");
            return;
        }

        RoomInformation ImportingInformation = JsonUtility.FromJson<RoomInformation>(RoomFile.text);
        RoomType ImportingRoomType = ImportingInformation.RoomType;

        // Appending to Lists
        if (ImportingDictionaryIndex == -1)
        {
            // D, DL, DLR, DR, L, LR, R, U, UD, UDL, UDLR, UDR, UL, ULR, UR
            if (ImportingRoomType == RoomType.D) Dictionary.D.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.DL) Dictionary.DL.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.DLR) Dictionary.DLR.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.DR) Dictionary.DR.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.L) Dictionary.L.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.LR) Dictionary.LR.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.R) Dictionary.R.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.U) Dictionary.U.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.UD) Dictionary.UD.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.UDL) Dictionary.UDL.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.UDLR) Dictionary.UDLR.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.UDR) Dictionary.UDR.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.UL) Dictionary.UL.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.ULR) Dictionary.ULR.Add(ImportingInformation);
            else if (ImportingRoomType == RoomType.UR) Dictionary.UR.Add(ImportingInformation);

            Debug.Log("Imported Room at " + ImportingRoomType + " [" + ImportingDictionaryIndex + "]");
        }
        else if (ImportingDictionaryIndex != -1 && ImportingDictionaryIndex >= 0) OverwriteInformation();
        else Debug.LogError("Dictionary Index was Invalid");

        SaveDictionary();
    }

    public void OverwriteInformation()
    {
        RoomInformation OverwritingInformation = JsonUtility.FromJson<RoomInformation>(RoomFile.ToString());
        RoomType ImportingRoomType = OverwritingInformation.RoomType;

        if (ImportingDictionaryIndex > 0)
        {
            if (ImportingRoomType == RoomType.D) Dictionary.D.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.DL) Dictionary.DL.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.DLR) Dictionary.DLR.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.DR) Dictionary.DR.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.L) Dictionary.L.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.LR) Dictionary.LR.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.R) Dictionary.R.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.U) Dictionary.U.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.UD) Dictionary.UD.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.UDL) Dictionary.UDL.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.UDLR) Dictionary.UDLR.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.UDR) Dictionary.UDR.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.UL) Dictionary.UL.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.ULR) Dictionary.ULR.Insert(ImportingDictionaryIndex, OverwritingInformation);
            else if (ImportingRoomType == RoomType.UR) Dictionary.UR.Insert(ImportingDictionaryIndex, OverwritingInformation);

            Debug.Log("Overwriten Room at " + ImportingRoomType + " [" + ImportingDictionaryIndex + "]");
        }
        else Debug.LogError("Index for Dictionary was Invalid");
    }

    public void RemoveInformation()
    {
        if (RemovingRoomType == RoomType.D) Dictionary.D.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.DL) Dictionary.DL.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.DLR) Dictionary.DLR.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.DR) Dictionary.DR.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.L) Dictionary.L.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.LR) Dictionary.LR.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.R) Dictionary.R.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.U) Dictionary.U.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.UD) Dictionary.UD.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.UDL) Dictionary.UDL.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.UDLR) Dictionary.UDLR.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.UDR) Dictionary.UDR.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.UL) Dictionary.UL.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.ULR) Dictionary.ULR.RemoveAt(RemovingDictionaryIndex);
        else if (RemovingRoomType == RoomType.UR) Dictionary.UR.RemoveAt(RemovingDictionaryIndex);
    }

    public void SaveDictionary()
    {
        string SavingInformation = JsonUtility.ToJson(Dictionary);
        File.WriteAllText(Application.dataPath + "/Dictionary/Rooms.dictionary", SavingInformation);
    }

    public void LoadDictionary()
    {
        Dictionary = JsonUtility.FromJson<RoomDictionary>(File.ReadAllText(Application.dataPath + "/Dictionary/Rooms.dictionary"));
    }

}
