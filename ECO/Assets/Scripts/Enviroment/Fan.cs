using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] private float blowSpeed = 10f;
    [SerializeField] private Vector2 blowDirection = Vector2.right;
    private Rigidbody2D playerRb;
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            playerRb = collision.GetComponent<Rigidbody2D>();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            playerRb = null;
        }
    }

    void FixedUpdate()
    {
        if (playerInRange && playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x + blowDirection.x * blowSpeed, playerRb.linearVelocity.y);
        }
    }
}