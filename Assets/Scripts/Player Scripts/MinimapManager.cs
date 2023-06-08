using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[DefaultExecutionOrder(1)]
public class MinimapManager : MonoBehaviour
{
    [Header("Camera Parameters")]
    private Vector3 Position = new Vector3(0, 30, 0);
    private float SmoothTime = 0.25f;
    private Transform Player;
    private float VelocityX = 0.0f;
    private float VelocityY = 0.0f;
    private float VelocityZ = 0.0f;

    [Header("Minimap Parameters")]
    private bool ActiveMinimap = true;
    private Camera MiniMapCamera;
    private float[] ZoomLevels = { 30f, 40f, 60f, 75f, 100f };
    private int ZoomLevel = 1;
    private float VelocityZoom = 0.0f;
    [SerializeField] private bool CanZoom = false;
    private bool ZoomIn = false;
    private bool ZoomOut = false;

    [Header("Minimap Gameplay Parameters")]
    
    [Tooltip("Parameters to Control the Minimap to rotate to the player's movement")]
    public bool CameraRotation = false;
    
    [Tooltip("Parameter to Control the Minimap to rotate where the player is aiming")]
    public bool CameraRotationAiming = false;

    [Tooltip("Parameter to Control the Minimap to Lerp to the current room's center")]
    public bool CameraLerpCenterRoom = false;

    void Start() {
        if (GameManager.Instance.PlayerControllerInstance.Character != null)
            Player = GameManager.Instance.PlayerControllerInstance.Character.transform;
        MiniMapCamera = GetComponent<Camera>();
    }

    void Update() {
        Keyboard keyboard = Keyboard.current;

        if (keyboard.tabKey.wasPressedThisFrame) UpdateMinimap();

        if (CanZoom && !CameraLerpCenterRoom) {
            if (keyboard.equalsKey.wasPressedThisFrame) ZoomIn = true;
            if (keyboard.minusKey.wasPressedThisFrame) ZoomOut = true;
        }
        else if (CameraLerpCenterRoom) {
            CanZoom = false;
            ZoomLevel = 0;
        }
        else if (!CameraLerpCenterRoom && !CanZoom) CanZoom = true;

        MiniMapCamera.orthographicSize = Mathf.SmoothDamp(MiniMapCamera.orthographicSize, ZoomLevels[ZoomLevel], ref VelocityZoom, SmoothTime, Mathf.Infinity, Time.deltaTime);
    }

    void FixedUpdate() {
        if (CanZoom) {
            if (ZoomOut) {
                ZoomLevel++;
                if (ZoomLevel > ZoomLevels.Length - 1) ZoomLevel = 0;
                ZoomOut = false;
            }

            if (ZoomIn) {
                ZoomLevel--;
                if (ZoomLevel < 0) ZoomLevel = ZoomLevels.Length - 1;
                ZoomIn = false;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        if (!GameManager.Instance.IsGamePaused) {
            if (CameraLerpCenterRoom) {
                if (GameManager.Instance.MainCameraInstance.GetComponent<MainCamera>().CurrentRoom == null || GameManager.Instance.MainCameraInstance == null){
                    transform.position = new Vector3(0, 0, 0);
                    return;
                }

                Transform CenterRoom = GameManager.Instance.MainCameraInstance.GetComponent<MainCamera>().CurrentRoom.gameObject.transform;
                
                float NewX = Mathf.SmoothDamp(transform.position.x, CenterRoom.position.x + Position.x, ref VelocityX,
                    SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
                float NewY = Mathf.SmoothDamp(transform.position.y, CenterRoom.position.y + Position.y, ref VelocityY,
                    SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
                float NewZ = Mathf.SmoothDamp(transform.position.z, CenterRoom.position.z + Position.z, ref VelocityZ,
                    SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
                transform.position = new Vector3(NewX, NewY, NewZ);
            }
            else {
                float NewX = Mathf.SmoothDamp(transform.position.x, Player.position.x + Position.x, ref VelocityX,
                    SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
                float NewY = Mathf.SmoothDamp(transform.position.y, Player.position.y + Position.y, ref VelocityY,
                    SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
                float NewZ = Mathf.SmoothDamp(transform.position.z, Player.position.z + Position.z, ref VelocityZ,
                    SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
                transform.position = new Vector3(NewX, NewY, NewZ);
            }

            if (CameraRotation) {
                if (CameraRotationAiming) {
                    transform.rotation = Quaternion.Euler(90, Player.Find("Player Mesh").rotation.eulerAngles.y, 0);
                    transform.position = Player.position + Position;
                }
                else transform.rotation = Quaternion.Euler(90, Player.rotation.eulerAngles.y, 0);
            }
            else transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    void UpdateMinimap() {
        // Turning off Minimap
        if (ActiveMinimap) {
            GetComponent<Camera>().enabled = false;
            ActiveMinimap = false;
            GameManager.Instance.MainCameraInstance.GetComponent<MainCamera>().HandleMinimap(false);
        }

        // Turning on Minimap
        else if (!ActiveMinimap) {
            GetComponent<Camera>().enabled = true;
            ActiveMinimap = true;
            GameManager.Instance.MainCameraInstance.GetComponent<MainCamera>().HandleMinimap(true);
        }
    }
}
