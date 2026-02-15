using System.Threading.Tasks;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float invincibleTime = 1.0f;
    public float currentHealth = 3f;
    [SerializeField] float maxHealth = 3f;
    bool isInvincible = false;
    float OrigInvincibleTime => invincibleTime;
    public void GetDamaged(float damage)
    {
        if (!isInvincible && !GameObject.FindWithTag("Player").GetComponent<PlayerCheats>().isGodMode)
        {
            FindFirstObjectByType<DamageVignette>().ShowDamageVignette();
            isInvincible = true;
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                PlayerDeath playerDeath = FindFirstObjectByType<PlayerDeath>();
                if (playerDeath != null)
                {
                    playerDeath.Die();
                    invincibleTime = 3;
                }
            }
            Invoke("Invincibility", invincibleTime);
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
        invincibleTime = OrigInvincibleTime;
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