using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField, Tooltip("A reference to the Player in the level")] private Transform _player;
    
    [Header("Movement")]
    [SerializeField, Range(0.0f, 3.0f), Tooltip("How early show the camera follow the player")]
    private float _followThreshold;
    
    [Header("Audio")]
    [SerializeField, Tooltip("The Audio Source Responsible for playing SFXs")]
    private AudioSource _audioSource;
    
    [SerializeField, Tooltip("The SFX to play when Player lands on a platform")]
    private AudioClip _jumpSFX;
    
    [SerializeField, Tooltip("The SFX to play when Player levels up")]
    private AudioClip _levelUpSFX;
    
    [SerializeField, Tooltip("The SFX to play when the game is over")]
    private AudioClip _gameOverSFX;

    [SerializeField, Tooltip("The SFX to play when Player touches the Death Zone")]
    private AudioClip _deathSFX;
    
    private bool _isGameEnded;

    private void Start()
    {
        GameManager.Instance.PlayerJumped += OnPlayerJumped;
        GameManager.Instance.PlayerLeveledUp += OnPlayerLeveledUp;
        GameManager.Instance.GameEnded += OnGameEnded;
        GameManager.Instance.PlayerDied += OnPlayerDied;
    }

    private void OnDestroy()
    {
        GameManager.Instance.PlayerJumped -= OnPlayerJumped;
        GameManager.Instance.PlayerLeveledUp -= OnPlayerLeveledUp;
        GameManager.Instance.GameEnded -= OnGameEnded;
        GameManager.Instance.PlayerDied -= OnPlayerDied;
    }

    private void LateUpdate()
    {
        if (_isGameEnded) return;
        var position = transform.position;
        position = new Vector3(position.x, Mathf.Max(position.y, _player.position.y + _followThreshold), position.z);
        transform.position = position;
    }

    private void OnPlayerJumped() => PlaySFX(_jumpSFX);

    private void OnPlayerLeveledUp() => PlaySFX(_levelUpSFX);
    
    private void OnGameEnded()
    {
        _isGameEnded = true;
        PlaySFX(_gameOverSFX);
    }

    private void OnPlayerDied() => PlaySFX(_deathSFX);

    private void PlaySFX(AudioClip audioClip)
    {
        if (!_audioSource || !audioClip)
            return;
        
        if (_audioSource.isPlaying)
            _audioSource.Stop();
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }
}
