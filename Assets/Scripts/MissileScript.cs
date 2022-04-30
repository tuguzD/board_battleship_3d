using UnityEngine;

public class MissileScript : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        gameManager.CheckHit(other.gameObject);
        Destroy(gameObject);
    }
}
