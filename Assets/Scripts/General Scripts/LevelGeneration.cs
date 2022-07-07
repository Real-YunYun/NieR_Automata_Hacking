using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField] private GameObject[] BuildingBlocks;

    private static Vector2Int MAXDIM = new Vector2Int(5, 5);

    private GameObject[,] PlayArea = new GameObject[MAXDIM.x, MAXDIM.y];
    private bool[,] Flag = new bool[MAXDIM.x, MAXDIM.y];

    // Start is called before the first frame update
    void Awake()
    {
        //Figure out the nuleation point ground type should be
        PlayArea[2, 2] = Instantiate(
            BuildingBlocks[2],
            new Vector3(0, 0, 0),
            Quaternion.Euler(new Vector3(90, 0, 0)));

        PlayArea[1, 2] = Instantiate(
            BuildingBlocks[Random.Range(2, 7)],
            new Vector3(((2 - 2) * 50) + (5 * (2 - 2)), 0, ((3 - 2) * 50) + (5 * (3 - 2))),
            Quaternion.Euler(new Vector3(90, 0, 0)));
        PlayArea[1, 2].transform.LookAt(PlayArea[2, 2].transform);
        PlayArea[1, 2].transform.rotation = Quaternion.Euler(new Vector3(90, 0, PlayArea[1, 2].transform.rotation.z));
        
    }

}
