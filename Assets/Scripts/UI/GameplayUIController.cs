using TMPro;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _inGamePanel;
    [SerializeField] private CanvasGroup _endGamePanel;
    [SerializeField] private Transform _camera;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    
    private float _cameraInitialHeight;

    public void RestartGame() => GameManager.Instance.RestartGame();

    private void Start()
    {
        GameManager.Instance.GameEnded += OnGameEnded;
        _inGamePanel.alpha = 1;
        _inGamePanel.interactable = false;
        _inGamePanel.blocksRaycasts = false;
        _endGamePanel.alpha = 0;
        _endGamePanel.interactable = false;
        _endGamePanel.blocksRaycasts = false;

        _cameraInitialHeight = _camera.position.y;
    }
    
    private void OnDestroy() => GameManager.Instance.GameEnded -= OnGameEnded;

    private void Update()
    {
        _scoreText.text = (_camera.position.y - _cameraInitialHeight).ToString("0.00");
    }

    private void OnGameEnded()
    {
        _inGamePanel.alpha = 0;
        _endGamePanel.alpha = 1;
        _endGamePanel.interactable = true;
        _endGamePanel.blocksRaycasts = true;
        _finalScoreText.text = _scoreText.text;
    }
}