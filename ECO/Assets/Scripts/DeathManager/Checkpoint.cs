using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Vector3 lastCheckpointPosition = Vector3.zero;
    private bool hasBeenActivated = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasBeenActivated)
        {
            lastCheckpointPosition = transform.position;
            hasBeenActivated = true;
            spriteRenderer.color = Color.red;
        }
    }
}