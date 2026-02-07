using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] float jumpForce = 10;
    [SerializeField] float jumpCooldown = 0.04f;
    [SerializeField] float jumpLength = 2f;
    [SerializeField] float fallGravityScale = 4f;
    [SerializeField] float jumpBufferTime = 0.5f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] ContactFilter2D groundFilter;

    Animator ani;
    Rigidbody2D rb;
    public bool isGrounded;
    bool hasJumped;
    float gravityScaleAtStart;
    float lastGrounded;
    float lastJumpTime;
    bool startJumpTimer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject  SpriteObject = rb.transform.GetChild(0).gameObject;
        ani = SpriteObject.GetComponent<Animator>();
        gravityScaleAtStart = rb.gravityScale;
    }
    void OnJump()
    {
        hasJumped = true;
        
        Invoke("JumpTimer", jumpCooldown);
    }
    void JumpTimer()
    {
        hasJumped = false;
        lastGrounded = jumpBufferTime;
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
        lastGrounded -= Time.fixedDeltaTime;
        if (startJumpTimer)
            {
            lastJumpTime += Time.fixedDeltaTime;
            if (lastJumpTime >= jumpBufferTime && isGrounded)
            {
                lastJumpTime = 0;
                startJumpTimer = false;
            }
        }
        if (isGrounded)
        {
            lastGrounded = coyoteTime;
        }
    }
    private void Update()
    {
        if (lastGrounded >= 0 && lastJumpTime == 0 && hasJumped)
        { 
            rb.linearVelocityY = 0;
            Jump();
            hasJumped = false;

        }
        isGrounded = rb.IsTouching(groundFilter);


        if (!isGrounded)
        {
            if (rb.linearVelocityY > -0.1 && rb.linearVelocityY < 0.5)
            {
                rb.gravityScale = jumpLength;
                ani.SetBool("isJumping", false);
                ani.SetBool("isTop", true);
            }
            else if (rb.linearVelocityY < -0.1)
            {
                rb.gravityScale = fallGravityScale;
                ani.SetBool("isFalling", true);
                ani.SetBool("isTop", false);

            }
            else if (rb.linearVelocityY > 0.5)
            {
                ani.SetBool("isJumping", true);
                ani.SetBool("isTop", false);
            }

        }
        else
        {
            if (ani.GetBool("isFalling"))
            {
                
                ani.SetTrigger("JumpLand");
                
            }

            ani.SetBool("isJumping", false);
            ani.SetBool("isFalling", false);
            ani.SetBool("isTop", false);
            rb.gravityScale = gravityScaleAtStart;
        }

    }

}
