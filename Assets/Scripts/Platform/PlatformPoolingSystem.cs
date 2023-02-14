using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformPoolingSystem : MonoBehaviour
{
    [SerializeField] private List<GameObject> _platforms;
    [SerializeField] private List<GameObject> _oneTimePlatforms;
    [SerializeField] private Transform _player;
    [SerializeField, Min(0)] private int _poolSize;
    [SerializeField, Min(0.0f)] private float _initialHeight;
    [SerializeField, Min(0.0f)] private float _spawnAheadDistance;
    [SerializeField, Min(0.0f)] private float _retrieveDistance;
    [SerializeField, Min(0.0f)] private float _spacing;
    [SerializeField, Min(0)] private int _spawnChunkSize;

    private List<Queue<GameObject>> _pools;
    private List<Transform> _poolParents;
    private int _currentLevel = 3;
    private float _lastSpawnHeight;
    private bool _isGameEnded;

    private void Awake()
    {
        CreatePlatforms();
        GameManager.Instance.GameEnded += OnGameEnded;
        GameManager.Instance.PlayerLeveledUp += OnPlayerLeveledUp;
    }

    private void OnDestroy()
    {
        GameManager.Instance.GameEnded -= OnGameEnded;
        GameManager.Instance.PlayerLeveledUp -= OnPlayerLeveledUp;
    }

    private void Start()
    {
        PlacePlatforms(_player.position.y + _initialHeight);
        StartCoroutine(CheckRetrieve());
    }

    private void Update()
    {
        if (_player.position.y + _spawnAheadDistance > _lastSpawnHeight)
            PlacePlatforms(_lastSpawnHeight + _spacing * _spawnChunkSize);
    }

    private void CreatePlatforms()
    {
        _pools = new List<Queue<GameObject>>();
        _poolParents = new List<Transform>();

        for (int i = 0; i < _platforms.Count; i++)
        {
            _pools.Add(new Queue<GameObject>(_poolSize));
            var platform = _platforms[i];
            var poolParent = new GameObject($"{platform.name} Pool").transform;
            poolParent.SetParent(transform);
            _poolParents.Add(poolParent);
            
            for (int j = 0; j < _poolSize; j++)
            {
                var go = Instantiate(platform, poolParent);
                go.SetActive(false);
                _pools[i].Enqueue(go);
            }
        }
    }

    private void PlacePlatforms(float startHeight)
    {
        Debug.Log("Spawning");
        _lastSpawnHeight = startHeight;
        for (int i = 0; i < _spawnChunkSize; i++)
        {
            var rnd = Random.Range(0, _currentLevel);
            var specialRnd = Random.Range(0.0f, 1.0f);
            var isOneTime =  specialRnd < 0.1f;
            var isTrampoline = specialRnd >= 0.1f && specialRnd < 0.2f;
            GameObject platform;
            if (isOneTime)
                platform = Instantiate(_oneTimePlatforms[rnd]);
            //TODO: add two other types of platforms
            else
                platform = _pools[rnd].Dequeue();

            platform.transform.position = new Vector3(Random.Range(-2f, 2f), startHeight + i * _spacing, 0.0f);
            platform.SetActive(true);
        }
    }

    private IEnumerator CheckRetrieve()
    {
        var waitForOneSecond = new WaitForSeconds(1f);
        while (!_isGameEnded)
        {
            for (int i = 0; i < _currentLevel; i++)
            {
                foreach (Transform platform in _poolParents[i])
                {
                    // If platform is not active, it means it is in queue
                    if (!platform.gameObject.activeSelf)
                        continue;
                    
                    // If platform is not far away enough, don't retrieve yet
                    if (platform.position.y > _player.position.y - _retrieveDistance)
                        continue;
                    
                    platform.gameObject.SetActive(false);
                    _pools[i].Enqueue(platform.gameObject);
                }
            }

            yield return waitForOneSecond;
        }
    }

    private void OnGameEnded() => _isGameEnded = true;

    private void OnPlayerLeveledUp() => _currentLevel++;
}
