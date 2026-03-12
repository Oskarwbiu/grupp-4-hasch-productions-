using System.Threading.Tasks;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float invincibleTime = 0.4f;
    public float currentHealth = 3f;
    [SerializeField] float maxHealth = 3f;
    bool isInvincible = false;
    public bool isDead = false;
    public void GetDamaged(float damage)
    {
        if (!isInvincible && !GameObject.FindWithTag("Player").GetComponent<PlayerCheats>().isGodMode && !isDead)
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
                    isDead = true;
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
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public float CurrentHealth()
    {
        return currentHealth;
    }


}