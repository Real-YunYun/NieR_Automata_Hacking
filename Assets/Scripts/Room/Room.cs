using UnityEngine;

[System.Serializable] public enum RoomType { U, UD, UDL, UDLR, UDR, UL, ULR, UR, D, DL, DLR, DR, L, LR, R }

public class Room : MonoBehaviour
{
    public RoomType Type = RoomType.D;
}
