using System.Collections;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float invincibleTime = 0.4f;
    public float currentHealth = 3f;
    [SerializeField] float maxHealth = 3f;
    [SerializeField] float deathShakeDuration = 0.5f;
    [SerializeField] float deathShakeIntensity = 4f;
    [SerializeField] float deathHitFlashDuration = 0.5f;
    bool isInvincible = false;
    public bool isDead = false;
    SpriteRenderer spriteRenderer;


    private void Start()
    {
        currentHealth = PlayerPrefs.GetFloat("PlayerHealth", maxHealth);
      
    }
    public void GetDamaged(float damage, float hitFlashDuration = 0.25f, float shakeDuration = 0.25f, float shakeIntensity = 2f)
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
            if (isDead)
            {
                shakeDuration = deathShakeDuration;
                shakeIntensity = deathShakeIntensity;
                hitFlashDuration = deathHitFlashDuration;
            }
            ShakeManager.Instance.ShakeCamera(shakeDuration, shakeIntensity);

            PlayerPrefs.SetFloat("PlayerHealth", currentHealth);
            StartCoroutine(ResetTime(hitFlashDuration));

            spriteRenderer = GameObject.FindWithTag("Player").GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.material.SetFloat("_Flash", 1);

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

    IEnumerator ResetTime(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        if (!FindFirstObjectByType<PauseMenu>().isPaused)
        {
            spriteRenderer = GameObject.FindWithTag("Player").GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.material.SetFloat("_Flash", 0);
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