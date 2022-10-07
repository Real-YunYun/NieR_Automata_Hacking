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

    //Random Starter Room Getter
    static public GameObject StarterRoom { get { return StarterRooms[Random.Range(0, StarterRooms.Length)]; } }

}

[DefaultExecutionOrder(1)]
public class LevelManager : MonoBehaviour
{
    [Header("Level Manager Parameters")]
    private bool Started = false;
    private bool BossRoom = false;
    private bool ItemRoom = false;
    private bool GearRoom = false;
    private int GenerateSeed = 7689;
    private GameObject StartingRoom;

    void Awake()
    {
        Random.InitState(GenerateSeed);
        StartingRoom = Instantiate(RoomTemplate.StarterRoom, Vector3.zero, Quaternion.identity);
        StartingRoom.transform.parent = transform;
        Started = true;
    }

}
