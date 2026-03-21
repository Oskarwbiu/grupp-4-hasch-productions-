using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    GameObject player;
    
    [SerializeField] public int checkpointID;

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
                SoundManager.Instance.PlaySound3D("ActivateCheckpoint", transform.position);
                float duration = animator.GetAnimatorTransitionInfo(0).duration;
                Invoke("TriggerRespawn", duration);
            }
            
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && CheckpointManager.Instance != null && !CheckpointManager.Instance.IsCheckpointActivated(checkpointID))
        {
            ActivateCheckpoint();

            PlayerDeath playerDeath = collision.GetComponent<PlayerDeath>();
            if (playerDeath != null)
            {
                
                playerDeath.SetCheckpoint(transform.position);
                
            }
        }
    }
    
    public void ActivateCheckpoint()
    {
        animator.SetTrigger("Activate");
        CheckpointManager.Instance.CurrentActiveInstance = this;
        CheckpointManager.Instance.ActivateCheckpoint(checkpointID, transform.position);
        SoundManager.Instance.PlaySound2D("ActivateCheckpoint");
    }

    public void TriggerRespawn()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(TriggerAfterRespawnSequence());
    }

    private IEnumerator TriggerAfterRespawnSequence()
    {
        player = GameObject.FindWithTag("Player");
        yield return null;
        animator.SetTrigger("Respawn");
        SoundManager.Instance.PlaySound2D("RespawnCheckpoint");
        yield return new WaitForSeconds(1.5f);
        player.GetComponent<PlayerJump>().isRespawning = false;
        SoundManager.Instance.PlaySound2D("AfterRespawnCheckpoint");
        animator.SetTrigger("AfterRespawn");
    }
}