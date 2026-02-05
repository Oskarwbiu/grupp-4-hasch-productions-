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
    [SerializeField] float crouchSpeedMultiplier = 0.5f;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] LayerMask detectionLayer;


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

    
    private void FixedUpdate()
    {
        if (!chasePlayer)
        {
            Move();
        }
        else if (chasePlayer)
        {
            Chase();
        }
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
        if (player == null && chasePlayer)
        {
            chasePlayer = false;
        }
        if (!chasePlayer && !isLookingForPlayer)
        {
            isLookingForPlayer = true;


            for (int i = 0; i < 12; i++)
            {
                dir = Quaternion.Euler(0, 0, (i * 3) -19f) * new Vector2(Mathf.Sign(transform.localScale.x/Mathf.Abs(transform.localScale.x)), 0);
                hit = Physics2D.Raycast(transform.position, dir, detectionRange, detectionLayer);
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
        
    }

    void Chase()
    {
        
        if (Mathf.Abs(rb.linearVelocity.x) < moveSpeed * runSpeedMultiplier || Mathf.Abs(rb.linearVelocity.x) < maxSpeed * runSpeedMultiplier && !chasePlayer && player != null)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            directionToPlayer = (player.transform.position - transform.position).normalized;
            rb.AddForceX(directionToPlayer.x/Mathf.Abs(directionToPlayer.x) * moveSpeed * runSpeedMultiplier);
        }
        else
        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
        }
    }

    private void OnBecameInvisible()
    {
        if (chasePlayer)
        {
            StopChasing();
        }
    }
    void StopChasing()
    {
        chasePlayer = false;
        player = null;
    }
    void Move()
    {
        if (Mathf.Abs(rb.linearVelocity.x) < moveSpeed && !chasePlayer || Mathf.Abs(rb.linearVelocity.x) < maxSpeed && !chasePlayer)
        {
            rb.AddForceX(moveSpeed * 2);
        }
        else if (!chasePlayer)
        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.85f, rb.linearVelocity.y);
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
                { 
                    moveSpeed = origMoveSpeed;
                    isPatrolling = false;
                    StopCoroutine(Lookaround());
                }
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
    public bool IsChasing()
    {
        return chasePlayer;
    }

    public Transform PlayerPos()
    {         
        if (player != null)
        {
            return player.transform;
        }
        else
        {
            return null;
        }
    }
    public void SetCrouchSpeedMultiplier(bool isCrouching)
    {
        if (!isCrouching)
        {
            moveSpeed = origMoveSpeed;
        }
        else
        {
            moveSpeed = origMoveSpeed * crouchSpeedMultiplier;
        }
    }


}

