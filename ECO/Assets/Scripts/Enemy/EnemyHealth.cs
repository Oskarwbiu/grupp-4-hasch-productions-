using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    public float currentHealth;
    [SerializeField] float invincibilityDuration = 0.5f;
    [SerializeField] float hitFlashDuration = 0.25f;

    SpriteRenderer spriteRenderer;
    float invincibilityTimer = 0f;
    public bool isHurt = false;
    public bool isStunned = false;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void TakeDamage(float damage)
    {
        if (invincibilityTimer < invincibilityDuration)
        {
            return;
        }
        

        currentHealth -= damage;
        invincibilityTimer = 0f;
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
        isHurt = true;
        Invoke("ResetHurt", 0.15f);
    }

    IEnumerator HitFlash()
    {
        spriteRenderer.material.SetFloat("_Flash", 1);
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.material.SetFloat("_Flash", 0);
    }

    void ResetHurt()
    {
        isHurt = false;
    }

    void Die()
    {
     
        Destroy(gameObject);
    }
    private void Update()
    {
        invincibilityTimer += Time.deltaTime;
    }

    public IEnumerator Stun(float stunDuration)
    {
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }
}
