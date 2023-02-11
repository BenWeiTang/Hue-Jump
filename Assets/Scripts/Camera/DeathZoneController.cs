using System.Linq;
using UnityEngine;
public class DeathZoneController : MonoBehaviour
{

    [SerializeField] private int _maxLifeCount = 3;
    [SerializeField] private GameObject[] _lifeImages;

    private int _lifeCount;

    private void Start()
    {
        _lifeCount = _maxLifeCount;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        _lifeCount--;
        if (_lifeImages.ElementAtOrDefault(_lifeCount) != null)
            _lifeImages[_lifeCount].SetActive(false);
        Debug.Log("Lives = " + _lifeCount);
        if (_lifeCount <= 0)
        {
            GameManager.Instance.EndGame();
        }
        else
        {
            GameManager.Instance.KillPlayer();
        }
    }
}