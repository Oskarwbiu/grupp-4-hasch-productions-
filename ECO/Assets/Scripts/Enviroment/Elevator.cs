using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;       
    [SerializeField] private float activationDuration = 3f; 
    
    private float elapsedTime = 0f;
    private bool isMoving = false;
    private bool hasBeenActivated = false;

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player") && !hasBeenActivated)
        {
            StartElevator();
        }
    }

    private void StartElevator()
    {
        isMoving = true;
        elapsedTime = 0f;
        hasBeenActivated = true;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            elapsedTime += Time.fixedDeltaTime;

          
            transform.Translate(Vector3.up * moveSpeed * Time.fixedDeltaTime);

          
            if (elapsedTime >= activationDuration)
            {
                isMoving = false;

            }
        }
    }
}