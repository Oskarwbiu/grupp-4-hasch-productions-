using System.Collections;
using UnityEngine;

public class CarHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 51f;
    [SerializeField] float invincibilityDuration = 1f;
    [SerializeField] float hitFlashDuration = 0.3f;

    bool isInvincible = false;
    Animator ani;
    Bossbar healthbar;
    SpriteRenderer spriteRenderer;
    CarAttack attackScript;
    bool isPhase2 = false;

    private void Start()
    {
        currentHealth = maxHealth;
        ani = GetComponent<Animator>();
        healthbar = FindFirstObjectByType<Bossbar>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackScript = GetComponent<CarAttack>();
        healthbar.maxHealth = maxHealth;
        healthbar.health = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        StartCoroutine(HitFlash());
        healthbar.health = currentHealth;
        isInvincible = true;
        Invoke("ResetInvIncibility", invincibilityDuration);

        if (currentHealth <= maxHealth / 2 && !isPhase2)
        {
            isPhase2 = true;
            attackScript.InitiatePhase2();
        }

        if (currentHealth <= 0)
        {
            attackScript.StopAllCoroutines();
            attackScript.enabled = false;
            //float duration = ani.GetCurrentAnimatorStateInfo(0).length;
            float duration = 0.1f;

            Invoke("Die", duration);
        }


    }

    IEnumerator HitFlash()
    {
        spriteRenderer.material.SetFloat("_Flash", 1);
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.material.SetFloat("_Flash", 0);
    }

    void ResetInvIncibility()
    {
        isInvincible = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
