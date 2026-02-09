using UnityEngine;

public class HorizontalPlatform : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float leftBound = 0f;
    [SerializeField] private float rightBound = 10f;

    private Vector3 originalPosition;
    private bool movingRight = true;

    void Start()
    {
        originalPosition = transform.position;
        leftBound = originalPosition.x - 5f;
        rightBound = originalPosition.x + 5f;
    }

    void FixedUpdate()
    {
        if (movingRight)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            if (transform.position.x >= rightBound)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            if (transform.position.x <= leftBound)
            {
                movingRight = true;
            }
        }
    }
}