using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public class CellCriteria
    {
        private float PityFoactor = 1.25f;
        public int Sides = 0;

        public bool up = false;
        public bool down = false;
        public bool right = false;
        public bool left = false;

        public CellCriteria(bool Randomize = false)
        {
            if (Randomize)
            {
                Sides = 0;
                up = Random.Range(0, 100) < 25 * PityFoactor ? true : false;
                if (up) Sides++;
                down = Random.Range(0, 100) < 25 * PityFoactor ? true : false;
                if (down) Sides++;
                right = Random.Range(0, 100) < 25 * PityFoactor ? true : false;
                if (right) Sides++;
                left = Random.Range(0, 100) < 25 * PityFoactor ? true : false;
                if (left) Sides++;
            }
            else
            {
                up = false;
                down = false;
                right = false;
                left = false;
                Sides = 0;
            }
        }

        public CellCriteria(int up = 0, int down = 0, int right = 0, int left = 0)
        {
            this.up = up > 0 ? true : false;
            if (this.up) Sides++;
            this.down = down > 0 ? true : false;
            if (this.down) Sides++;
            this.right = right > 0 ? true : false;
            if (this.right) Sides++;
            this.left = left > 0 ? true : false;
            if (this.left) Sides++;
        }

        public static bool Compare(CellCriteria Compared, CellCriteria Comparing)
        {
            return (Compared.up == Comparing.up) && (Compared.down == Comparing.down) && (Compared.right == Comparing.right) && (Compared.left == Comparing.left);
        }

        public void ToText(Vector2Int Index)
        {
            Debug.Log($"Cell [{Index.x}, {Index.y}]: {{Up: {up}, Down: {down}, Right: {right}, Left: {left}}}");
        }

        #region Criteria Constants

        // End Criteria
        public static readonly CellCriteria[] End =
        {
            new CellCriteria(1, 0, 0, 0),   // Up (0 Degrees)
            new CellCriteria(0, 1, 0, 0),   // Down (180 Degrees)
            new CellCriteria(0, 0, 1, 0),   // Right (90 Degrees)
            new CellCriteria(0, 0, 0, 1)    // Left (270 Degrees)
        };

        // Elbow Criteria
        public static readonly CellCriteria[] Elbow =
        {
            new CellCriteria(1, 0, 1, 0),   // Up Right (0 Degrees)
            new CellCriteria(0, 1, 1, 0),   // Down Right (90 Degrees)
            new CellCriteria(0, 1, 0, 1),   // Down Left (180 Degrees)
            new CellCriteria(1, 0, 0, 1),   // Up Left (270 Degrees)
        };

        // PassBy Criteria
        public static readonly CellCriteria[] PassBy =
        {
            new CellCriteria(0, 0, 1, 1),   // Left Right (0 Degrees & 180 Degrees)
            new CellCriteria(1, 1, 0, 0),   // Up Down (90 Degrees & 270 Degrees)
        };

        // Tri Criteria
        public static readonly CellCriteria[] Tri =
        {
            new CellCriteria(1, 0, 1, 1),   // Up L|R (0 Degrees)
            new CellCriteria(1, 1, 1, 0),   // Right U|D (90 Degrees)
            new CellCriteria(0, 1, 1, 1),   // Down L|R (180 Degrees)
            new CellCriteria(1, 1, 0, 1),   // Left U|D (270 Degrees)
        };

        public static readonly CellCriteria Quad = new CellCriteria(1, 1, 1, 1);    // Any Rotation

        #endregion
    }

    public class Cell
    {
        public enum CellDirection
        {
            None, Up, Down, Right, Left
        }

        public Vector2Int Index = new Vector2Int(-1, -1);
        public CellCriteria Criteria = new CellCriteria(false);
        public CellDirection Direction = CellDirection.None;

        public Cell(Vector2Int index, CellCriteria criteria, CellDirection direction = CellDirection.None)
        {
            Index = index;
            Criteria = criteria;
            Direction = direction;
        }   

    }

    [Header("Level Generation Parameters")]
    [SerializeField] private GameObject[] BuildingBlocks;
    private static Vector2Int MAXDIM = new Vector2Int(13, 13); // X * Y = # CELLS
    private static Vector2Int NucleationPoint = new Vector2Int((MAXDIM.x - 1) / 2, (MAXDIM.y - 1) / 2);
    private static Vector2Int InstantiateOffsets = new Vector2Int(55, 55); //X & Y are how far apart will each Cell be from one another
    private GameObject[,] PlayArea = new GameObject[MAXDIM.x, MAXDIM.y];

    private static Vector3 TranslateToWorld(int x, int z)
    {
        return new Vector3(InstantiateOffsets.x * (x - ((MAXDIM.x - 1) / 2)), 0, InstantiateOffsets.y * (z - ((MAXDIM.y - 1) / 2)));
    }

    private void StartCell()
    {
        // Scouting
        CellCriteria Criteria = new CellCriteria(true);

        Cell NucleuationCell = new Cell(NucleationPoint, Criteria);

        // Loading
        PlayArea[NucleationPoint.x, NucleationPoint.y] = Instantiate(DetermineGround(NucleuationCell));
        PlayArea[NucleationPoint.x, NucleationPoint.y].transform.position = TranslateToWorld(NucleationPoint.x, NucleationPoint.y);
        PlayArea[NucleationPoint.x, NucleationPoint.y].transform.parent = this.transform;

        // Growing
        // Up
        if (Criteria.up && !(NucleationPoint.y + 1 > MAXDIM.y - 1) && !PlayArea[NucleationPoint.x, NucleationPoint.y + 1])
        {
            InstantiateCell(new Vector2Int(NucleationPoint.x, NucleationPoint.y + 1), NucleuationCell);
        }

        // Down
        if (Criteria.down && !(NucleationPoint.y - 1 < 0) && !PlayArea[NucleationPoint.x, NucleationPoint.y - 1])
        {
            InstantiateCell(new Vector2Int(NucleationPoint.x, NucleationPoint.y - 1), NucleuationCell);
        }
        // Right
        if (Criteria.right && !(NucleationPoint.x + 1 > MAXDIM.x - 1) && !PlayArea[NucleationPoint.x + 1, NucleationPoint.y])
        {
            InstantiateCell(new Vector2Int(NucleationPoint.x + 1, NucleationPoint.y), NucleuationCell);
        }

        // Left
        if (Criteria.left && !(NucleationPoint.x - 1 < 0) && !PlayArea[NucleationPoint.x - 1, NucleationPoint.y])
        {
            InstantiateCell(new Vector2Int(NucleationPoint.x - 1, NucleationPoint.y), NucleuationCell);
        }

    }

    private void InstantiateCell(Vector2Int CellIndex, Cell ParentCell)
    {
        // Scouting
        CellCriteria Criteria = new CellCriteria(true);

        Cell cell = new Cell(CellIndex, Criteria);

        // Growing
        if (Criteria.up && !(CellIndex.y + 1 > MAXDIM.y - 1) && !PlayArea[CellIndex.x, CellIndex.y + 1]) InstantiateCell(new Vector2Int(CellIndex.x, CellIndex.y + 1), cell);     // Up
        if (Criteria.down && !(CellIndex.y - 1 < 0) && !PlayArea[CellIndex.x, CellIndex.y - 1]) InstantiateCell(new Vector2Int(CellIndex.x, CellIndex.y - 1), cell);              // Down
        if (Criteria.right && !(CellIndex.x + 1 > MAXDIM.x - 1) && !PlayArea[CellIndex.x + 1, CellIndex.y]) InstantiateCell(new Vector2Int(CellIndex.x + 1, CellIndex.y), cell);  // Right
        if (Criteria.left && !(CellIndex.x - 1 < 0) && !PlayArea[CellIndex.x - 1, CellIndex.y]) InstantiateCell(new Vector2Int(CellIndex.x - 1, CellIndex.y), cell);              // Left

        // Loading
        PlayArea[CellIndex.x, CellIndex.y] = Instantiate(DetermineGround(cell));
        PlayArea[CellIndex.x, CellIndex.y].transform.position = TranslateToWorld(CellIndex.x, CellIndex.y);
        PlayArea[CellIndex.x, CellIndex.y].transform.LookAt(PlayArea[ParentCell.Index.x, ParentCell.Index.y].transform, Vector3.up);
        PlayArea[CellIndex.x, CellIndex.y].transform.parent = PlayArea[ParentCell.Index.x, ParentCell.Index.y].transform;
    }

    private GameObject DetermineGround(Cell ChildCell)
    {
        GameObject LoadedGameObject;

        //New Way
        switch (ChildCell.Criteria.Sides)
        {
            //End Cases
            case 1:
                LoadedGameObject = Resources.Load<GameObject>("Ground/End");
                return LoadedGameObject;

            // Elbow or PassBy Cases
            case 2:
                if (ChildCell.Criteria.up && ChildCell.Criteria.down || ChildCell.Criteria.right && ChildCell.Criteria.left) LoadedGameObject = Resources.Load<GameObject>("Ground/PassBy");
                else LoadedGameObject = Resources.Load<GameObject>("Ground/Elbow");
                return LoadedGameObject;

            // Tri Cases
            case 3:
                LoadedGameObject = Resources.Load<GameObject>("Ground/Tri");
                return LoadedGameObject;

            // Quad Cases
            case 4:
                LoadedGameObject = Resources.Load<GameObject>("Ground/Quad");
                return LoadedGameObject;

            default:
                ChildCell.Criteria.ToText(ChildCell.Index);
                return new();
        }
    }

    void Awake()
    {
        StartCell();
    }
}
