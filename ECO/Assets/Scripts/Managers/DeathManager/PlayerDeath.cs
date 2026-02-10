using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float deathZoneY = -10f;
    public Vector3 lastCheckpointPosition = Vector3.zero;

    void Update()
    {
        if (transform.position.y < deathZoneY)
        {
            Die();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position + Vector3.up * 2f;
    }

    public void Die()
    {
        
        transform.position = lastCheckpointPosition;

        Checkpoint checkpoint = FindFirstObjectByType<Checkpoint>();
        if (checkpoint != null)
        {
            checkpoint.TriggerRespawn();
        }
    }
}