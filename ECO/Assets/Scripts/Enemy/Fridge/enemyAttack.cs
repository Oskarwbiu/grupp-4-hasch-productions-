using System.Collections;
using UnityEngine;

public class enemyAttack : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float attackCooldown = 1f;
    bool hasAttacked = false;
    public bool lockScale = false;
    EnemyHealth health;
    bool canDamage;
    bool hasChecked = false;

    private void Start()
    {
        health = GetComponent<EnemyHealth>();
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (health.isStunned)
        {
            return;
        }
        lockScale = true;
        
        if (other.CompareTag("Player") && !hasAttacked)
        {
            hasAttacked = true;
            if (!hasChecked)
            {
                
                GetComponent<enemyAI>().PlayAttackAnimation();
                SoundManager.Instance.PlaySound2D("FridgeAttack");
                hasChecked = true;
            }
            StartCoroutine(Attack());

            
            
            Invoke("ResetAttack", attackCooldown);
        }
        if (health != null && canDamage && other.CompareTag("Player"))
        {
            PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
            canDamage = false;
            health.GetDamaged(damage);

        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.3f);
        canDamage = true;
        yield return new WaitForSeconds(0.1f);
        canDamage = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        lockScale = false;
    }
    void ResetAttack()
    {
        hasAttacked = false;
        hasChecked = false;
    }
}
