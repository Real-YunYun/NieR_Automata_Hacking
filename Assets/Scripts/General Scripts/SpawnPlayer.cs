using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SpawnPlayer(transform);
    }
}
