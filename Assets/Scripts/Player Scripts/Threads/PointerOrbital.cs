using UnityEngine;
using Threads;

public class PointerOrbital : Orbitals
{
    // Start is called before the first frame update
    protected void Awake()
    {
        OrbitingPrefab = Instantiate(Resources.Load<GameObject>("Building Blocks/Danger Cube"), transform.parent);
    }

    protected void Update()
    {
        base.Update();
    }
}