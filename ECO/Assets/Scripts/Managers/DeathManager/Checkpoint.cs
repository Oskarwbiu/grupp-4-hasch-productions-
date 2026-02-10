using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    private static Checkpoint activeCheckpoint;
    private bool hasBeenActivated = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasBeenActivated)
        {
            hasBeenActivated = true;
            activeCheckpoint = this;

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
        animator.SetTrigger("Respawn");
        StartCoroutine(TriggerAfterRespawn());
    }

    private IEnumerator TriggerAfterRespawn()
    {
        yield return new WaitForSeconds(1.5f);
        animator.SetTrigger("AfterRespawn");
    }
}