using System.Collections;
using UnityEngine;

public class TrafficLightAttack : MonoBehaviour
{
    [SerializeField] float damage = 1f;
    [SerializeField] float attackSpeed = 1f;

    Animator ani;
    LineRenderer lr;
    bool canAttack = true;
    private void Start()
    {
        ani = GetComponent<Animator>();
        lr = GetComponent<LineRenderer>();
    }
    public IEnumerator AttackPlayer(Vector2 playerPos, float delay)
    {
        if (canAttack)
        {
            canAttack = false;
            Invoke("ResetAttack", attackSpeed);

            ani.SetBool("isIdle", false);
            Vector2 dir = (playerPos - (Vector2)transform.position).normalized;

            yield return new WaitForSeconds(delay - 0.5f);

            ani.SetTrigger("Charge");

            yield return new WaitForSeconds(0.5f);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, LayerMask.GetMask("Ground", "Player"));

            lr.enabled = true;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, (Vector2)transform.position + (30 * dir));

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                FindFirstObjectByType<PlayerHealth>().GetDamaged(damage);
            }

            yield return new WaitForSeconds(0.35f);

            lr.enabled = false;
            yield return new WaitForSeconds(0.65f);
            ani.SetBool("isIdle", true);
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }
}
