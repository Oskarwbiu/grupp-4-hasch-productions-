using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] public float fallWait = 2f;
    [SerializeField] public float destroyWait = 1f;
    [SerializeField] public float respawnWait = 3f;

    bool isFalling;
    Rigidbody2D rb;
    Vector3 originalPosition;
    Quaternion originalRotation;
    Collider2D platformCollider;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            if (collision.gameObject.transform.position.y > transform.position.y)
            {
                StartCoroutine(Fall());
            }
        }
    }

    private IEnumerator Fall()
    {
        isFalling = true;
  
        yield return new WaitForSeconds(fallWait);
      
        rb.bodyType = RigidbodyType2D.Dynamic;
       
        float fadeTimer = 0f;
        while (fadeTimer < destroyWait)
        {
            fadeTimer += Time.deltaTime;
            float alpha = 1f - (fadeTimer / destroyWait);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
            yield return null;
        }
        
        platformCollider.enabled = false;
        
        yield return new WaitForSeconds(respawnWait);
        
        ResetPlatform();
    }

    private void ResetPlatform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        platformCollider.enabled = true;
        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
        isFalling = false;
    }
}