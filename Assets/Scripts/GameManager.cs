using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action GameEnded;
    public event Action PlayerJumped;
    public event Action PlayerLeveledUp;
    public event Action PlayerDied;
    public event Action<float> SwapTriggered;
    public event Action Swapped;
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

    public void TriggerSwappingInSeconds(float seconds)
    {
        SwapTriggered?.Invoke(seconds);
        StartCoroutine(WaitToSwap());

        IEnumerator WaitToSwap()
        {
            yield return new WaitForSeconds(seconds);
            Swapped?.Invoke();
        }
    }

    public void RestartGame() => SceneManager.LoadScene(1);

}