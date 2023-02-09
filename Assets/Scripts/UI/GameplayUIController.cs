using System;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _inGamePanel;
    [SerializeField] private CanvasGroup _endGamePanel;

    public void RestartGame() => GameManager.Instance.RestartGame();
    private void Awake() => GameManager.Instance.GameEnded += OnGameEnded;
    private void OnDestroy() => GameManager.Instance.GameEnded -= OnGameEnded;

    private void Start()
    {
        _inGamePanel.alpha = 1;
        _inGamePanel.interactable = false;
        _inGamePanel.blocksRaycasts = false;
        _endGamePanel.alpha = 0;
        _endGamePanel.interactable = false;
        _endGamePanel.blocksRaycasts = false;
    }

    private void OnGameEnded()
    {
        _inGamePanel.alpha = 0;
        _endGamePanel.alpha = 1;
        _endGamePanel.interactable = true;
        _endGamePanel.blocksRaycasts = true;
    }
}