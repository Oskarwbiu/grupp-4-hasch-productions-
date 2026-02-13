using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float deathZoneY = -10f;
    public Vector3 lastCheckpointPosition = Vector3.zero;

    PlayerInput input;
    Rigidbody2D rb;
    float gravity;
    GameObject spriteObject;

    private void Awake()
    {
        
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        input = rb.GetComponent<PlayerInput>();
        input.enabled = false;
        StartCoroutine(ResetInput());


        spriteObject = rb.transform.GetChild(0).gameObject;
        gravity = rb.gravityScale;

        

        Vector3 spawnPosition = CheckpointManager.Instance.GetLastCheckpointPosition();

        if (spawnPosition != Vector3.zero)
        {
            transform.position = spawnPosition;


            Checkpoint active = CheckpointManager.Instance.CurrentActiveInstance;
            if (active != null)
            {
                active.TriggerRespawn();
                spriteObject.SetActive(false);
                rb.gravityScale = 0;
                GetComponent<PlayerJump>().enabled = false;
                CheckpointManager.Instance.ResetCheckpoints();
            }
            StartCoroutine(Respawn());
        }
        
        
        
        


    }

    IEnumerator ResetInput()
    {
        yield return new WaitForSeconds(0.1f);
        input.enabled = true;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.3f);
        spriteObject.SetActive(true);
        input.enabled = false;
        yield return new WaitForSeconds(1.2f);
        rb.gravityScale = gravity;
        rb.AddForce(new Vector2(20, 12), ForceMode2D.Impulse);
        GetComponent<PlayerJump>().enabled = true;
        input.enabled = true;
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
        Checkpoint activeCheckpoint = CheckpointManager.Instance.CurrentActiveInstance;
        if (activeCheckpoint != null)
        {
            activeCheckpoint.TriggerRespawn();
        }
        rb.linearVelocity = Vector2.zero;
        GetComponent<PlayerMovement>().ResetMovement();
        GetComponent<PlayerMovement>().StartTriggerAnimation("Die");
        input.enabled = false;
        
        StartCoroutine(RestartSceneAfterDelay(2f));
    }

    private System.Collections.IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        FindFirstObjectByType<PlayerHealth>().ResetHealth();
    }
}