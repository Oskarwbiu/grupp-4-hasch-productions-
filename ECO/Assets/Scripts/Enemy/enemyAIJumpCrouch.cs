using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class enemyAIJumpCrouch : MonoBehaviour
{

    [SerializeField] float jumpForce = 10f;
    [SerializeField] LayerMask groundLayer;


    Vector2 footPosition;
    RaycastHit2D jumpCast;
    Collider2D crouchHit;
    new BoxCollider2D collider;
    Bounds colliderSize;
    Rigidbody2D rb;
    Vector2 originalColliderSize;
    Vector2 originalColliderOffset;

    Coroutine jumpCoroutine;


    bool isCrouching = false;
    bool isJumping = false;
    bool hasjumped = false;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        colliderSize = collider.bounds;
        rb = GetComponent<Rigidbody2D>();

        originalColliderSize = collider.size;
        originalColliderOffset = collider.offset;

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        bool isChasing = GetComponent<enemyAI>().IsChasing();
        Transform playerPos = GetComponent<enemyAI>().PlayerPos();

        footPosition = transform.position - new Vector3(0, originalColliderSize.y/2.4f, 0);
        Vector2 areaMin = (Vector2)transform.position + collider.offset + new Vector2(-originalColliderSize.x, originalColliderSize.y/2 - 0.5f + originalColliderSize.y - collider.size.y);
        Vector2 areaMax = areaMin + new Vector2(originalColliderSize.x * 2, 0.5f + originalColliderSize.y - collider.size.y);


        jumpCast = Physics2D.Raycast(footPosition, Vector2.right * transform.localScale.x, 1f, groundLayer);
        Debug.DrawRay(footPosition, transform.right * transform.localScale.x * 1f, Color.green, 0.1f);
        if (jumpCast.collider != null && jumpCast.collider != this.collider && isChasing)
        {

            if (!isJumping && !(playerPos.position.y < transform.position.y && transform.position.x - playerPos.position.x < 2))
            {
                StartCoroutine(Jump(true));
                Debug.Log("Jump");
            }
        }
        else
        {
            if (!isJumping)
            {
                StartCoroutine(Jump(false));
            }
        }



        crouchHit = Physics2D.OverlapArea(areaMin, areaMax, groundLayer);
        Debug.DrawLine(areaMin, areaMax, Color.blue);
        if (crouchHit != null && crouchHit != this.collider && isChasing)
        {
            Crouch(true);
            Debug.Log("Crouch");

        }
        else
        {
            Crouch(false);
        }

    }
    void Crouch(bool shouldCrouch)
    {


        if (shouldCrouch && !isCrouching && !isJumping)
        {
            isCrouching = true;
            collider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
            collider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - originalColliderSize.y * 0.25f);
        }
        else if (!shouldCrouch && isCrouching && !isJumping)
        {
            isCrouching = false;
            collider.size = originalColliderSize;
            collider.offset = originalColliderOffset;
        }


    }
    IEnumerator Jump(bool shouldJump)
    {
        
        if (!hasjumped)
            {
            isJumping = true;
            hasjumped = true;
            if (shouldJump)
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                yield return new WaitForSeconds(0.5f);
            }
            isJumping = false;
            hasjumped = false;
        }

       
    }
}

