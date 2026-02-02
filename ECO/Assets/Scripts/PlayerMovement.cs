using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] ContactFilter2D groundFilter;

    GameObject SpriteObject;
    Rigidbody2D rb;
    Vector2 moveInput;
    Vector3 originalSize;
    Coroutine moveCoroutine;
    bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SpriteObject = rb.transform.GetChild(0).gameObject;
        StartCoroutine(MovePlayer());
    }

    
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        isGrounded = rb.IsTouching(groundFilter);
    }


    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

    }
    
    IEnumerator MovePlayer()
    {
        while (true)
        {
            yield return null;

            if (Mathf.Abs(rb.linearVelocity.x) < moveSpeed || Mathf.Abs(rb.linearVelocity.x + moveInput.x) < Mathf.Abs(rb.linearVelocity.x))
            {
                rb.AddForceX(moveInput.x * moveSpeed);
            }
            else if (isGrounded)
            {
                yield return new WaitForSeconds(0.1f);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.83f, rb.linearVelocity.y);
            }
            if (rb.linearVelocityX != 0 && moveInput.x == 0 && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.85f, rb.linearVelocity.y);
            }

            if (moveInput.x < 0)
            {
                SpriteObject.transform.localScale = new Vector2(-originalSize.x, 1);
            }
            if (moveInput.x > 0)
            {
                SpriteObject.transform.localScale = new Vector2(originalSize.x, 1);
            }
        }
    }
}
