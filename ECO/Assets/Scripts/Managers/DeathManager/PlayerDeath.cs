using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float deathZoneY = -10f;
    public Vector3 lastCheckpointPosition = Vector3.zero;

    void Start()
    {
        if (CheckpointManager.Instance != null)
        {
            lastCheckpointPosition = CheckpointManager.Instance.GetLastCheckpointPosition();
        }
        if (lastCheckpointPosition == Vector3.zero)
        {
            lastCheckpointPosition = new Vector3(0, 0, 0);
        }
        transform.position = lastCheckpointPosition;
    }

    void Update()
    {
        if (transform.position.y < deathZoneY)
        {
            Die();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
    }

    public void Die()
    {
        Checkpoint activeCheckpoint = Checkpoint.GetActiveCheckpoint();
        if (activeCheckpoint != null)
        {
            activeCheckpoint.TriggerRespawn();
        }

        StartCoroutine(RestartSceneAfterDelay(2f));
    }

    private System.Collections.IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}