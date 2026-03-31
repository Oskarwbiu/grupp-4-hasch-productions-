using UnityEngine;
using UnityEngine.Tilemaps;

public class Fire : MonoBehaviour
{

    PlayerHealth health;
    bool isOnCooldown;


    private void OnTriggerStay2D(Collider2D collision)
    { 
        if (collision.CompareTag("Player") && !isOnCooldown)
        {
            if (health == null)
            {
                health = FindFirstObjectByType<PlayerHealth>();
            }
            isOnCooldown = true;
            health.GetDamaged(1f);
            Invoke("ResetCooldown", 2.5f);
        }
    }

    void ResetCooldown()
    {
        isOnCooldown = false;
    }
}
