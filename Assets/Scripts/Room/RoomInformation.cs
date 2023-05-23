using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomInformation {

    public RoomInformation() {
        ID = 0;
        Type = RoomType.D;
    }

    public RoomInformation(int id, RoomType type) {
        ID = id;
        Type = type;
    }
    
    public int ID = 0;
    public RoomType Type = RoomType.D;
    public List<Block> Blocks = new List<Block>();
    public List<EnemyTemplate> Enemies = new List<EnemyTemplate>();
    public List<Interactable> Interactables = new List<Interactable>();
}