using System;
using System.Collections;
using System.Drawing;
using System.Linq;
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
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float footstepInterval = 0.5f;
    [SerializeField] float runInterval = 0.5f;
    float moveSpeed;
    PlayerHealth playerHealth;
    Animator ani;
    float multiplier = 1f;
    float absMoveSpeed;
    bool wasGrounded;
    bool isLocked = false;
    float footstepTimer = 0f;
    bool isRunning = false;
    float runTimer = 0;

    private InputAction moveAction;

    void OnEnable()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
        {
            playerInput.ActivateInput();
            playerInput.actions?.Enable();

            
        }
    }

   


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (transform.childCount > 0)
            SpriteObject = transform.GetChild(0).gameObject;

        if (SpriteObject != null)
            ani = SpriteObject.GetComponent<Animator>();

        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        originalSize = SpriteObject.transform.localScale;
        moveSpeed = originalMoveSpeed;

        if (playerInput != null)
        {
            playerInput.ActivateInput();
            playerInput.actions.Enable();
        }
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        Debug.Log(playerInput);
    }

    public void SpeedBuff(UnityEngine.Color color, float speedMultiplier, float duration)
    {
        StartCoroutine(SpeedCoroutine(color, speedMultiplier, duration));
    }

    IEnumerator SpeedCoroutine(UnityEngine.Color color, float speedMultiplier, float duration)
    {
        UnityEngine.Color originalColor = SpriteObject.GetComponent<SpriteRenderer>().material.GetColor("_Color");
        SpriteObject.GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
        SpriteObject.GetComponent<SpriteRenderer>().material.SetFloat("_ChangeColor", 1);
        moveSpeed *= speedMultiplier;
        yield return new WaitForSeconds(duration);
        SpriteObject.GetComponent<SpriteRenderer>().material.SetFloat("_ChangeColor", 0);
        SpriteObject.GetComponent<SpriteRenderer>().material.SetColor("_Color", originalColor);
        moveSpeed = originalMoveSpeed;
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
            footstepInterval = 0.4f;
            isRunning = false;
        }
        else if (absMoveSpeed > moveSpeed && absMoveSpeed < moveSpeed * dashSpeedMultiplier)
        {
            footstepInterval = 0.2f;
            isRunning = true;
        }

        if (isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.5f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                SoundManager.Instance.PlaySound2D("Footstep");
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        if (isRunning && isGrounded)
        {
            runTimer -= Time.deltaTime;
            if (runTimer <= 0)
            {
                SoundManager.Instance.PlaySound2D("PlayerRun");
                runTimer = runInterval;
            }
        }
        else { runTimer = 0f; }

    }

    void Update()
    {
        SetAnimation();
        FlipSprite();
    }

    public void ResetMovement()
    {
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
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
            StartTriggerAnimation("Dash");
            SoundManager.Instance.PlaySound2D("PlayerDash");
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
 
        
        if (!playerHealth.isDead)
        {
            float horizontalInput = MathF.Sign(moveInput.x);
            float targetMoveSpeed = horizontalInput * moveSpeed * multiplier;

            float speedDifference = targetMoveSpeed - rb.linearVelocity.x;

            float accelerationRate = (Mathf.Abs(targetMoveSpeed) > 0.01f) ? acceleration : decceleration;

            float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 0.9f) * Mathf.Sign(speedDifference);

            rb.AddForce(movement * Vector2.right);
        }
        


        if (isGrounded && Mathf.Abs(moveInput.x) < 0.01f)
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
        if (playerHealth.isDead)
        {
            return;
        }
        if (moveInput.x > 0)
        {
            SpriteObject.transform.localScale = new Vector3(Mathf.Abs(originalSize.x), originalSize.y, originalSize.z);
        }
        else if (moveInput.x < 0)
        {
            SpriteObject.transform.localScale = new Vector3(-Mathf.Abs(originalSize.x), originalSize.y, originalSize.z);
        }
    }

    public void AttackAnimation()
    {
        StartTriggerAnimation("Attack");
    }

    public void StartTriggerAnimation(string animation)
    {
        ChangeAnimation("");
        ani.ResetTrigger(animation);
        ani.SetTrigger(animation);
        isLocked = true;

        float duration = ani.GetCurrentAnimatorStateInfo(0).length;
        Invoke("ResetLock", duration);
    }

    void ResetLock()
    { isLocked = false; }

    void SetAnimation()
    {
        if (ani.GetCurrentAnimatorStateInfo(0).IsName("Die") || ani.GetBool("isDead") || playerHealth.isDead)
        {
            return;
        }


        if (isLocked) { return; }

        float verticalVelocity = rb.linearVelocity.y;
        bool movingHorizontally = Mathf.Abs(rb.linearVelocity.x) > 0.3f && moveInput.x != 0;
        AnimatorStateInfo currentClip = ani.GetCurrentAnimatorStateInfo(0);

        if (currentClip.IsName("Attack") || currentClip.IsName("Dash") || currentClip.IsName("JumpLand") || currentClip.IsName("Die"))
        {

            if (currentClip.normalizedTime < 1.0f)
            {
                return;
            }

        }

        // -- LAND --
        if (isGrounded && !wasGrounded)
        {
            StartTriggerAnimation("JumpLand");
        }

        // -- AIRBORNE --
        if (!isGrounded)
        {
            // -- RISING --
            if (verticalVelocity > 0.1f)
            {

                ChangeAnimation("isJumping");
            }

            // -- FALLING --
            else if (verticalVelocity < -0.1f)
            {

                ChangeAnimation("isFalling");
            }

            // -- AT APEX --
            else
            {

                ChangeAnimation("isTop");
            }

            return;
        }


        if (movingHorizontally && isGrounded)
        {
            // -- RUNNING --
            if (absMoveSpeed >= moveSpeed * dashSpeedMultiplier - 0.1f && moveInput.x != 0)
            {
                ChangeAnimation("isRunning");
            }
            // -- WALKING --
            else if (moveInput.x != 0)
            {
                ChangeAnimation("isWalking");
            }
            return;
        }
        else if (absMoveSpeed > 0.1f && isGrounded)
        {
            // -- STOP --
            ChangeAnimation("isStopping");
            return;
        }
        else
        {
            ChangeAnimation("");
        }
    }
    void ChangeAnimation(string newParam)
    {


        string[] paramsToReset = { "isRunning", "isWalking", "isStopping", "isFalling", "isTop", "isJumping" };

        if (newParam != "" && ani.GetBool(newParam))
        {
            return;
        }




        foreach (var p in paramsToReset)
        {

            ani.SetBool(p, p == newParam);
        }
    }
}