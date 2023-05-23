using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class SpawnRoom : MonoBehaviour {
    public bool Spawned = false;
    public enum GroundDirection : int
    {
        Up = 1, 
        Down = 2,
        Left = 3, 
        Right = 4
    }

    [SerializeField] private GroundDirection RequiredDirection = GroundDirection.Up;

    private void Awake()
    {
        Invoke(nameof(Spawn), 0.1f);
    }
    
    private void Spawn() {
        if (!Spawned) {
            GameObject TempGameObject;
            GameObject TempBridge;
            if (RequiredDirection == GroundDirection.Up) {
                TempGameObject = Instantiate(RoomTemplate.Up, transform.position, Quaternion.identity);
                TempBridge = Instantiate(RoomTemplate.BUD, transform.position + new Vector3(0, 0, 27.5f), Quaternion.identity);
            } 
            else if (RequiredDirection == GroundDirection.Down) {
                TempGameObject = Instantiate(RoomTemplate.Down, transform.position, Quaternion.identity);
                TempBridge = Instantiate(RoomTemplate.BUD, transform.position + new Vector3(0, 0, -27.5f), Quaternion.identity);
            }
            else if (RequiredDirection == GroundDirection.Left) {
                TempGameObject = Instantiate(RoomTemplate.Left, transform.position, Quaternion.identity);
                TempBridge = Instantiate(RoomTemplate.BLR, transform.position + new Vector3(-27.5f, 0, 0), Quaternion.identity);
            }
            else {
                TempGameObject = Instantiate(RoomTemplate.Right, transform.position, Quaternion.identity);
                TempBridge = Instantiate(RoomTemplate.BLR, transform.position + new Vector3(27.5f, 0, 0), Quaternion.identity);
            }

            TempGameObject.transform.parent = GameObject.Find("Level Generation").transform;
            TempBridge.transform.parent = TempGameObject.transform;
            Spawned = true;
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Room") || other.CompareTag("Room Spawner")) Destroy(gameObject); }
}
