using UnityEngine;
using System.Collections;

public class Fan : MonoBehaviour
{
    [SerializeField] private float blowForce = 10f;
    [SerializeField] private Vector2 blowDirection = Vector2.right;

    private Collider2D fanCollider;
    private Rigidbody2D playerRb;
    private bool playerInRange = false;

    void Start()
    {
        fanCollider = GetComponent<Collider2D>();
    }

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
            playerRb.AddForce(blowDirection * blowForce, ForceMode2D.Force);
        }
    }
}