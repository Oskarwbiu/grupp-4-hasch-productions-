using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackDamage = 1f;
    [SerializeField] float attackCooldown = 0.5f;
    [SerializeField] float knockbackForce = 5f;
    [SerializeField] GameObject slashEffect;

    Vector2 attackDirection;
    float lastAttackTime;
 
    private void Start()
    {
        
    }

    void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();

        if (moveInput != Vector2.zero)
        {
            attackDirection = moveInput.normalized;
          
        }
    }

    private void Update()
    {
        lastAttackTime += Time.deltaTime;
    }

    void OnAttack()
    {
       
        if (lastAttackTime < attackCooldown)
            return;

        GetComponent<PlayerMovement>().AttackAnimation();

        lastAttackTime = 0;

        Vector2 point = (Vector2)transform.position + (attackDirection *  new Vector2(attackRange/2, attackRange/2));
        Vector2 size = new Vector2(attackRange, attackRange * 1.5f);
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(point, size, angle, LayerMask.GetMask("Enemy"));
        Debug.DrawRay(transform.position, attackDirection * attackRange, Color.red, 0.1f);

        
        Instantiate(slashEffect, point, Quaternion.Euler(0, 0, angle));

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyScript = enemy.GetComponent<EnemyHealth>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);

                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    
                    Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    float alignment = Vector2.Dot(attackDirection.normalized, knockbackDirection);

                    enemyRb.AddForce(knockbackDirection * (knockbackForce + alignment * 1.5f), ForceMode2D.Impulse);
                }
            }
        }
        
    }

    

    void OnPause()
    {
        FindFirstObjectByType<PauseMenu>().Pause();
    }


}
