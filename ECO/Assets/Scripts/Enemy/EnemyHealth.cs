using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    public float currentHealth;
    [SerializeField] float invincibilityDuration = 0.5f;

    float invincibilityTimer = 0f;
    public bool isHurt = false;
    public bool isStunned = false;

    private void Start()
    {
            currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        if (invincibilityTimer < invincibilityDuration)
        {
            return;
        }
        

        currentHealth -= damage;
        invincibilityTimer = 0f;

        if (currentHealth <= 0)
        {
            Die();
        }
        isHurt = true;
        Invoke("ResetHurt", 0.15f);
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
