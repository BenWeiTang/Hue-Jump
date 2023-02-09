using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField, Range(0.0f, 3.0f), Tooltip("How early show the camera follow the player.")]
    private float _followThreshold;

    private bool _gameEnded;

    private void Awake() => GameManager.Instance.GameEnded += OnGameEnded;

    private void OnDestroy() => GameManager.Instance.GameEnded -= OnGameEnded;

    private void OnGameEnded() => _gameEnded = true;

    private void LateUpdate()
    {
        if (_gameEnded) return;
        var position = transform.position;
        position = new Vector3(position.x, Mathf.Max(position.y, _player.position.y + _followThreshold), position.z);
        transform.position = position;
    }
}
