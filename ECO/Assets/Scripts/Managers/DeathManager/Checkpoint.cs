using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    
    [SerializeField] private int checkpointID;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (CheckpointManager.Instance != null && CheckpointManager.Instance.IsCheckpointActivated(checkpointID))
        {
            if (CheckpointManager.Instance.GetLastCheckpointPosition() == transform.position)
            {
                CheckpointManager.Instance.CurrentActiveInstance = this;
                animator.SetTrigger("Activate");

            }
            
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && CheckpointManager.Instance != null && !CheckpointManager.Instance.IsCheckpointActivated(checkpointID))
        {
            CheckpointManager.Instance.CurrentActiveInstance = this;
            CheckpointManager.Instance.ActivateCheckpoint(checkpointID, transform.position);

            PlayerDeath playerDeath = collision.GetComponent<PlayerDeath>();
            if (playerDeath != null)
            {
                playerDeath.SetCheckpoint(transform.position);
                animator.SetTrigger("Activate");
            }
        }
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