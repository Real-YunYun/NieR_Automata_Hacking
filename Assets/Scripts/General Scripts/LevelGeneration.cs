using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    [Header("Level Generation Parameters")]
    private static Vector2Int MAXDIM = new Vector2Int(13, 13); // X * Y = # CELLS
    private static Vector2Int NucleationPoint = new Vector2Int((MAXDIM.x - 1) / 2, (MAXDIM.y - 1) / 2);
    private static Vector2Int InstantiateOffsets = new Vector2Int(55, 55); //X & Y are how far apart will each Cell be from one another
    private GameObject[,] PlayArea = new GameObject[MAXDIM.x, MAXDIM.y];

    private static Vector3 TranslateToWorld(int x, int z)
    {
        return new Vector3(InstantiateOffsets.x * (x - ((MAXDIM.x - 1) / 2)), 0, InstantiateOffsets.y * (z - ((MAXDIM.y - 1) / 2)));
    }


    void Awake()
    {
        
    }
}
