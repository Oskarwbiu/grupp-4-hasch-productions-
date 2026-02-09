using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float deathZoneY = -10f;

    void Update()
    {
        if (transform.position.y < deathZoneY)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        transform.position = Checkpoint.lastCheckpointPosition;
    }
}