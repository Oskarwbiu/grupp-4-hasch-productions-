using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    private static Checkpoint activeCheckpoint;
    [SerializeField] private int checkpointID;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (CheckpointManager.Instance != null && CheckpointManager.Instance.IsCheckpointActivated(checkpointID))
        {
            animator.SetTrigger("Activate");
            activeCheckpoint = this;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && CheckpointManager.Instance != null && !CheckpointManager.Instance.IsCheckpointActivated(checkpointID))
        {
            activeCheckpoint = this;
            CheckpointManager.Instance.ActivateCheckpoint(checkpointID, transform.position);

            PlayerDeath playerDeath = collision.GetComponent<PlayerDeath>();
            if (playerDeath != null)
            {
                playerDeath.SetCheckpoint(transform.position);
                animator.SetTrigger("Activate");
            }
        }
    }

    public static Checkpoint GetActiveCheckpoint()
    {
        return activeCheckpoint;
    }

    public void TriggerRespawn()
    {
        StartCoroutine(TriggerAfterRespawnSequence());
    }

    private IEnumerator TriggerAfterRespawnSequence()
    {
        animator.SetTrigger("Respawn");
        yield return new WaitForSeconds(1.5f);
        animator.SetTrigger("AfterRespawn");
    }
}