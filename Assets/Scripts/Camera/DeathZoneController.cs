using UnityEngine;



public class DeathZoneController : MonoBehaviour
{

    public int lives = 3;
    public GameObject[] liveImages;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            if (lives == 1)
            {
                GameManager.Instance.EndGame();
            }
            else
            {
                Debug.Log("Lives = " + lives);
                lives--;
                liveImages[lives].SetActive(false);
                col.gameObject.transform.position += Vector3.up * 5f;
            }
        }
    }
}
