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
        Invoke("Spawn", 0.05f);
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
            else if (RequiredDirection == GroundDirection.Right)
            {
                Instantiate(RoomTemplate.Right, transform.position, Quaternion.identity);
                Instantiate(RoomTemplate.BLR, transform.position + new Vector3(27.5f, 0, 0), Quaternion.identity);
            }
            Spawned = true;
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
