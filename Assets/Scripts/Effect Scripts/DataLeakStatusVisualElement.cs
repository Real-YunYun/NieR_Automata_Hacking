using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLeakStatusVisualElement : MonoBehaviour {
    // Start is called before the first frame update
    void Awake() {
        transform.rotation = UnityEngine.Random.rotationUniform;
    }
}
