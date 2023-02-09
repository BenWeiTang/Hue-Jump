using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action GameEnded; 
    private void Awake() => Instance = this;

    public void EndGame() => GameEnded?.Invoke();

    public void RestartGame() => SceneManager.LoadScene(1);
}