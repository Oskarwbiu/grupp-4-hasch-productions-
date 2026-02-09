using UnityEngine;

public class HorizontalPlatform : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float distance = 5f;

    private Vector3 startPosition;
    private bool movingRight = true;
    private Vector3 lastPosition;
    private Rigidbody2D playerOnPlatform;

    void Start()
    {
        startPosition = transform.position;
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = movingRight ?
            startPosition + Vector3.right * distance :
            startPosition - Vector3.right * distance;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            movingRight = !movingRight;
        }

        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (playerOnPlatform != null)
        {
            Vector3 platformMovement = transform.position - lastPosition;
            playerOnPlatform.transform.position += platformMovement;
        }

        lastPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = null;
        }
    }
}