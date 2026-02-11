using UnityEngine;

public class MechHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 51f;
    [SerializeField] float invincibilityDuration = 1f;

    bool isInvincible = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        Invoke("ResetInvIncibility", invincibilityDuration);

        if (currentHealth <= maxHealth/2)
        {
            GetComponent<MechAttack>().InitiatePhase2();
        }

        if (currentHealth <= 0)
        {
            Die();
        }


    }

    void ResetInvIncibility()
    {
        isInvincible = false ;
    }

    void Die()
    {

        Destroy(gameObject);
    }
    



}
