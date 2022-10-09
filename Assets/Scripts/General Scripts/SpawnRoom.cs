using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class SpawnRoom : MonoBehaviour
{
    public bool Spawned = false;
    public enum GroundDirection : int
    {
        Up = 1, 
        Down = 2,
        Left = 3, 
        Right = 4
    }

    [SerializeField] private GroundDirection RequiredDirection = GroundDirection.Up;

    void Awake()
    {
        GenerateRoom(transform.parent.parent.gameObject);
        Invoke("Spawn", 0.05f);
    }

    static public RoomInformation BuildRoom(RoomType Type)
    {
        string JSONFile = System.IO.File.ReadAllText(Application.dataPath + "/Dictionary/Rooms.dictionary");
        RoomDictionary Dictionary = JsonUtility.FromJson<RoomDictionary>(JSONFile);
        List<RoomInformation> GottenInformation = GetList(Dictionary, Type);
        int RandomAccessor = Random.Range(0, GottenInformation.Count);
        if (GottenInformation.Count == 0) return null;
        else return GottenInformation[RandomAccessor];
    }

    static private List<RoomInformation> GetList(RoomDictionary Dictionary, RoomType Type)
    {
        if (Type == RoomType.D) return Dictionary.D;
        else if (Type == RoomType.DL) return Dictionary.DL;
        else if (Type == RoomType.DLR) return Dictionary.DLR;
        else if (Type == RoomType.DR) return Dictionary.DR;
        else if (Type == RoomType.L) return Dictionary.L;
        else if (Type == RoomType.LR) return Dictionary.LR;
        else if (Type == RoomType.R) return Dictionary.R;
        else if (Type == RoomType.U) return Dictionary.U;
        else if (Type == RoomType.UD) return Dictionary.UD;
        else if (Type == RoomType.UDL) return Dictionary.UDL;
        else if (Type == RoomType.UDLR) return Dictionary.UDLR;
        else if (Type == RoomType.UDR) return Dictionary.UDR;
        else if (Type == RoomType.UL) return Dictionary.UL;
        else if (Type == RoomType.ULR) return Dictionary.ULR;
        else if (Type == RoomType.UR) return Dictionary.UR;
        else return null;
    }

    private async void Spawn()
    {
        if (!Spawned)
        {
            if (RequiredDirection == GroundDirection.Up)
            {
                Instantiate(RoomTemplate.Up, transform.position, Quaternion.identity);
                Instantiate(RoomTemplate.BUD, transform.position + new Vector3(0, 0, 27.5f), Quaternion.identity);
            } 
            else if (RequiredDirection == GroundDirection.Down)
            {
                Instantiate(RoomTemplate.Down, transform.position, Quaternion.identity);
                Instantiate(RoomTemplate.BUD, transform.position + new Vector3(0, 0, -27.5f), Quaternion.identity);
            }
            else if (RequiredDirection == GroundDirection.Left)
            {
                Instantiate(RoomTemplate.Left, transform.position, Quaternion.identity);
                Instantiate(RoomTemplate.BLR, transform.position + new Vector3(-27.5f, 0, 0), Quaternion.identity);
            }
            else
            {
                Instantiate(RoomTemplate.Right, transform.position, Quaternion.identity);
                Instantiate(RoomTemplate.BLR, transform.position + new Vector3(27.5f, 0, 0), Quaternion.identity);
            }
            Spawned = true;
        }
    }

    private void GenerateRoom(GameObject Room)
    {
        GameObject Layout = Room.transform.Find("Layout").gameObject;
        Layout.transform.parent = Room.transform;
        RoomInformation Information = BuildRoom(Room.GetComponent<Room>().Type);

        // Instantiating GameObjects
        if (Information != null)
        {
            foreach (Block Block in Information.Blocks)
            {
                // Reference to the GameObject we're trying to create
                GameObject CreatedBlock;

                if (Block.Type == BlockType.Destructible) CreatedBlock = Instantiate(RoomBlocks.Destructible, new Vector3(Block.Position.x, 1, Block.Position.y), Quaternion.identity);
                else if (Block.Type == BlockType.Danger) CreatedBlock = Instantiate(RoomBlocks.Danger, new Vector3(Block.Position.x, 1, Block.Position.y), Quaternion.identity);
                else CreatedBlock = Instantiate(RoomBlocks.Default, new Vector3(Block.Position.x, 1, Block.Position.y), Quaternion.identity);

                CreatedBlock.transform.parent = Layout.transform;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Room") || other.CompareTag("Room Spawner"))
        {
            Destroy(gameObject);
        }
    }
}
