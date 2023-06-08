using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class SpawnPlayer : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        GameManager.Instance.SpawnPlayer(transform);
    }
}
