using UnityEngine;
using System.Collections;

public class ElevatorPlatform : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float peakHeight = 4.1f;
    [SerializeField] private float exitDelay = 0.75f;

    private Vector3 originalPosition;
    private Vector3 peakPosition;
    private Rigidbody2D playerRb;
    private bool playerOnElevator = false;
    private float timePlayerLeft = 0f;

    void Start()
    {
        originalPosition = transform.position;
        peakPosition = originalPosition + Vector3.up * peakHeight;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.transform.position.y > transform.position.y)
            {
                playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                playerOnElevator = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnElevator = false;
            timePlayerLeft = Time.time;
        }
    }

    private void FixedUpdate()
    {
        bool shouldGoUp = playerOnElevator || (Time.time - timePlayerLeft < exitDelay);

        if (shouldGoUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, peakPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }
}