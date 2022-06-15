using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Level Parameters")]
    [SerializeField] public string LevelName = "Title";
    [SerializeField] public GameState State = GameState.Title;
    void Start()
    {
        GameManager.Instance.CurrentGameState = State;
        GameManager.Instance.CurrentLevel = LevelName;
    }
}
