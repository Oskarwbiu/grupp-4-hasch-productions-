using System;
using System.Collections;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float originalMoveSpeed = 5f;
    [SerializeField] ContactFilter2D groundFilter;

    GameObject SpriteObject;
    Rigidbody2D rb;
    Vector2 moveInput;
    Vector3 originalSize;
    Coroutine moveCoroutine;
    bool isGrounded = false;
    bool dashed = false;
    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float dashSpeedMultiplier = 2f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float decceleration = 10f;
    [SerializeField] float frictionAmount = 0.2f;
    float moveSpeed;
    Animator ani;
    float multiplier = 1f;
    float absMoveSpeed;
    bool wasGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SpriteObject = rb.transform.GetChild(0).gameObject;
        originalSize = SpriteObject.transform.localScale;
        ani = SpriteObject.GetComponent<Animator>();
        moveSpeed = originalMoveSpeed;
        ResetAnimation();
    }
    private void FixedUpdate()
    {
        absMoveSpeed = Mathf.Abs(rb.linearVelocity.x);

        wasGrounded = isGrounded;
        isGrounded = rb.IsTouching(groundFilter);
        MovePlayer();

        if (absMoveSpeed <= moveSpeed)
        {
            multiplier = 1f;
        }

    }

    void Update()
    {
        SetAnimation();
        FlipSprite();
    }


    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

    }
    void OnDash()
    {
        if (!dashed)
        {
            dashed = true;
            rb.AddForce(new Vector2(SpriteObject.transform.localScale.x * dashForce, 0), ForceMode2D.Impulse);
            ResetAnimation();
            ani.ResetTrigger("Dash");
            ani.SetTrigger("Dash");
            Invoke("ResetDash", dashCooldown);
            multiplier = dashSpeedMultiplier;
        }
    }
    void ResetDash()
    {
        dashed = false;
    }
    void MovePlayer()
    {
        float targetMoveSpeed = moveInput.x * moveSpeed * multiplier;

        float speedDifference = targetMoveSpeed - rb.linearVelocity.x;

        float accelerationRate = (Mathf.Abs(targetMoveSpeed) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 0.9f) * Mathf.Sign(speedDifference);

        rb.AddForce(movement * Vector2.right);


        if (isGrounded && moveInput.x == 0)
        {
            float stoppingSpeed = Mathf.Min(absMoveSpeed, Mathf.Abs(frictionAmount));

            stoppingSpeed *= Mathf.Sign(rb.linearVelocity.x);

            rb.AddForce(-stoppingSpeed * Vector2.right, ForceMode2D.Impulse);

            if (absMoveSpeed < 0.1f)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }

    }

    void FlipSprite()
    {
        if (moveInput.x > 0)
        {
            SpriteObject.transform.localScale = new Vector3(Mathf.Abs(originalSize.x), originalSize.y, originalSize.z);
        }
        else if (moveInput.x < 0)
        {
            SpriteObject.transform.localScale = new Vector3(-Mathf.Abs(originalSize.x), originalSize.y, originalSize.z);
        }
    }



    void SetAnimation()
    {
        float verticalVelocity = rb.linearVelocity.y;
        bool movingHorizontally = Mathf.Abs(rb.linearVelocity.x) > 0.3f && moveInput.x != 0;


        // -- DASH --
        if (ani.GetBool("Dash"))
        {
            return;
        }

        // -- LAND --
        if (isGrounded && !wasGrounded)
        {
            ResetAnimation();
            ani.ResetTrigger("JumpLand");
            ani.SetTrigger("JumpLand");
        }

        // -- AIRBORNE --
        if (!isGrounded)
        {
            // -- RISING --
            if (verticalVelocity > 0.1f)
            {
                ResetAnimation();
                ani.SetBool("isJumping", true);
            }

            // -- FALLING --
            else if (verticalVelocity < -0.1f)
            {
                ResetAnimation();
                ani.SetBool("isFalling", true);
            }

            // -- AT APEX --
            else
            {
                ResetAnimation();
                ani.SetBool("isTop", true);
            }

            return;
        }

        ResetAnimation();

        if (movingHorizontally && isGrounded)
        {
            // -- RUNNING --
            if (absMoveSpeed >= moveSpeed * dashSpeedMultiplier - 0.1f && moveInput.x != 0)
            {
                ani.SetBool("isRunning", true);
            }
            // -- WALKING --
            else if (moveInput.x != 0)
            {
                ani.SetBool("isWalking", true);
            }
        }
        else if (absMoveSpeed > 0.1f && isGrounded)
        {
            // -- STOP --
            ani.SetBool("isStopping", true);
        }
        
        // -- IDLE --
        // No movement, idle animation will play by default
    }
    void ResetAnimation()
    {
        ani.SetBool("isRunning", false);
        ani.SetBool("isWalking", false);
        ani.SetBool("isStopping", false);
        ani.SetBool("isFalling", false);
        ani.SetBool("isTop", false);
        ani.SetBool("isJumping", false);
    }
}
