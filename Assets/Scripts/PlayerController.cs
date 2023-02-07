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
    private float _decelarationModifier = 0.5f;

    [SerializeField, Range(0.0f, 1.0f)]
    private float _apexModifier = 0.5f;


    [Header("Animation")]
    [SerializeField, Range(0.0f, 0.1f)]
    private float _bounceAnimDuration = 0.05f;

    [SerializeField, Range(0.0f, 0.75f)]
    private float _squeezeAmount = 0.5f;

    [SerializeField, Range(0.0f, 0.1f)]
    private float _rotateAnimDuration = 0.05f;


    private Rigidbody2D _rigidbody2D;
    private float _horizontalInput;

    private void Awake()
    {
        _polygonBuilder.Build();
        _rigidbody2D = GetComponent<Rigidbody2D>(); // Important: needs to come after the polygons were built
        _rigidbody2D.gravityScale = _gravityScale;
        gameObject.layer = _polygonBuilder.CurrentPolygon.CurrentColorLayer;
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        float accelerationForce;
        if (_horizontalInput != 0.0f)
        {
            if (_rigidbody2D.velocity.x > 0 && _horizontalInput < 0 || _rigidbody2D.velocity.x < 0 && _horizontalInput > 0)
            {
                accelerationForce = _horizontalInput * _acceleration * (1.0f + _decelarationModifier);
            }
            else
            {
                accelerationForce = _horizontalInput * _acceleration;
            }
        }
        else
        {
            accelerationForce = _rigidbody2D.velocity.x > 0 ? -_acceleration * (1.0f + _decelarationModifier) : _acceleration * (1.0f + _decelarationModifier);
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

    private void GetInput()
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        _rigidbody2D.velocity = Vector2.zero;

        var t = transform;
        t.localScale = new Vector3(1.0f, 1.0f - _squeezeAmount, 1.0f);
        t.DOScaleY(1.0f, _bounceAnimDuration).OnComplete(() =>
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.AddForce(_jumpForce * Vector2.up, ForceMode2D.Impulse);
        });
    }
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