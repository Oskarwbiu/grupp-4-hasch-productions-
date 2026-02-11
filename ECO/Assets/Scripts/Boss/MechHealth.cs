using UnityEngine;

public class MechHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 100f;
    [SerializeField] float invincibilityDuration = 1f;

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

        if (currentHealth <= maxHealth/2)
        {
            GetComponent<MechAttack>().InitiatePhase2();
        }

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
