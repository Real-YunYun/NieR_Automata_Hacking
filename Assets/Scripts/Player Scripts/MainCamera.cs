using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Items;
using Items.Executables;
using Items.Threads;
using Entities;
using TMPro;


[DefaultExecutionOrder(10)]
public class MainCamera : MonoBehaviour {
    [Header("Camera Parameters")]
    public Vector3 Position = new Vector3(0, 12, -10);
    public float SmoothTime = 0.25f;
    public Transform Player;
    private float VelocityX = 0.0f;
    private float VelocityY = 0.0f;
    private float VelocityZ = 0.0f;

    private bool ExitingGame = false;

    public Room CurrentRoom;

    [Header("Interface Parameters")]
    private Canvas UI_Canvas;
    private GameObject UI_HUD_Canvas;
    private GameObject UI_Menu_Canvas;
    private GameObject UI_Load_Canvas;
    private GameObject UI_Exit_Canvas;
    private Image[] UI_Executables_Canvas = new Image[4];
    private GameObject UI_Minimap;
    
    [HideInInspector] public GameObject InteractText;

    void Awake() {
        //Getting UI Children
        UI_Canvas = transform.Find("UI Canvas").GetComponent<Canvas>();
        InteractText = transform.Find("UI Canvas/InteractText").gameObject;
        UI_HUD_Canvas = transform.Find("UI Canvas/HUD Canvas").gameObject;
        UI_Menu_Canvas = transform.Find("UI Canvas/Menu Canvas").gameObject;
        UI_Load_Canvas = transform.Find("UI Canvas/Menu Canvas/Load Game Canvas").gameObject;
        UI_Exit_Canvas = transform.Find("UI Canvas/Menu Canvas/Exit Canvas").gameObject;
        UI_Minimap = transform.Find("UI Canvas/HUD Canvas/MiniMap").gameObject;
        for (int i = 0; i < UI_Executables_Canvas.Length; i++) UI_Executables_Canvas[i] = transform.Find("UI Canvas/HUD Canvas/Executables/Executable " + (i + 1)).GetComponent<Image>();

        transform.Find("UI Canvas/HUD Canvas/Debug Messages").gameObject.SetActive(true);
    }

    private void OnEnable() {
        //Events and Delegates
        if (GameManager.Instance == null) return;
        
        GameManager.Instance.PlayerControllerInstance.PlayerCharacter.OnCharacterDisabled += OnCharacterDisable;
        
        ExectuableComponent ExecutableComponent = GameManager.Instance.PlayerControllerInstance.PlayerExecutableComponent;
        if (ExecutableComponent != null) {
            ExecutableComponent.OnExecutableUsed += OnExecutableUsed;
            ExecutableComponent.OnExecutableAdded += OnExecutableAdded;
        }
    }

    private void Start() {
        //Events and Delegates
        ExectuableComponent ExecutableComponent = GameManager.Instance.PlayerControllerInstance.PlayerExecutableComponent;
        if (ExecutableComponent != null) {
            ExecutableComponent.OnExecutableUsed += OnExecutableUsed;
            ExecutableComponent.OnExecutableAdded += OnExecutableAdded;
        }
    }

    private void OnCharacterDisable() {
        //Events and Delegates
        ExectuableComponent ExecutableComponent = GameManager.Instance.PlayerControllerInstance.PlayerExecutableComponent;
        if (ExecutableComponent != null) {
            ExecutableComponent.OnExecutableUsed -= OnExecutableUsed;
            ExecutableComponent.OnExecutableAdded -= OnExecutableAdded;
        }
        GameManager.Instance.PlayerControllerInstance.PlayerCharacter.OnCharacterDisabled -= OnCharacterDisable;
    }

