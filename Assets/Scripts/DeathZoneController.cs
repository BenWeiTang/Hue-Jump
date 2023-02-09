using UnityEngine;

public class DeathZoneController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        
        GameManager.Instance.EndGame();
    }
}
