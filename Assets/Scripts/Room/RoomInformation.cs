using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomInformation
{
    public RoomType RoomType = RoomType.U;
    public List<Block> Blocks = new List<Block>();
    public List<Enemy> Enemies = new List<Enemy>();
}