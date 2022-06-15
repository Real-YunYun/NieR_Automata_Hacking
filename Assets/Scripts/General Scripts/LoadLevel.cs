using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadLevel : Interact
{
    [Header("Interact: Load Level Parameters")]
    [SerializeField] private string LevelName = "Title";
    [SerializeField] public GameState State = GameState.Title;

    public override void Activate()
    {
        GameManager.Instance.CurrentLevel = LevelName;
        GameManager.Instance.SaveGame();
        SceneManager.LoadScene(LevelName);
    }
}
