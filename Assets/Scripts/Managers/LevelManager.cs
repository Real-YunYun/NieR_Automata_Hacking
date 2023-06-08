using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RoomLoader {
    [Header("Room Template Parameters")]
    private static string Path;
    private static float DeadEndChance = 0.33f;
    private static float StraightRoomChance = 0.25f;
    
    #region Static Resources for Loading

    // Bridges
    public static GameObject BUD => Resources.Load<GameObject>(Path + "BUD");
    public static GameObject BLR => Resources.Load<GameObject>(Path + "BLR");

    // Single
    public static GameObject N => Resources.Load<GameObject>(Path + "N");
    public static GameObject U => Resources.Load<GameObject>(Path + "U");
    public static GameObject D => Resources.Load<GameObject>(Path + "D");
    public static GameObject L => Resources.Load<GameObject>(Path + "L");
    public static GameObject R => Resources.Load<GameObject>(Path + "R");

    // Double
    public static GameObject UD => Resources.Load<GameObject>(Path + "UD");
    public static GameObject UL => Resources.Load<GameObject>(Path + "UL");
    public static GameObject UR => Resources.Load<GameObject>(Path + "UR");
    public static GameObject DL => Resources.Load<GameObject>(Path + "DL");
    public static GameObject DR => Resources.Load<GameObject>(Path + "DR");
    public static GameObject LR => Resources.Load<GameObject>(Path + "LR");

    // Triple
    public static GameObject UDL => Resources.Load<GameObject>(Path + "UDL");
    public static GameObject UDR => Resources.Load<GameObject>(Path + "UDR");
    public static GameObject ULR => Resources.Load<GameObject>(Path + "ULR");
    public static GameObject DLR => Resources.Load<GameObject>(Path + "DLR");

    // Quad
    public static GameObject UDLR => Resources.Load<GameObject>(Path + "UDLR");
    
    #endregion

    // Rooms (Non Alphabetical)
    public static GameObject None;
    public static GameObject[] UpRooms;
    public static GameObject[] DownRooms;
    public static GameObject[] LeftRooms;
    public static GameObject[] RightRooms;

    // Extra Subrooms (Bridges, Secret Rooms, etc.)
    public static GameObject[] Bridges;

    // Starter Room Criteria
    private static GameObject[] StarterRooms;

    static RoomLoader() {
        Path = "Ground/";
        None = N;
        UpRooms = new[] { U, UD, UDL, UDLR, UDR, UL, ULR, UR };
        DownRooms = new[] { D, DL, DLR, DR, UD, UDL, UDLR, UDR };
        LeftRooms = new[] { L, DL, DLR, LR, UDL, UDLR, UL, ULR };
        RightRooms = new[] { R, DLR, DR, LR, UDLR, UDR, ULR, UR };
        Bridges = new[] { BUD, BLR };
        StarterRooms = new[] { D, DL, DLR, DR, L, LR, R, U, UD, UDL, UDLR, UDR, UL, ULR, UR };
    }

    //Random Room Getters
    public static GameObject Up {
        get {
            if (Random.Range(0.0f, 1.0f) <= StraightRoomChance) return UD;
            if (Random.Range(0.0f, 1.0f) <= DeadEndChance) return U;
            return UpRooms[Random.Range(0, UpRooms.Length)];
        }
    }

    public static GameObject Down {
        get {
            if (Random.Range(0.0f, 1.0f) <= StraightRoomChance) return UD;
            if (Random.Range(0.0f, 1.0f) <= DeadEndChance) return D;
            return DownRooms[Random.Range(0, DownRooms.Length)];
        }
    }

    public static GameObject Left {
        get {
            if (Random.Range(0.0f, 1.0f) <= StraightRoomChance) return LR;
            if (Random.Range(0.0f, 1.0f) <= DeadEndChance) return L;
            return LeftRooms[Random.Range(0, LeftRooms.Length)];
        }
    }

    public static GameObject Right {
        get {
            if (Random.Range(0.0f, 1.0f) <= StraightRoomChance) return LR;
            if (Random.Range(0.0f, 1.0f) <= DeadEndChance) return R;
            return RightRooms[Random.Range(0, RightRooms.Length)];
        }
    }
}

