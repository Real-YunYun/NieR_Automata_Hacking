using UnityEngine;

public class PlayerTurret : MonoBehaviour
{
    [Header("Turret Parameters")]
    [SerializeField] private GameObject ProjectilePrefab;
    private float SmoothTime = 0.025f;
    [HideInInspector] public Transform PlayerTurretSpawn;
    private Vector3 Velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().OnFireEvent += OnFire;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
        { 
            transform.position = Vector3.SmoothDamp(transform.position, PlayerTurretSpawn.position, ref Velocity, SmoothTime);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, 0b_0100_1000))
            {
                Vector3 rotation = Quaternion.LookRotation(raycastHit.point - transform.position, Vector3.up).eulerAngles;
                rotation.x = rotation.z = 0;
                gameObject.transform.rotation = Quaternion.Euler(rotation);
            }
        }
    }

    public void OnFire()
    {
        Instantiate(ProjectilePrefab, transform.Find("Projectile Spawn").position, transform.Find("Projectile Spawn").rotation);
    }

    void OnDestroy()
    {
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().OnFireEvent -= OnFire;
    }
}
