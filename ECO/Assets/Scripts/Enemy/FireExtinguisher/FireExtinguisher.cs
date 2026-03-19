using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [SerializeField] float origMoveSpeed;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float decceleration = 10f;
    [SerializeField] float runSpeedMultiplier = 1.5f;
    [SerializeField] int lookaroundDuration = 2;
    [SerializeField] float lookaroundInterval = 0.5f;
    [SerializeField] LayerMask detectionLayer;

    Coroutine lookaroundCoroutine;
    Animator ani;
    GameObject player;
    float moveSpeed;
    Rigidbody2D rb;
    bool chasePlayer;
    Vector2 dir;
    RaycastHit2D hit;
    bool isLookingForPlayer = false;
    bool isPatrolling = false;
    bool isAttacking = false;

    float wallCheckTimer = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = origMoveSpeed;
        ani = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
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

            isPatrolling = false;

            if (lookaroundCoroutine != null)
            {
                StopCoroutine(lookaroundCoroutine);
            }

        }
        Vision();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("End Point") && !isPatrolling && !chasePlayer)
        {

            lookaroundCoroutine = StartCoroutine(Lookaround());
        }

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


    void FlipHorizontalMovement()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        moveSpeed = -moveSpeed;
    }

    void Chase()
    {
        float distance = player.transform.position.x - transform.position.x;
        float absDistance = Mathf.Abs(distance);

        float preferredDistance = attackRange;      
        float tolerance = 0.3f;                     

        float direction = Mathf.Sign(distance);

        float targetSpeed = 0f;

        if (absDistance > preferredDistance + tolerance)
        {
            
            targetSpeed = origMoveSpeed * runSpeedMultiplier * direction;
        }
        else if (absDistance < preferredDistance - tolerance)
        {
            
            targetSpeed = origMoveSpeed * runSpeedMultiplier * -direction;
        }
        else
        {
            targetSpeed = 0f;
        }


        float speedDifference = targetSpeed - rb.linearVelocity.x;

        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 0.9f) * Mathf.Sign(speedDifference);
        
        rb.AddForce(movement * Vector2.right);

    }

    void Move()
    {
        
        float speedDifference = moveSpeed - rb.linearVelocity.x;

        float accelerationRate = (Mathf.Abs(moveSpeed) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 0.9f) * Mathf.Sign(speedDifference);

        rb.AddForce(movement * Vector2.right);

    }

}
     
