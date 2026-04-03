using System.Collections;
using UnityEngine;

public class MechanicHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 51f;
    [SerializeField] float hitFlashDuration = 0.3f;

    bool isInvincible = true;
    public bool isDead;
    Animator ani;
    Bossbar healthbar;
    SpriteRenderer spriteRenderer;
    MechanicAttack attackScript;
    ShakeManager shakeManager;
    private void Start()
    {
        shakeManager = FindFirstObjectByType<ShakeManager>();
        currentHealth = maxHealth;
        ani = GetComponent<Animator>();
        healthbar = FindFirstObjectByType<Bossbar>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackScript = GetComponent<MechanicAttack>();
        healthbar.showBar();
        healthbar.UpdateHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        SoundManager.Instance.PlaySound2D("MechanicHurt");
        healthbar.UpdateHealth(currentHealth, maxHealth);
        isInvincible = true;

        spriteRenderer.material.SetFloat("_Flash", 1);
        Time.timeScale = 0f;
        shakeManager.ShakeCamera(0.4f, 3f);
        StartCoroutine(ResetTime(hitFlashDuration));




        if (currentHealth <= 0)
        {
            attackScript.StopAllCoroutines();
            attackScript.enabled = false;
            StartCoroutine(Die());
        }
        else
        {
            attackScript.IsHurt();
        }


    }

    IEnumerator ResetTime(float duration)
    {
       
        yield return new WaitForSecondsRealtime(duration);
        if (!FindFirstObjectByType<PauseMenu>().isPaused)
        {
            spriteRenderer.material.SetFloat("_Flash", 0);
            Time.timeScale = 1;
        }
    }

    public void ResetInvIncibility(bool invincible)
    {
        isInvincible = invincible;
        Debug.Log(isInvincible);
    }

    IEnumerator Die()
    {
        isDead = true;
        ani.SetTrigger("Die");
        yield return new WaitForSeconds(0.3f);
        FindFirstObjectByType<BossSummon>().GoToNextLevel();
    }

}
