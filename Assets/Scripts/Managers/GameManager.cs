using System;
using System.IO;
using Entities;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

public enum GameState {
    Title   = 0b_0000_0000,
    Pause   = 0b_0100_0001,
    Playing = 0b_1000_0010,
    InGame  = 0b_0000_0011,
    Options = 0b_0000_0100,
    Load    = 0b_0110_0101,
    Exiting = 0b_0101_0110,
}

[DefaultExecutionOrder(-1)]
public class GameManager : SingletonPersistent<GameManager> {
    [Header("Game Manager Parameters")]
    public string CurrentLevel = "HUB";
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject DirectorPrefab;
    
    [Header("Game State Parameters")]
    public bool IsGamePaused = false;
    public bool UseDirector = false;
    public bool PossibleSaveGame = true;
    public float DifficultyModifier = 2.25f; //Easy: 1f, Normal: 1.5f, Hard: 2.25f
    public int StangeCount = 0;
    public int GenerationSeed = -1;
    
    [Header("Player Instances")]
    public PlayerController PlayerControllerInstance;
    public GameObject PlayerInstance;
    public GameObject MainCameraInstance;
    public GameObject DirectorInstance;

    //Game State Variables
    public GameState CurrentGameState { get; set; }

    //Game Manager Parameters
    public PlayerData Data;
    private static readonly string key = "43286579693697635478639456395002084630948674278724";

    // Start is called before the first frame update
    protected override void Awake() {
        //if (SceneManager.GetActiveScene().name != "Title Screen") SpawnPlayer(transform);
        base.Awake();
        GarbageCollector.incrementalTimeSliceNanoseconds = 25000;
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
        //PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
        //PlayerSettings.useFlipModelSwapchain = false;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
        if (CurrentLevel == "Title Screen") CurrentGameState = GameState.Title;
    }

    public void PlayGame() {
        CurrentGameState = GameState.Playing;
        CurrentLevel = "HUB";
        SceneManager.LoadScene("Loading");
    }

    public void SaveGame() {
        //Getting Data
        Data.CurrentDate = DateTime.Now.ToString("f");

        //Showing Data
        string playerDataJSON = EncryptDecrypt(JsonUtility.ToJson(Data)); 
        File.WriteAllText(Application.dataPath + "/Saves/0.sav", playerDataJSON);
    }
    
    public void LoadGame() {
        //re-Loading Game
        if (IsGamePaused) PauseGame();
        CurrentGameState = GameState.Playing;
        SceneManager.LoadScene("HUB");
    }

    public void PauseGame() {
        IsGamePaused = !IsGamePaused;
        if (IsGamePaused) {
            CurrentGameState = GameState.Pause;
            Time.timeScale = 0;
        }
        else {
            if (CurrentLevel == "Title") CurrentGameState = GameState.Title;
            else CurrentGameState = GameState.Playing;
            Time.timeScale = 1;
        }
    }

    public void SpawnPlayer(Transform spawnLocation) {
        PlayerControllerInstance = Instantiate(Resources.Load<GameObject>("Player/Player Controller"), spawnLocation.position, spawnLocation.rotation).GetComponent<PlayerController>() ;
        PlayerInstance = Instantiate(PlayerPrefab, spawnLocation.position, spawnLocation.rotation);
        PlayerControllerInstance.Posses(PlayerInstance.GetComponent<Entity>()); 
        MainCameraInstance = Instantiate(MainCamera, MainCamera.transform.position, MainCamera.transform.rotation);
        MainCameraInstance.GetComponent<MainCamera>().Player = PlayerInstance.transform;

        switch (SceneManager.GetActiveScene().name) {
            case "Loading":
            case "Main Game":
                if (!UseDirector) break;
                if (DirectorInstance) DirectorInstance.GetComponent<Director>().enabled = true;
                else {
                    DirectorInstance = Instantiate(DirectorPrefab, new Vector3(0, 10, 0), Quaternion.identity);
                    DirectorInstance.transform.parent = transform;
                }
                break;

            case "Title":
            case "HUB":
                if (DirectorInstance) Destroy(DirectorInstance);
                break;
        }

        //Loading the Data to the new spawned Player
        //Getting Save File
        string playerDataJSON = File.ReadAllText(Application.dataPath + "/Saves/0.sav");
        JsonUtility.FromJsonOverwrite(EncryptDecrypt(playerDataJSON), Data);
    }

    private static string EncryptDecrypt(string data) {
        string result = "";
        for (int i = 0; i < data.Length; i++) { result += (char)(data[i] ^ key[i % key.Length]); }
        return result;
    }

    public void ExitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void PromptInteract(string Message = "\0", bool state = false)
    {
        if (Message != "\0") MainCameraInstance.GetComponent<MainCamera>().InteractText.GetComponent<Text>().text = "[E]: " + Message;
        else MainCameraInstance.GetComponent<MainCamera>().InteractText.GetComponent<Text>().text = "[E]: Use";

        MainCameraInstance.GetComponent<MainCamera>().InteractText.SetActive(state);
    }

}