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
    float dashValue;
    float moveSpeed;
    Animator ani;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SpriteObject = rb.transform.GetChild(0).gameObject;
        StartCoroutine(MovePlayer());
        originalSize = SpriteObject.transform.localScale;
        ani = SpriteObject.GetComponent<Animator>();
        moveSpeed = originalMoveSpeed;
        Debug.Log(moveSpeed);
    }
    private void FixedUpdate()
    {
        isGrounded = rb.IsTouching(groundFilter);
    }


    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

    }
    void OnDash(InputValue value)
    { 
            dashValue = value.Get<float>();
            if (!dashed)
            {
            dashed = true;
            rb.AddForce(new Vector2(SpriteObject.transform.localScale.x * dashForce, 0), ForceMode2D.Impulse);
            Invoke("ResetDash", dashCooldown);
            }}
    void ResetDash()
    {
        dashed = false;
    }
    IEnumerator MovePlayer()
    {
        while (true)
        {
            moveSpeed = Mathf.Sign(transform.localScale.x) * (originalMoveSpeed + originalMoveSpeed * (dashSpeedMultiplier-1) * dashValue);
            yield return null;

            if (Mathf.Abs(rb.linearVelocity.x) < moveSpeed || Mathf.Abs(rb.linearVelocity.x + moveInput.x) < Mathf.Abs(rb.linearVelocity.x) && moveInput.x != 0)
            {
                rb.AddForceX(moveInput.x * moveSpeed);
            }
            if (moveInput.x < 0)
            {
                SpriteObject.transform.localScale = new Vector2(-originalSize.x, 1);
            }
            if (moveInput.x > 0)
            {
                SpriteObject.transform.localScale = new Vector2(originalSize.x, 1);
            }
            if (moveInput.x != 0 && isGrounded)
            {
                ani.SetBool("isWalking", true);
                ani.SetBool("isStopping", false);
            }
            else if (isGrounded && moveInput.x == 0)
            {

                ani.SetBool("isWalking", false);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.80f, rb.linearVelocity.y);
                if (Mathf.Abs(rb.linearVelocityX) >= 0.1 && isGrounded && moveInput.x == 0)
                {
                    ani.SetBool("isStopping", true);
                    yield return new WaitForSeconds(0.05f);
                }
                if (Mathf.Abs(rb.linearVelocityX) < 0.7f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.4f, rb.linearVelocity.y);
                    ani.SetBool("isStopping", false);
                }
                if (Mathf.Abs(rb.linearVelocityX) < 0.2f)
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }
            }
        }
    }
}
