using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] float jumpForce = 10;
    [SerializeField] float jumpCooldown = 0.04f;
    [SerializeField] float jumpLength = 2f;
    [SerializeField] float fallGravityScale = 4f;
    [SerializeField] float jumpBufferTime = 0.5f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float maxFallSpeed = 10f;
    [SerializeField] ContactFilter2D groundFilter;

    Animator ani;
    Rigidbody2D rb;
    public bool isGrounded;
    bool hasJumped;
    float gravityScaleAtStart;
    float lastGrounded;
    float lastJumpTime;
    bool jumpHeld;
    bool startJumpTimer;
    bool isJumping;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject  SpriteObject = rb.transform.GetChild(0).gameObject;
        ani = SpriteObject.GetComponent<Animator>();
        gravityScaleAtStart = rb.gravityScale;
    }
    void OnJump(InputValue value)
    {
        jumpHeld = value.isPressed;
        hasJumped = true;
        isJumping = true;

        Debug.Log(jumpHeld);

        Invoke("JumpCutReset", 0.1f);
        Invoke("JumpTimer", jumpBufferTime);
        if (jumpHeld)
        {
            TryJump();
        }
    }

    void TryJump()
    {
        if (lastGrounded >= 0 && lastJumpTime == 0)
        { 
            Jump();
            hasJumped = false;
        }
    }

    void JumpCutReset()
    {
        isJumping = false;
    }
    void JumpTimer()
    {
        hasJumped = false;
        lastGrounded = jumpCooldown;
    }
    void Jump()
    {   

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        

        startJumpTimer = true;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }

    private void FixedUpdate()
    {
        CheckFallSpeed();

        if (startJumpTimer)
            {
            lastJumpTime += Time.fixedDeltaTime;
            if (lastJumpTime >= jumpCooldown && isGrounded)
            {
                lastJumpTime = 0;
                startJumpTimer = false;
            }
        }

        lastGrounded -= Time.fixedDeltaTime;
        if (isGrounded)
        {
            lastGrounded = coyoteTime;
        }


        
    }
    private void Update()
    {
        
        isGrounded = rb.IsTouching(groundFilter);

        if (!jumpHeld && isJumping && rb.linearVelocity.y > 0)
        {
            rb.linearVelocityY *= 0.65f;
        }

        if (!isGrounded)
        {
            if (rb.linearVelocityY > -0.1 && rb.linearVelocityY < 0.5)
            {
                rb.gravityScale = jumpLength;
                
            }
            else if (rb.linearVelocityY < -0.1)
            {
                rb.gravityScale = fallGravityScale;

            }
            

        }
        else
        {
            rb.gravityScale = gravityScaleAtStart;
        }

    }

    void CheckFallSpeed()
    {

        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            Vector2 clampedVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxFallSpeed);
            rb.linearVelocityY = clampedVelocity.y;
        }

        
    }

}
