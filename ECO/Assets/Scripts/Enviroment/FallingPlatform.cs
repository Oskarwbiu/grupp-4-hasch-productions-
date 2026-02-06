using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    // How long until the platform starts to fall after touch � set very small so it falls almost instantly
    public float fallWait = 2f;
    public float destoryWait = 1f;        // time until the object is destroyed after it starts falling
    public float initialFallSpeed = -5f;  // initial downward speed applied when falling begins

    bool isFalling;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        { 
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        isFalling = true;
  
        yield return new WaitForSeconds(fallWait);
      
        rb.bodyType = RigidbodyType2D.Dynamic;
       
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, initialFallSpeed);
        Destroy(gameObject, destoryWait);                
    }
}
