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
    
    [SerializeField, Tooltip("The SFX to play when Player dies")]
    private AudioClip _deathSFX;
    

    private bool _isGameEnded;

    private void Awake()
    {
        GameManager.Instance.PlayerJumped += OnPlayerJumped;
        GameManager.Instance.PlayerLeveledUp += OnPlayerLeveledUp;
        GameManager.Instance.GameEnded += OnGameEnded;
    }

    private void OnDestroy()
    {
        GameManager.Instance.PlayerJumped -= OnPlayerJumped;
        GameManager.Instance.PlayerLeveledUp -= OnPlayerLeveledUp;
        GameManager.Instance.GameEnded -= OnGameEnded;
    }

    private void LateUpdate()
    {
        if (_isGameEnded) return;
        var position = transform.position;
        position = new Vector3(position.x, Mathf.Max(position.y, _player.position.y + _followThreshold), position.z);
        transform.position = position;
    }

    private void OnPlayerJumped()
    {
        if (!_jumpSFX || !_audioSource)
            return;
        _audioSource.PlayOneShot(_jumpSFX);
    }
    
    private void OnPlayerLeveledUp()
    {
        if (!_levelUpSFX || !_audioSource)
            return;
        _audioSource.PlayOneShot(_levelUpSFX);
    }
    
    private void OnGameEnded()
    {
        _isGameEnded = true;
        
        if (!_deathSFX || !_audioSource)
            return;
        _audioSource.PlayOneShot(_deathSFX);
    }
}
