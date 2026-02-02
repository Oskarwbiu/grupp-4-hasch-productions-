using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] float jumpForce = 10;
    [SerializeField] float jumpCooldown = 0.04f;
    [SerializeField] ContactFilter2D groundFilter;
    Rigidbody2D rb;
    public bool isGrounded;
    bool hasJumped;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    void OnJump()
    {
        hasJumped = true;
        Invoke("JumpTimer", jumpCooldown);
    }
    void JumpTimer()
    {
        hasJumped = false;
    }
    void Jump()
    {   
       rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
         
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }
    private void FixedUpdate()
    {
        if (isGrounded && hasJumped)
        { 
        Jump();
            hasJumped = false;
        }
       isGrounded = rb.IsTouching(groundFilter);
    }

}
