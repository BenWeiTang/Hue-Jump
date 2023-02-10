using System.Collections.Generic;
using UnityEngine;

public class PolygonBuilder : MonoBehaviour
{
    public readonly List<PolygonConfig> Polygons = new List<PolygonConfig>();

    public PolygonConfig CurrentPolygon => Polygons[_index];

    [SerializeField] private EdgeColorPalette _colorPalette;
    
    [SerializeField, Range(3, 6), Tooltip("The maximum number of edges the regular polygon can have.")]
    private int _maxPolygon = 4;

    [SerializeField, Range(0.0f, 2.0f), Tooltip("Length of the edges of the every polygon")]
    private float _edgeLength = 1.0f;

    [SerializeField, Range(-0.1f, 0.1f), Tooltip("How much should the edge move toward or away from the center of the polygon? A negative value will move the edges inward, and a positive outward")]
    private float _edgeDepthOffset = 0.0f;

    [SerializeField] private GameObject _edge;

    private int _index;

    public void Build()
    {
        BuildFoundation();
        BuildCenters();
        BuildPolygons();
        Setup();
    }

    public void LevelUp()
    {
        Polygons[_index].Center.gameObject.SetActive(false);
        _index++;
        _index = Mathf.Min(_index, _maxPolygon);
        Polygons[_index].Center.gameObject.SetActive(true);
    }

    private void BuildFoundation()
    {
        var collider2D = gameObject.AddComponent<BoxCollider2D>();
        collider2D.size = new Vector2(_edgeLength, 0.1f);
        var rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void BuildCenters()
    {
        for (int i = 3; i < _maxPolygon + 1; i++)
        {
            var go = new GameObject(i.ToString());
            var t = go.transform;
            t.SetParent(transform);
            t.rotation = Quaternion.identity;

            var halfStep = Mathf.PI / i;
            
            // Distance from center to edge (line is perpendicular)
            var height = _edgeLength / (2 * Mathf.Tan(halfStep));
            height += _edgeDepthOffset;

            // Move the center above the root Transform by height
            t.position = transform.position + height * Vector3.up;

            // Add this newly created polygon center to collection
            Polygons.Add(new PolygonConfig(i, t, _colorPalette));
        }
    }

    private void BuildPolygons()
    {
        foreach (var polygon in Polygons)
        {
            for (int i = 0; i < polygon.N; i++)
            {
                var go = Instantiate(_edge, transform.position, Quaternion.identity, polygon.Center);
                go.GetComponent<SpriteRenderer>().color = _colorPalette.EdgeColors[i].Color;
                go.transform.localScale = new Vector3(_edgeLength, 0.1f, 1.0f);
                polygon.Center.Rotate(new Vector3(0.0f, 0.0f, 2.0f * Mathf.Rad2Deg * Mathf.PI / polygon.N), Space.Self);
            }
        }
    }

    private void Setup()
    {
        _index = 0;
        foreach (var polygon in Polygons)
        {
            polygon.Center.gameObject.SetActive(false);
        }
        Polygons[_index].Center.gameObject.SetActive(true);
    }
}

public class PolygonConfig
{
    public readonly int N;
    public readonly Transform Center;
    public int CurrentColorLayer => (int)(_colorPalette.EdgeColors[_currentIndex].ColorLayer);

    private int _currentIndex;
    private readonly EdgeColorPalette _colorPalette;

    public PolygonConfig(int n, Transform center, EdgeColorPalette colorPalette)
    {
        N = n;
        Center = center;
        _currentIndex = 0;
        _colorPalette = colorPalette;
    }

    public void RotateLeft()
    {
        _currentIndex++;
        _currentIndex %= N;
    }

    public void RotateRight()
    {
        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = N - 1;
    }
}