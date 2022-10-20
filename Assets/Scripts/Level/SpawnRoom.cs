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

#pragma warning disable
    private async void Spawn()
    {
        if (!Spawned)
        {
            GameObject TempGameObject;
            GameObject TempBridge;
            if (RequiredDirection == GroundDirection.Up)
            {
                TempGameObject = Instantiate(RoomTemplate.Up, transform.position, Quaternion.identity);
                TempBridge = Instantiate(RoomTemplate.BUD, transform.position + new Vector3(0, 0, 27.5f), Quaternion.identity);
            } 
            else if (RequiredDirection == GroundDirection.Down)
            {
                TempGameObject = Instantiate(RoomTemplate.Down, transform.position, Quaternion.identity);
                TempBridge = Instantiate(RoomTemplate.BUD, transform.position + new Vector3(0, 0, -27.5f), Quaternion.identity);
            }
            else if (RequiredDirection == GroundDirection.Left)
            {
                TempGameObject = Instantiate(RoomTemplate.Left, transform.position, Quaternion.identity);
                TempBridge = Instantiate(RoomTemplate.BLR, transform.position + new Vector3(-27.5f, 0, 0), Quaternion.identity);
            }
            else
            {
                TempGameObject = Instantiate(RoomTemplate.Right, transform.position, Quaternion.identity);
                TempBridge = Instantiate(RoomTemplate.BLR, transform.position + new Vector3(27.5f, 0, 0), Quaternion.identity);
            }

            TempGameObject.transform.parent = GameObject.Find("Level Generation").transform;
            TempBridge.transform.parent = TempGameObject.transform;
            Spawned = true;
        }
    }
#pragma warning restore

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
                GameObject CreatedBlock;

                if (Block.Type == BlockType.Destructible) CreatedBlock = Instantiate(RoomBlocks.Destructible, Layout.transform);
                else if (Block.Type == BlockType.Danger) CreatedBlock = Instantiate(RoomBlocks.Danger, Layout.transform);
                else if (Block.Type == BlockType.Default10x10) CreatedBlock = Instantiate(RoomBlocks.Default10x10, Layout.transform);
                else if (Block.Type == BlockType.Default8x8) CreatedBlock = Instantiate(RoomBlocks.Default8x8, Layout.transform);
                else if (Block.Type == BlockType.Default6x6) CreatedBlock = Instantiate(RoomBlocks.Default6x6, Layout.transform);
                else if (Block.Type == BlockType.Default4x4) CreatedBlock = Instantiate(RoomBlocks.Default4x4, Layout.transform);
                else CreatedBlock = Instantiate(RoomBlocks.Default, Layout.transform);

                CreatedBlock.transform.parent = Layout.transform;
                CreatedBlock.transform.position = new Vector3(Block.Position.x + Room.transform.position.x, 1, Block.Position.y + Room.transform.position.z);
                CreatedBlock.transform.rotation = Quaternion.identity;
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
