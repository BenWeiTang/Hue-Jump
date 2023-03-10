using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PolygonBuilder _polygonBuilder;

    [Header("Vertical Movement")]
    [SerializeField, Range(0.0f, 20.0f)]
    private float _jumpForce = 10;

    [SerializeField, Range(0.0f, 20.0f)]
    private float _maxFallSpeed = 10;

    [SerializeField, Range(0.0f, 5.0f)]
    private float _gravityScale = 1.0f;

    [SerializeField, Range(0.0f, 10.0f)]
    private float _downwardAccelerationModifier = 1.0f;

    [Header("Horizontal Movement")]
    [SerializeField, Range(0.0f, 20.0f)]
    private float _maxMoveSpeed = 10;

    [SerializeField, Range(0.0f, 20.0f)]
    private float _acceleration = 10;

    [SerializeField, Range(0.0f, 1.0f)]
    private float _decelerationModifier = 0.5f;

    [SerializeField, Range(0.0f, 1.0f)]
    private float _apexModifier = 0.5f;

    [SerializeField, Range(0.0f, 5.0f), Tooltip("The speed at which Player moves horizontally when waiting for resuming gameplay.")]
    private float _resumeMoveSpeed = 2.0f;


    [Header("Animation")]
    [SerializeField, Range(0.0f, 0.1f)]
    private float _bounceAnimDuration = 0.05f;

    [SerializeField, Range(0.0f, 0.75f)]
    private float _squeezeAmount = 0.5f;

    [SerializeField, Range(0.0f, 0.1f)]
    private float _rotateAnimDuration = 0.05f;

    [Header("Levels")]
    [SerializeField, Tooltip("The vertical distance Player needs to travel to level up to Square")]
    private float _squareThreshold;
    [SerializeField, Tooltip("The vertical distance Player needs to travel to level up to Pentagon. Must be greater than Square Threshold")]
    private float _pentagonThreshold;
    [SerializeField, Tooltip("The vertical distance Player needs to travel to level up to Hexagon. Must be greater than Pentagon Threshold")]
    private float _hexagonThreshold;


    private Rigidbody2D _rigidbody2D;
    private float _horizontalInput;
    private int _currentLevel = 3;
    private bool _isWaitingForResume;
    private bool _isInputSwapped;

    private void Awake()
    {
        _polygonBuilder.Build();
        _rigidbody2D = GetComponent<Rigidbody2D>(); // Important: needs to come after the polygons were built
        _rigidbody2D.gravityScale = _gravityScale;
        gameObject.layer = _polygonBuilder.CurrentPolygon.CurrentColorLayer;
    }

    private void Start()
    {
        GameManager.Instance.GameEnded += OnGameEnded;
        GameManager.Instance.PlayerDied += OnPlayerDied;
        GameManager.Instance.Swapped += OnSwapped;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameEnded -= OnGameEnded;
        GameManager.Instance.PlayerDied -= OnPlayerDied;
        GameManager.Instance.Swapped -= OnSwapped;
    }

    private void Update()
    {
        GetInput();
        CheckLevelUp();
    }

    private void FixedUpdate()
    {
        if (!_isWaitingForResume)
            NormalMove();
        else
            ResumeMove();
    }

    private void NormalMove()
    {
        float accelerationForce;
        if (_horizontalInput != 0.0f)
        {
            if (_rigidbody2D.velocity.x > 0 && _horizontalInput < 0 || _rigidbody2D.velocity.x < 0 && _horizontalInput > 0)
                accelerationForce = _horizontalInput * _acceleration * (1.0f + _decelerationModifier);
            else
                accelerationForce = _horizontalInput * _acceleration;
        }
        else
        {
            if (_rigidbody2D.velocity.x < 0.01f)
                accelerationForce = 0.0f;
            else
                accelerationForce = _rigidbody2D.velocity.x > 0 ? -_acceleration * (1.0f + _decelerationModifier) : _acceleration * (1.0f + _decelerationModifier);
        }

        _rigidbody2D.AddForce(Vector2.right * accelerationForce);
        _rigidbody2D.gravityScale = _rigidbody2D.velocity.y > 0 ? _gravityScale : _gravityScale * (1 + _downwardAccelerationModifier);
        var velocity = _rigidbody2D.velocity;
        var xVelocity = velocity.x;
        var yVelocity = velocity.y;
        xVelocity = Mathf.Abs(yVelocity) < 0.1f ? xVelocity * (1.0f + _apexModifier) : xVelocity;
        xVelocity = math.clamp(xVelocity, -_maxMoveSpeed, _maxMoveSpeed);
        yVelocity = math.max(yVelocity, -_maxFallSpeed);
        _rigidbody2D.velocity = new Vector2(xVelocity, yVelocity);
    }

    private void ResumeMove()
    {
        var current = _rigidbody2D.position;
        _rigidbody2D.MovePosition(current + Vector2.right * (_resumeMoveSpeed * _horizontalInput * Time.fixedDeltaTime));
    }

    private void GetInput()
    {
        if (!_isInputSwapped)
        {
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                _horizontalInput = -1.0f;
            else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
                _horizontalInput = 1.0f;
            else
                _horizontalInput = 0.0f;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                RotatePolygon(RotationDirection.Left);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                RotatePolygon(RotationDirection.Right);
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                _horizontalInput = -1.0f;
            else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
                _horizontalInput = 1.0f;
            else
                _horizontalInput = 0.0f;
            
            if (Input.GetKeyDown(KeyCode.A))
                RotatePolygon(RotationDirection.Left);
            else if (Input.GetKeyDown(KeyCode.D))
                RotatePolygon(RotationDirection.Right);
        }
        
        if (_isWaitingForResume && Input.GetKeyDown(KeyCode.Space))
            Resume();
    }

    private void RotatePolygon(RotationDirection direction)
    {
        var polygon = _polygonBuilder.CurrentPolygon;
        var step = 2 * Mathf.PI / polygon.N;
        step *= Mathf.Rad2Deg;
        if (direction == RotationDirection.Left)
        {
            polygon.Center.DOLocalRotate(new Vector3(0.0f, 0.0f, step), _rotateAnimDuration, RotateMode.LocalAxisAdd);
            polygon.RotateLeft();
        }
        else
        {
            polygon.Center.DOLocalRotate(new Vector3(0.0f, 0.0f, -step), _rotateAnimDuration, RotateMode.LocalAxisAdd);
            polygon.RotateRight();
        }
        gameObject.layer = polygon.CurrentColorLayer;
    }

    private void CheckLevelUp()
    {
        var currentHeight = _rigidbody2D.position.y;
        if (currentHeight > _squareThreshold && _currentLevel == 3 ||
            currentHeight > _pentagonThreshold && _currentLevel == 4 ||
            currentHeight > _hexagonThreshold && _currentLevel == 5)
        {
            _currentLevel++;
            _polygonBuilder.LevelUp(gameObject.layer);
            GameManager.Instance.LevelUp();
        }
    }

    private void Resume()
    {
        _rigidbody2D.isKinematic = false;
        _isWaitingForResume = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // If find no PlayerController, it is not a platform
        var platform = col.gameObject.GetComponent<PlatformController>();
        if (!platform)
            return;

        // Should not bounce while still moving up
        if (_rigidbody2D.velocity.y > 0.0f)
            return;
        
        _rigidbody2D.velocity = Vector2.zero;

        var platformType = platform.PlatformType;
        var jumpForceModifier = platformType == PlatformType.Trampoline ? 1.75f : 1f;
        
        // First shrink and then tween back to the normal y scale
        var t = transform;
        t.localScale = new Vector3(1.0f, 1.0f - _squeezeAmount, 1.0f);
        t.DOScaleY(1.0f, _bounceAnimDuration).OnComplete(() =>
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.AddForce(_jumpForce * jumpForceModifier * Vector2.up, ForceMode2D.Impulse);

            if (platformType == PlatformType.OneTime)
            {
                Destroy(col.gameObject);
            }
            else if (platformType == PlatformType.Swapper)
            {
                Destroy(col.gameObject);
                GameManager.Instance.TriggerSwappingInSeconds(5f);
            }
        });
        
        GameManager.Instance.PlayerJump();
    }

    private void OnGameEnded()
    {
        gameObject.SetActive(false);
    }

    private void OnPlayerDied()
    {
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.isKinematic = true;
        _rigidbody2D.position += 5f * Vector2.up;
        _isWaitingForResume = true;
    }

    private void OnSwapped() => _isInputSwapped = !_isInputSwapped;
}

public enum RotationDirection
{
    /// <summary>
    /// Counterclockwise rotation
    /// </summary>
    Left,
    /// <summary>
    /// Clockwise rotation
    /// </summary>
    Right
}