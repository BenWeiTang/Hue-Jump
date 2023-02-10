using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action GameEnded;
    public event Action PlayerJumped;
    public event Action PlayerLeveledUp;
    private void Awake() => Instance = this;

    public void EndGame() => GameEnded?.Invoke();
    public void LevelUp() => PlayerLeveledUp?.Invoke();
    public void PlayerJump() => PlayerJumped?.Invoke();
    public void RestartGame() => SceneManager.LoadScene(1);

}