using System.Threading.Tasks;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float InvincibleTime = 1.0f;
    [SerializeField] int currentHealth = 3;
    [SerializeField] int maxHealth;
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
                Death();
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

    public void Death()
    {
        FindFirstObjectByType<SceneManager>().Restart();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public int CurrentHealth()
    {
        return currentHealth;
    }
}
