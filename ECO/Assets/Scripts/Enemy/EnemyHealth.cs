using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] float invincibilityDuration = 0.5f;

    float invincibilityTimer = 0f;

    private void Start()
    {
            currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        if (invincibilityTimer < invincibilityDuration)
        {
            return;
        }

        currentHealth -= damage;
        invincibilityTimer = 0f;

        if (currentHealth <= 0)
        {
            Die();
        }

       
    }

    void Die()
    {
     
        Destroy(gameObject);
    }
    private void Update()
    {
        invincibilityTimer += Time.deltaTime;
    }
}
