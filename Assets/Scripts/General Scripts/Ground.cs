using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum GroundType
{
    End = 0_0000,
    Elbow = 0b_1010,
    PassBy = 0b_1100,
    Tri = 0b_1110,
    Quad = 0b_1111,
    Bridge,
    Elevation,
    None
}
public class Ground : MonoBehaviour
{
    public GroundType Type;

    public Ground(GroundType type = GroundType.None)
    {
        Type = type;
    }
}