    // Update is called once per frame
    void Update() {
        if (!GameManager.Instance.IsGamePaused) {
            float NewX = Mathf.SmoothDamp(transform.position.x, Player.position.x + Position.x, ref VelocityX, SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            float NewY = Mathf.SmoothDamp(transform.position.y, Player.position.y + Position.y, ref VelocityY, SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            float NewZ = Mathf.SmoothDamp(transform.position.z, Player.position.z + Position.z, ref VelocityZ, SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            transform.position = new Vector3(NewX, NewY, NewZ);
        }

        if (CurrentRoom) {
            transform.Find("UI Canvas/HUD Canvas/Debug Messages/Debug Text").GetComponent<TMP_Text>().text =
                "FPS: " + Mathf.RoundToInt(1f / Time.deltaTime) + "\n" +
                "Room ID: " + CurrentRoom.Information.ID + "\n" +
                "Room Type: " + CurrentRoom.Information.Type + "\n" +
                "Room Special Type: " + CurrentRoom.Information.SpecialType + "\n";
        }
    }

    void LateUpdate() {
        HandleUI();
    }

    public void UpdateRoomInformation(RoomInformation Information, Room NewCurrentRoom = null) {
        if (NewCurrentRoom) CurrentRoom = NewCurrentRoom;
    }

    public void HandleUI(GameState gameState = GameState.Title) {
        if (GameManager.Instance.CurrentGameState == GameState.Title) {
            UI_HUD_Canvas.SetActive(false);
            UI_Menu_Canvas.SetActive(false);
            UI_Load_Canvas.SetActive(false);
            UI_Exit_Canvas.SetActive(false);
        }
        if (GameManager.Instance.CurrentGameState == GameState.Pause) {
            UI_HUD_Canvas.SetActive(false);
            UI_Menu_Canvas.SetActive(true);
            UI_Load_Canvas.SetActive(false);
            UI_Exit_Canvas.SetActive(false);

            //Handling UI Buttons
            transform.Find("UI Canvas/Menu Canvas/Parent/Save Game Button").GetComponent<Button>().interactable = GameManager.Instance.CurrentLevel == "HUB";
            transform.Find("UI Canvas/Menu Canvas/Parent/Exit to Lobby Button").GetComponent<Button>().interactable = GameManager.Instance.CurrentLevel != "HUB";
        }
        if (GameManager.Instance.CurrentGameState == GameState.Load) {
            UI_HUD_Canvas.SetActive(false);
            UI_Menu_Canvas.SetActive(true);
            UI_Load_Canvas.SetActive(true);
            UI_Exit_Canvas.SetActive(false);
        }
        if (GameManager.Instance.CurrentGameState == GameState.Playing) {
            UI_HUD_Canvas.SetActive(true);
            UI_Menu_Canvas.SetActive(false);
            UI_Load_Canvas.SetActive(false);
            UI_Exit_Canvas.SetActive(false);

            //Handling Health Bar
            string Health = "";
            for (int i = 1; i <= Player.gameObject.GetComponent<HealthComponent>().Health; i++) Health += "/";
            UI_HUD_Canvas.transform.Find("Bars/Health Bar").GetComponent<Text>().text = Health;

            //Handling Bits Text
            UI_HUD_Canvas.transform.Find("Bits/Text").GetComponent<Text>().text = "Bits: " + GameManager.Instance.Data.Bits.ToString();

            //Handling Bounty Text
            if (GameManager.Instance.DirectorInstance) {
                UI_HUD_Canvas.transform.Find("Bounty").gameObject.SetActive(true);
                Bounty CurrentBounty = GameManager.Instance.DirectorInstance.GetComponent<Director>().GetBounty();
                string Objective = "";
                string TimeRemaining = "";

                if (CurrentBounty.Completed) UI_HUD_Canvas.transform.Find("Bounty/Text").GetComponent<Text>().color = new Color32(73, 216, 24, 255);
                else if (CurrentBounty.Expired) UI_HUD_Canvas.transform.Find("Bounty/Text").GetComponent<Text>().color = new Color32(216, 41, 24, 255);
                else UI_HUD_Canvas.transform.Find("Bounty/Text").GetComponent<Text>().color = new Color32(50, 50, 50, 255);

                if (CurrentBounty.Type == BountyType.Kill) Objective = CurrentBounty.Type + ": " + CurrentBounty.Current + "/" + CurrentBounty.Target + " Entites\n";
                if (CurrentBounty.Type == BountyType.Earn) Objective = CurrentBounty.Type + ": " + CurrentBounty.Current + " Bits/" + CurrentBounty.Target + " Bits\n";

                if (!CurrentBounty.Expired && !CurrentBounty.Completed) TimeRemaining = "Time Remaining: " + (CurrentBounty.Duration - CurrentBounty.DeltaTime).ToString("n0") + "s\n";
                else if (CurrentBounty.Completed) TimeRemaining = "Completed Objective";
                else if (CurrentBounty.Expired) TimeRemaining = "Expired Objective";

                UI_HUD_Canvas.transform.Find("Bounty/Text").GetComponent<Text>().text =
                    Objective +
                    "Reward: " + CurrentBounty.Reward + " Bits\n" +
                    TimeRemaining;
            }
            else UI_HUD_Canvas.transform.Find("Bounty").gameObject.SetActive(false);

            if (GameManager.Instance.PlayerControllerInstance == null) return;

            //Handling Abilities 
            ExectuableComponent PlayerExectuableComponent;
            if (GameManager.Instance.PlayerControllerInstance.PlayerCharacter.TryGetComponent(out PlayerExectuableComponent)) {
                if (PlayerExectuableComponent.Executables[0].OnCooldown) UpdateExecutable(0); 
                if (PlayerExectuableComponent.Executables[1].OnCooldown) UpdateExecutable(1); 
                if (PlayerExectuableComponent.Executables[2].OnCooldown) UpdateExecutable(2); 
                if (PlayerExectuableComponent.Executables[3].OnCooldown) UpdateExecutable(3);
            }
        }
        
        if (GameManager.Instance.CurrentGameState == GameState.Exiting) {
            UI_HUD_Canvas.SetActive(false);
            UI_Menu_Canvas.SetActive(true);
            UI_Load_Canvas.SetActive(false);
            UI_Exit_Canvas.SetActive(true);
        }
    }

    private void OnExecutableUsed(int slot) {
        UI_Executables_Canvas[slot].transform.Find("Fill").GetComponent<Image>().fillAmount = 1;
    }

    private void UpdateExecutable(int slot) {
        if (GameManager.Instance.PlayerControllerInstance.PlayerExecutableComponent != null) {
            Item item = GameManager.Instance.PlayerControllerInstance.PlayerExecutableComponent.Executables[slot];
            UI_Executables_Canvas[slot].transform.Find("Fill").GetComponent<Image>().fillAmount = 1 - (item.Upkeep / item.Cooldown);
        }
    }

    private void OnExecutableAdded(int slot) {
        if (GameManager.Instance.PlayerControllerInstance.PlayerExecutableComponent != null)
            UI_Executables_Canvas[slot].sprite = GameManager.Instance.PlayerControllerInstance.PlayerExecutableComponent.Executables[slot].GetSprite();
    }

    public void Resume() {
        GameManager.Instance.PauseGame();
    }

    //Saving Handle
    public void SaveGame() {
        GameManager.Instance.SaveGame();
    }

    public void HandleMinimap(bool state) {
        UI_Minimap.SetActive(state);
    }

    //Loading Handles
    public void PromptLoadGame() {
        GameManager.Instance.CurrentGameState = GameState.Load;
    }

    public void LoadGame(bool state) {
        if (state) GameManager.Instance.LoadGame();
        else GameManager.Instance.CurrentGameState = GameState.Pause;
    }

    //Exiting Game Handles
    public void PromptExit(bool ExitGame = true) {
        if (ExitGame) ExitingGame = true;
        else ExitingGame = false;
        GameManager.Instance.CurrentGameState = GameState.Exiting;
    }

    public void ExitTo(bool state) {
        if (state && !ExitingGame) {
            GameManager.Instance.PauseGame();
            GameManager.Instance.LoadGame();
            GameManager.Instance.CurrentGameState = GameState.Playing;
            SceneManager.LoadScene("HUB");
        }
        else if (state && ExitingGame) GameManager.Instance.ExitGame();
        else GameManager.Instance.CurrentGameState = GameState.Pause;
    }
}
