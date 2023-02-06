using System;
using Threads;
using UnityEngine;
using Threads.Obritals;

public class PointerOrbital : Orbital
{
    protected new float RotationalSpeed = 15.0f;

    protected override void Awake()
    {
        Stats.Name = "Pointer Orbital!";
        Stats.Description = "Summons a permanent Pointer Orbital!";
        Stats.Sprite = "Player/UI Images/None";
        Stats.Duration = 0f;
        Stats.Cooldown = 0f;
        Stats.Upkeep = 0f;
    }

    void OnEnable()
    {
        OrbitingPrefab = Instantiate(Resources.Load<GameObject>("Building Blocks/Danger Cube"));
        OrbitingPrefab.transform.position = transform.position + new Vector3(Range, 0.0f, 0.0f);
    }

    protected void Update()
    {
        base.Update();
    }
}