using UnityEngine;

[System.Serializable]
public enum BlockType : int
{
    None,
    Default,
    Default4x4,
    Default6x6,
    Default8x8,
    Default10x10,
    Destructible,
    Danger
}

[System.Serializable]
public class Block
{
    public BlockType Type = BlockType.None;
    public Vector2Int Position = Vector2Int.zero;
}
