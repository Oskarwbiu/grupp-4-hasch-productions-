using UnityEngine;

public class enemyAttack : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float attackCooldown = 1f;
    bool hasAttacked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasAttacked)
        {
            other.GetComponent<PlayerHealth>().GetDamaged(damage);
            Invoke("ResetAttack", attackCooldown);
        }
    }
    void ResetAttack()
    {
        hasAttacked = false;
    }
}
