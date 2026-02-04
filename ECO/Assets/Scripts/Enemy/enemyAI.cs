using System.Collections;
using UnityEditor;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [SerializeField] float origMoveSpeed = 3f;
    [SerializeField] float maxSpeed = 3f;
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

    private void OnBecameInvisible()
    {
        if (chasePlayer)
        {
            chasePlayer = false;
            player = null;
        }
    }
    private void FixedUpdate()
    {
        Move();
        Vision();
        if (rb.linearVelocityX < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (rb.linearVelocityX > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    void Vision()
    {
        int playerLayerMask = 1 << 3;
        if (!chasePlayer && !isLookingForPlayer)
        {
            isLookingForPlayer = true;


            for (int i = 0; i < 20; i++)
            {
                dir = Quaternion.Euler(0, 0, (i * 3) - 30) * new Vector2(Mathf.Sign(transform.localScale.x/Mathf.Abs(transform.localScale.x)), 0);
                hit = Physics2D.Raycast(transform.position, dir, detectionRange, playerLayerMask);
                Debug.DrawRay(transform.position, dir * detectionRange, Color.red, 0.1f);
                if (hit.collider == null)
                    continue;

                if (hit.collider.CompareTag("Player"))
                {
                    rb.linearVelocityX = 0;
                    player = hit.collider.gameObject;
                    chasePlayer = true;
                    Debug.Log("Player Spotted: " + player);
                }
            }
            isLookingForPlayer = false;
        }
        else if (chasePlayer)
        {
            Chase();
        }
    }

    void Chase()
    {
        
        if (Mathf.Abs(rb.linearVelocity.x) < moveSpeed * runSpeedMultiplier || Mathf.Abs(rb.linearVelocity.x) < maxSpeed * runSpeedMultiplier && !chasePlayer && player != null)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            directionToPlayer = (player.transform.position - transform.position).normalized;
            rb.AddForceX(directionToPlayer.x/Mathf.Abs(directionToPlayer.x) * moveSpeed * runSpeedMultiplier);
        }
    }
    void Move()
    {
        if (Mathf.Abs(rb.linearVelocity.x) < moveSpeed && !chasePlayer || Mathf.Abs(rb.linearVelocity.x) < maxSpeed && !chasePlayer)
        {
            rb.AddForceX(moveSpeed * 2);
        }
    }
        
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !isPatrolling && !chasePlayer)
        {
            FlipHorizontalMovement();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("End Point") && !isPatrolling && !chasePlayer)
        {

          StartCoroutine(Lookaround());
        }
    }
    private void FlipHorizontalMovement()
    {
        moveSpeed = -moveSpeed;
        Debug.Log(moveSpeed);
    }

    IEnumerator Lookaround()
    {
        
        if (!isPatrolling && !chasePlayer)
        {
            isPatrolling = true;
            origMoveSpeed = moveSpeed;
            moveSpeed = 0;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            for (int i = 0; i < lookaroundDuration; i++)
            {
                if (chasePlayer)
                    break;
                yield return new WaitForSeconds(lookaroundInterval);
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                yield return new WaitForSeconds(lookaroundInterval);
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                
            }
            moveSpeed = origMoveSpeed;
            FlipHorizontalMovement();
            isPatrolling = false;
        }
    }

}

