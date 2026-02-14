using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    GameObject player;
    
    [SerializeField] private int checkpointID;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (CheckpointManager.Instance != null && CheckpointManager.Instance.IsCheckpointActivated(checkpointID))
        {
            if (CheckpointManager.Instance.GetLastCheckpointPosition() == transform.position)
            {
                player = GameObject.FindWithTag("Player");
                player.GetComponent<PlayerJump>().isRespawning = true;
                CheckpointManager.Instance.CurrentActiveInstance = this;
                animator.SetTrigger("Activate");
                float duration = animator.GetAnimatorTransitionInfo(0).duration;
                Invoke("TriggerRespawn", duration);
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
        animator = GetComponent<Animator>();
        StartCoroutine(TriggerAfterRespawnSequence());
    }

    private IEnumerator TriggerAfterRespawnSequence()
    {
        yield return null;
        Debug.Log("play animation");
        animator.SetTrigger("Respawn");
        yield return new WaitForSeconds(1.5f);
        player.GetComponent<PlayerJump>().isRespawning = false;
        animator.SetTrigger("AfterRespawn");
    }
}