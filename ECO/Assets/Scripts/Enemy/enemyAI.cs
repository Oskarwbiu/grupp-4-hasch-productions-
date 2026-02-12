using System.Collections;
using UnityEditor;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [SerializeField] float origMoveSpeed = 3f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float decceleration = 10f;
    [SerializeField] int lookaroundDuration = 2;
    [SerializeField] float lookaroundInterval = 0.5f;
    [SerializeField] float runSpeedMultiplier = 1.5f;
    [SerializeField] float crouchSpeedMultiplier = 0.5f;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] LayerMask detectionLayer;

    float wallCheckTimer = 0;
    Rigidbody2D rb;
    float moveSpeed;
    RaycastHit2D hit;
    GameObject player;
    bool chasePlayer = false;
    Vector2 dir;
    bool isLookingForPlayer = false;
    bool isPatrolling = false;
    bool isGrounded = false;
    float moveSpeedMultiplier = 1f;
    Animator ani;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = origMoveSpeed;
        ani = GetComponent<Animator>();
    }

    public void StunEnemy(float stunDuration)
        {
        StopAllCoroutines();
        moveSpeed = 0;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        isPatrolling = false;
        Invoke("UnStun", stunDuration);
    }

    void UnStun()
    {
        moveSpeed = origMoveSpeed;
    }

    private void FixedUpdate()
    {
        isGrounded = GetComponent<enemyAI>().isGrounded;

        if (!chasePlayer)
        {
            Move();
        }
        else if (chasePlayer)
        {
            Chase();
            
             isPatrolling = false;
             StopCoroutine(Lookaround());
            
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
                dir = Quaternion.Euler(0, 0, (i * 3) -19f) * new Vector2(Mathf.Sign(-transform.localScale.x/Mathf.Abs(transform.localScale.x)), 0);
                hit = Physics2D.Raycast(transform.position, dir, detectionRange, detectionLayer);
                Debug.DrawRay(transform.position, dir * detectionRange, Color.red, 0.05f);
                if (hit.collider == null)
                { 
                    
                    continue;
                }

                if (hit.collider.CompareTag("Player"))
                {
                    rb.linearVelocityX = 0;
                    player = hit.collider.gameObject;
                    chasePlayer = true;

                }
                
            }
            RaycastHit2D wallCheck = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y), Vector2.left * Mathf.Sign(transform.localScale.x), 1f, LayerMask.GetMask("Ground"));
            RaycastHit2D wallCheck2 = Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.left * Mathf.Sign(transform.localScale.x), 1f, LayerMask.GetMask("Ground"));
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.right * Mathf.Sign(-transform.localScale.x) * 1f, Color.violet, 0.05f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.right * Mathf.Sign(-transform.localScale.x) * 1f, Color.violet, 0.05f);
            wallCheckTimer += Time.fixedDeltaTime;
            
            if ((wallCheck.collider != null || wallCheck2.collider != null) && wallCheckTimer > 1)
            {
                wallCheckTimer = 0;
                FlipHorizontalMovement();
            }

            isLookingForPlayer = false;
        }
        
    }

    void Chase()
    {
        if (!ani.GetBool("walk") && !ani.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            ani.SetBool("walk", true);
            ani.SetBool("idle", false);
        }
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

        float targetSpeed = Mathf.Abs(origMoveSpeed) * runSpeedMultiplier * moveSpeedMultiplier * Mathf.Sign(directionToPlayer.x);

        float speedDifference = targetSpeed - rb.linearVelocity.x;

        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 0.9f) * Mathf.Sign(speedDifference);

        rb.AddForce(movement * Vector2.right);

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
        if(!ani.GetBool("walk") && !ani.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            ani.SetBool("walk", true);
            ani.SetBool("idle", false);
        }
        float speedDifference = moveSpeed - rb.linearVelocity.x;

        float accelerationRate = (Mathf.Abs(moveSpeed) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 0.9f) * Mathf.Sign(speedDifference);

        rb.AddForce(movement * Vector2.right);

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
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);

    }

    IEnumerator Lookaround()
    {
        
        if (!isPatrolling && !chasePlayer)
        {
            if (!ani.GetBool("idle"))
            {
                ani.SetBool("idle", true);
                ani.SetBool("walk", false);
            }

            isPatrolling = true;
            origMoveSpeed = moveSpeed;
            moveSpeed = 0;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            for (int i = 0; i < lookaroundDuration; i++)
            {
 
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
        if (isCrouching)
        {
            moveSpeedMultiplier *= crouchSpeedMultiplier;
        }
        else
        {
            moveSpeedMultiplier = 1f;
        }

    }


}

