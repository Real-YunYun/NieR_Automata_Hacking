using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [Header("Level Parameters")]
    [SerializeField] public string LevelName;
    [SerializeField] public GameState State = GameState.Title;
    void Start()
    {
        GameManager.Instance.CurrentGameState = State;
        GameManager.Instance.CurrentLevel = LevelName;
    }
}
