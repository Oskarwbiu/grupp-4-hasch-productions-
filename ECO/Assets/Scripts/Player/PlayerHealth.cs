using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float invincibleTime = 0.4f;
    public float currentHealth = 3f;
    [SerializeField] float maxHealth = 3f;
    bool isInvincible = false;
    public bool isDead = false;

    private void Start()
    {
        currentHealth = PlayerPrefs.GetFloat("PlayerHealth", maxHealth);
    }
    public void GetDamaged(float damage)
    {
        if (!isInvincible && !GameObject.FindWithTag("Player").GetComponent<PlayerCheats>().isGodMode && !isDead)
        {
            FindFirstObjectByType<DamageVignette>().ShowDamageVignette();
            SoundManager.Instance.PlaySound2D("PlayerHurt");
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
            PlayerPrefs.SetFloat("PlayerHealth", currentHealth);
            StartCoroutine(ResetTime());
            Time.timeScale = 0;
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

    IEnumerator ResetTime()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (!FindFirstObjectByType<PauseMenu>().isPaused)
        {
            Time.timeScale = 1;
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