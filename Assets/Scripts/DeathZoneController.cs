using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZoneController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        //TODO: remove later
        SceneManager.LoadScene(1);
    }
}
