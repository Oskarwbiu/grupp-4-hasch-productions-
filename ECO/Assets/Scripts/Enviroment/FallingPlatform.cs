using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
   
    [SerializeField] public float fallWait = 2f;
    [SerializeField] public float destoryWait = 1f;

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
       
        Destroy(gameObject, destoryWait);                
    }
}
