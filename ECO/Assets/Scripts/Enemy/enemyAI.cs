using System.Collections;
using UnityEditor;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [SerializeField] float origMoveSpeed = 5f;
    [SerializeField] int lookaroundDuration = 2;
    [SerializeField] float lookaroundInterval = 0.5f;
    [SerializeField] float runSpeedMultiplier = 1.5f;
    [SerializeField] float detectionRange = 5f;
    Rigidbody2D rb;
    float moveSpeed;
    RaycastHit2D hit;
    GameObject player;
    bool chasePlayer = false;
    Vector2 dir;
    bool isLookingForPlayer = false;
    bool isPatrolling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = origMoveSpeed;
    }

    
    void Update()
    {
        Move();
    }

    private void OnBecameInvisible()
    {
        if (chasePlayer)
        {
            chasePlayer = false;
        }
    }
    private void FixedUpdate()
    {
        
        if (!chasePlayer && !isLookingForPlayer)
        {
            isLookingForPlayer = true;
            Debug.Log("Looking for player");

            for (int i = 0; i < 20; i++)
            {
                dir = Quaternion.Euler(0, 0, (i * 3) - 40) * new Vector2(Mathf.Sign(moveSpeed/Mathf.Abs(moveSpeed)), 0);
                hit = Physics2D.Raycast(transform.position, dir, detectionRange);
                Debug.DrawRay(transform.position, dir * detectionRange, Color.red, 0.1f);

                if (hit.collider == CompareTag("Player"))
                {
                    if (hit.collider != null)
                    {
                        player = hit.collider.gameObject;
                        chasePlayer = true;
                    }
                }
            }
            isLookingForPlayer = false;
        }
        else if (chasePlayer)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            if (player != null && Mathf.Abs(rb.linearVelocity.x) < moveSpeed || Mathf.Abs(runSpeedMultiplier * rb.linearVelocity.x + directionToPlayer.x) < Mathf.Abs(runSpeedMultiplier * rb.linearVelocity.x))
            {
                directionToPlayer = (player.transform.position - transform.position).normalized;
                rb.AddForceX(directionToPlayer.x * moveSpeed);
            }
            
        }
    }
    void Move()
    {
        if (Mathf.Abs(rb.linearVelocity.x) < moveSpeed || Mathf.Abs(rb.linearVelocity.x + moveSpeed) < Mathf.Abs(rb.linearVelocity.x) && !chasePlayer)
        {
            rb.AddForceX(moveSpeed);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            FlipHorizontalMovement();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("End Point"))
        {

          StartCoroutine(Lookaround());
        }
    }
    private void FlipHorizontalMovement()
    {
        moveSpeed = -moveSpeed;
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }

    IEnumerator Lookaround()
    {
        
        if (!isPatrolling && !chasePlayer)
        {
            isPatrolling = true;
            origMoveSpeed = moveSpeed;
            moveSpeed = 0;
            for (int i = 0; i < lookaroundDuration; i++)
            {
                yield return new WaitForSeconds(lookaroundInterval);
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                Debug.Log($"Lookaround {i}");
                yield return new WaitForSeconds(lookaroundInterval);
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                Debug.Log($"Lookaround {i}");
            }
            moveSpeed = origMoveSpeed;
            FlipHorizontalMovement();
            isPatrolling = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, dir);
    }
}

