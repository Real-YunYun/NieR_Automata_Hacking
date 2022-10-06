using UnityEngine;

public static class RoomTemplate
{
    static private string Path = "Ground/";
    
    // Single
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
    static public GameObject[] UpRooms = { U, UD, UDL, UDLR, UDR, UL, ULR, UR };
    static public GameObject[] DownRooms = { D, DL, DLR, DR, UD, UDL, UDLR, UDR };
    static public GameObject[] LeftRooms = { L, DL, DLR, LR, UDL, UDLR, UL, ULR };
    static public GameObject[] RightRooms = { R, DLR, DR, LR, UDLR, UDR, ULR, UR };

    //Random Room Getters
    static public GameObject Up { get { return UpRooms[Random.Range(0, UpRooms.Length)]; } }
    static public GameObject Down { get { return DownRooms[Random.Range(0, DownRooms.Length)]; } }
    static public GameObject Left { get { return LeftRooms[Random.Range(0, LeftRooms.Length)]; } }
    static public GameObject Right { get { return RightRooms[Random.Range(0, RightRooms.Length)]; } }
}


public class SpawnRoom : MonoBehaviour
{
    private bool Spawned = false;
    public enum GroundDirection : int
    {
        Up = 1, 
        Down = 2,
        Left = 3, 
        Right = 4
    }

    [SerializeField] private GroundDirection Direction = GroundDirection.Up;

    void Awake()
    {
        
    }
}