[System.Serializable]
public class RoomDictionary {
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
    
    public void SaveDictionary(bool FilterAll = false) {
        if (!FilterAll) FilterAllDictionaries();
        string SavingInformation = JsonUtility.ToJson(this, true); 
        File.WriteAllText(Application.dataPath + "/Dictionary/Rooms.dictionary", SavingInformation);
    }

    public RoomInformation Traverse(List<RoomInformation> List, int FindID) {
        RoomInformation FoundRoom = List.Find(FoundRoom => FoundRoom.ID == FindID);
        return FoundRoom;
    }

    public RoomInformation GetStarterRoomInformation() {
        RoomType RandomType = (RoomType)Enum.GetValues(typeof(RoomType)).GetValue((int)Random.Range(0f, 14f));
        return GetList(RandomType)[^1];
    }

    public List<RoomInformation> GetList(RoomType Type) {
        List<RoomInformation> ReturnList;
        
        if (Type == RoomType.D) ReturnList = D;
        else if (Type == RoomType.DL) ReturnList = DL;
        else if (Type == RoomType.DLR) ReturnList = DLR;
        else if (Type == RoomType.DR) ReturnList = DR;
        else if (Type == RoomType.L) ReturnList = L;
        else if (Type == RoomType.LR) ReturnList = LR;
        else if (Type == RoomType.R) ReturnList = R;
        else if (Type == RoomType.U) ReturnList = U;
        else if (Type == RoomType.UD) ReturnList = UD;
        else if (Type == RoomType.UDL) ReturnList = UDL;
        else if (Type == RoomType.UDLR) ReturnList = UDLR;
        else if (Type == RoomType.UDR) ReturnList = UDR;
        else if (Type == RoomType.UL) ReturnList = UL;
        else if (Type == RoomType.ULR) ReturnList = ULR;
        else ReturnList = UR;

        return ReturnList;
    }

    public void AddToDictionary(RoomInformation Room) {
        List<RoomInformation> List = GetList(Room.Type);

        if (Room.ID == 0) {
            Debug.LogError($"Can not add Room to ID [0]");
            return;
        }

        if (!(Room.Blocks.Count > 0) && !(Room.Enemies.Count > 0)) {
            Debug.LogError($"Could not add Room To dictionary since it's empty!");
            return;
        }

        // Checking for Conflicts and overwriting them!
        for (int i = 0; i < List.Count; i++) {
            if (List[i].ID == Room.ID) {
                Debug.LogWarning($"Conflict with pre-existing Room ID [{Room.ID.ToString()}]");
                List[i] = Room;
                return;
            }
        }

        List.Add(Room);
        FilterDictionary(List, Room.Type);
    }

    public RoomInformation GetRoomInformation(int RoomID, RoomType Type) {
        List<RoomInformation> List = new List<RoomInformation>();

        if (Type == RoomType.D) List = D;
        else if (Type == RoomType.DL) List = DL;
        else if (Type == RoomType.DLR) List = DLR;
        else if (Type == RoomType.DR) List = DR;
        else if (Type == RoomType.L) List = L;
        else if (Type == RoomType.LR) List = LR;
        else if (Type == RoomType.R) List = R;
        else if (Type == RoomType.U) List = U;
        else if (Type == RoomType.UD) List = UD;
        else if (Type == RoomType.UDL) List = UDL;
        else if (Type == RoomType.UDLR) List = UDLR;
        else if (Type == RoomType.UDR) List = UDR;
        else if (Type == RoomType.UL) List = UL;
        else if (Type == RoomType.ULR) List = ULR;
        else List = UR;

        return Traverse(List, RoomID);
    }

