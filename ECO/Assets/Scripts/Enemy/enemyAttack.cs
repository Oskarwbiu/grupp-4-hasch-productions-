using UnityEngine;

public class enemyAttack : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float attackCooldown = 1f;
    bool hasAttacked = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasAttacked)
        {
            PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
            if (health != null)
            {
                GetComponent<enemyAI>().PlayAttackAnimation();
                health.GetDamaged(damage);
                hasAttacked = true;
                Invoke("ResetAttack", attackCooldown);
            }
        }
    }
    void ResetAttack()
    {
        hasAttacked = false;
    }
}
