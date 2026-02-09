using System.Threading.Tasks;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float InvincibleTime = 1.0f;
    [SerializeField] float currentHealth = 3f;
    [SerializeField] float maxHealth = 3f;
    bool isInvincible = false;

    public void GetDamaged(float damage)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            currentHealth -= damage;
            Debug.Log("ouch         " + currentHealth);
            if (currentHealth < 1)
            {
                PlayerDeath playerDeath = FindFirstObjectByType<PlayerDeath>();
                if (playerDeath != null)
                {
                    playerDeath.Die();
                }
            }
            Invoke("Invincibility", InvincibleTime);
        }
    }

    public void Heal()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
        }
    }

    public void Invincibility()
    {
        isInvincible = false;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public float CurrentHealth()
    {
        return currentHealth;
    }
}