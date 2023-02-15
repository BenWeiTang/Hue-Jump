using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _inGamePanel;
    [SerializeField] private CanvasGroup _endGamePanel;
    [SerializeField] private Transform _camera;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [SerializeField] private GameObject[] _lifeImages;
    [SerializeField] private TextMeshProUGUI _swapText;

    private float _cameraInitialHeight;

    public void RestartGame() => GameManager.Instance.RestartGame();

    private void Start()
    {
        GameManager.Instance.GameEnded += OnGameEnded;
        GameManager.Instance.PlayerDied += OnPlayerDied;
        GameManager.Instance.SwapTriggered += OnSwapTriggered;
        _inGamePanel.alpha = 1;
        _inGamePanel.interactable = false;
        _inGamePanel.blocksRaycasts = false;
        _endGamePanel.alpha = 0;
        _endGamePanel.interactable = false;
        _endGamePanel.blocksRaycasts = false;

        _cameraInitialHeight = _camera.position.y;
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.GameEnded -= OnGameEnded;
        GameManager.Instance.PlayerDied -= OnPlayerDied;
        GameManager.Instance.SwapTriggered -= OnSwapTriggered;
    }

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

    private void OnPlayerDied()
    {
        var currentLifeCount = GameManager.Instance.CurrentLifeCount;
        if (_lifeImages.ElementAtOrDefault(GameManager.Instance.CurrentLifeCount) != null)
            _lifeImages[currentLifeCount].SetActive(false);
    }

    private void OnSwapTriggered(float seconds)
    {
        StartCoroutine(Countdown());
        
        IEnumerator Countdown()
        {
            _swapText.enabled = true;
            while (seconds > 0f)
            {
                seconds -= Time.deltaTime;
                _swapText.text = "Switch roles in " + seconds.ToString("0.00");
                yield return null;
            }
            _swapText.enabled = false;
        }
    }
}