    public void FilterDictionary(List<RoomInformation> DirtyList, RoomType Type) {
        // For Comparison
        Dictionary<int, RoomInformation> CleanedListDictionary = new Dictionary<int, RoomInformation>();
        List<RoomInformation> RoomsToRemove = new List<RoomInformation>();

        foreach (RoomInformation DirtyRoom in DirtyList) {
            if (!(DirtyRoom.Blocks.Count > 0) && !(DirtyRoom.Enemies.Count > 0)) 
                continue;
            
            CleanedListDictionary.TryAdd(DirtyRoom.ID, DirtyRoom);
        }

        // Adding Empty Room in case!
        if (!CleanedListDictionary.ContainsKey(0))
            CleanedListDictionary.Add(0,  new RoomInformation(0, Type));
        
        // Generate a List from Dictionary
        List<RoomInformation> CleanedList = CleanedListDictionary.Values.ToList();

        // Adding this List back into the Dictionary
        if (Type == RoomType.D) D = CleanedList;
        if (Type == RoomType.DL) DL = CleanedList;
        if (Type == RoomType.DLR) DLR = CleanedList;
        if (Type == RoomType.DR) DR = CleanedList;
        if (Type == RoomType.L) L = CleanedList;
        if (Type == RoomType.LR) LR = CleanedList;
        if (Type == RoomType.R) R = CleanedList;
        if (Type == RoomType.U) U = CleanedList;
        if (Type == RoomType.UD) UD = CleanedList;
        if (Type == RoomType.UDL) UDL = CleanedList;
        if (Type == RoomType.UDLR) UDLR = CleanedList;
        if (Type == RoomType.UDR) UDR = CleanedList;
        if (Type == RoomType.UL) UL = CleanedList;
        if (Type == RoomType.ULR) ULR = CleanedList;
        if (Type == RoomType.UR) UR = CleanedList;

        // Saving the new Dictionary!
        File.WriteAllText(Application.dataPath + "/Dictionary/Rooms.dictionary", JsonUtility.ToJson(this, true));
    }

    public void FilterAllDictionaries() {
        FilterDictionary(D, RoomType.D);
        FilterDictionary(DL, RoomType.DL);
        FilterDictionary(DLR, RoomType.DLR);
        FilterDictionary(DR, RoomType.DR);
        FilterDictionary(L, RoomType.L);
        FilterDictionary(LR, RoomType.LR);
        FilterDictionary(R, RoomType.R);
        FilterDictionary(U, RoomType.U);
        FilterDictionary(UD, RoomType.UD);
        FilterDictionary(UDL, RoomType.UDL);
        FilterDictionary(UDLR, RoomType.UDLR);
        FilterDictionary(UDR, RoomType.UDR);
        FilterDictionary(UL, RoomType.UL);
        FilterDictionary(ULR, RoomType.ULR);
        FilterDictionary(UR, RoomType.UR);
    }
}

[DefaultExecutionOrder(1), System.Serializable]
public class LevelManager : SingletonPersistent<LevelManager> {
    [Header("Generation Parameters")] 
    [SerializeField] private RoomDictionary _Dictionary = new RoomDictionary();
    public RoomDictionary Dictionary { get { return _Dictionary; } }

    [Header("Level Manager Parameters")] 
    public int GenerateSeed = -1;
    public bool CanStart = true;
    private GameObject StartingRoom;

    private void OnValidate() {
        BuildDictionary();
    }

    protected override void Awake() {
        base.Awake();
        BuildDictionary();
        
        if (CanStart) {
            if (GenerateSeed != -1 && GenerateSeed >= 0) Random.InitState(GenerateSeed);
            RoomInformation StartingRoomInformation = Dictionary.GetStarterRoomInformation();
            StartingRoom = Instantiate(Resources.Load<GameObject>("Ground/" + StartingRoomInformation.Type), Vector3.zero, Quaternion.identity);
            StartingRoom.GetComponent<Room>().Information = StartingRoomInformation;
            StartingRoom.GetComponent<Room>().StarterRoom = true;
            StartingRoom.transform.parent = transform;
            StartingRoom.name = "Starting Room";
        }
    }
    
    private void BuildDictionary() {
        string JSONFile = System.IO.File.ReadAllText(Application.dataPath + "/Dictionary/Rooms.dictionary");
        _Dictionary = JsonUtility.FromJson<RoomDictionary>(JSONFile);
    }

    // Try a requesting System for generating levels
    
}
