using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public enum RoomType { D = 0, DL = 1, DLR = 2, DR = 3, L = 4, LR = 5, R = 6, U = 7, UD = 8, UDL = 9, UDLR = 10, UDR = 11, UL = 12, ULR = 13, UR = 14 }
[Serializable] public enum RoomSpecialType { None, ItemRoom, Shop, Boss }

[Serializable]
public class RoomInformation {

    public RoomInformation() {
        ID = 0;
        Type = RoomType.D;
    }

    public RoomInformation(int id, RoomType type) {
        ID = id;
        Type = type;
    }
    
    [Header("Room Serialization Data")]
    [Header("Unique Identifiers")]
    public int ID = 0;
    public RoomType Type = RoomType.D;
    public RoomSpecialType SpecialType = RoomSpecialType.None;

    [Header("Room Gameplay Variables")] 
    public bool ClearedRoom = false;
    public bool ChanceToSpawnEnemies = false;
    public bool HasSpawnedEnemies = false;
    public List<string> SpecialRoomArguments = new List<string>();

    [Header("Room Aspects")]
    public List<Block> Blocks = new List<Block>();
    public List<EnemyTemplate> Enemies = new List<EnemyTemplate>();
    public List<Interactable> Interactables = new List<Interactable>();
}