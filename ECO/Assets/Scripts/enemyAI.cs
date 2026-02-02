using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        Move();
    }
    private void FixedUpdate()
    {

    }
    void Move()
    {
      rb.linearVelocityX = moveSpeed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Clone") && !collision.gameObject.CompareTag("Bullet"))
        {
            FlipHorizontalMovement();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("End Point"))
        {

          FlipHorizontalMovement();
        }
    }
    private void FlipHorizontalMovement()
    {
        moveSpeed = -moveSpeed;
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }
}
