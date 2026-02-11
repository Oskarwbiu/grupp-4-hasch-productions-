using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float deathZoneY = -10f;
    public Vector3 lastCheckpointPosition = Vector3.zero;

    Rigidbody2D rb;
    float gravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        gravity = rb.gravityScale;

        Vector3 spawnPosition = CheckpointManager.Instance.GetLastCheckpointPosition();

        if (spawnPosition != Vector3.zero)
        {
            transform.position = spawnPosition;


            Checkpoint active = Checkpoint.GetActiveCheckpoint();
            if (active != null)
            {
                active.TriggerRespawn();
                rb.gravityScale = 0;
                rb.linearVelocityY = 0;
                rb.linearVelocityX = 0;
                GetComponent<PlayerJump>().enabled = false;
                GetComponent<PlayerMovement>().enabled = false;
            }
            StartCoroutine(Respawn());
        }
        


    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.5f);
        rb.gravityScale = gravity;
        Debug.Log("add gravity");
        rb.AddForce(new Vector2(20, 12), ForceMode2D.Impulse);
        GetComponent<PlayerJump>().enabled = true;
        GetComponent <PlayerMovement>().enabled = true;
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