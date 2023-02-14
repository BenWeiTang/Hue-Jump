using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action GameEnded;
    public event Action PlayerJumped;
    public event Action PlayerLeveledUp;
    public event Action PlayerDied;
    public int MaxLifeCount => _maxLifeCount;
    public int CurrentLifeCount { get; private set; }
    
    [SerializeField] private int _maxLifeCount = 3;
    private void Awake()
    {
        Instance = this;
        CurrentLifeCount = _maxLifeCount;
    }

    public void EndGame() => GameEnded?.Invoke();
    public void LevelUp() => PlayerLeveledUp?.Invoke();
    public void PlayerJump() => PlayerJumped?.Invoke();
    public void KillPlayer()
    {
        CurrentLifeCount--;
        PlayerDied?.Invoke();
    }

    public void RestartGame() => SceneManager.LoadScene(1);

}