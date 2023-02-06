using UnityEngine;

public class PlayerTurret : MonoBehaviour
{
    [Header("Turret Parameters")]
    [SerializeField] private GameObject ProjectilePrefab;
    private float SmoothTime = 0.025f;
    [HideInInspector] public Transform PlayerTurretSpawn;
    private float VelocityX = 0.0f;
    private float VelocityZ = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().OnFire += OnFire;
        //Optional Detail perhaps an upgrade?
        ProjectilePrefab = GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().GetProjectile();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            float NewX = Mathf.SmoothDamp(transform.position.x, PlayerTurretSpawn.position.x, ref VelocityX, SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            float NewY = PlayerTurretSpawn.position.y;
            float NewZ = Mathf.SmoothDamp(transform.position.z, PlayerTurretSpawn.position.z, ref VelocityZ, SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            transform.position = new Vector3(NewX, NewY, NewZ);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, 0b_1000_0000))
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

    void OnDisable()
    {
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().OnFire -= OnFire;
    }
}
