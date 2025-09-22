using System.Collections;
using System.Collections.Generic;
using Entities.Status;
using UnityEngine;

public class MarkedStatusVisualEffect : MonoBehaviour {

    [Header("Marked Visual Effect Parameters")]
    [SerializeField, Tooltip("How fast is this rotating, in degrees...")]
    private float RotationalSpeed = 45.0f;
    private StatusInfliction MarkedInfliction;

    public void Initialize(StatusInfliction SI) => MarkedInfliction = SI;

    // Update is called once per frame
    void Update() {
        transform.localEulerAngles += new Vector3(0, RotationalSpeed * Time.deltaTime, 0);
    }
}