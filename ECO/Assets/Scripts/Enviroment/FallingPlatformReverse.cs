using UnityEngine;
using System.Collections;

public class FallingPlatformReverse : MonoBehaviour
{
    // How long until the platform starts to float after touch
    public float fallWait = 0.05f;
    public float destoryWait = 1f;        // time until the object is destroyed after it starts floating
    public float initialFallSpeed = 5f;   // upward speed applied while player stands on the platform (positive = up)

    bool isFloating;
    bool playerOnPlatform;
    Rigidbody2D rb;

    void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        // Keep platform static/kinematic until floating begins
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
            if (!isFloating)
            {
                StartCoroutine(BeginFloat());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
            // When player steps off, stop rising
            if (isFloating)
            {
                StopFloating();
            }
        }
    }

    private IEnumerator BeginFloat()
    {
        // small delay before starting to float
        yield return new WaitForSeconds(fallWait);

        isFloating = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f; // prevent gravity from pulling it down
        // Optionally schedule destroy after a delay (set destoryWait <= 0 to disable)
        if (destoryWait > 0f)
            Destroy(gameObject, destoryWait);
    }

    private void StopFloating()
    {
        isFloating = false;
        rb.linearVelocity = Vector2.zero;
        // return to kinematic so it won't be affected by physics after stopping
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 1f;
    }

    void FixedUpdate()
    {
        if (isFloating && playerOnPlatform)
        {
            // Move upward while player stands on it
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, initialFallSpeed);
        }
    }
}
