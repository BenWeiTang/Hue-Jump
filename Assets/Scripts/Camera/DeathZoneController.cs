using UnityEngine;
public class DeathZoneController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        if (GameManager.Instance.CurrentLifeCount <= 1)
        {
            GameManager.Instance.EndGame();
        }
        else
        {
            GameManager.Instance.KillPlayer();
        }
    }
}