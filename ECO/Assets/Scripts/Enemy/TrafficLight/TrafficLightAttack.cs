using System.Collections;
using UnityEngine;

public class TrafficLightAttack : MonoBehaviour
{
    [SerializeField] float damage = 1f;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float chargeInterval = 0.2f;
    [SerializeField] float chargeDuration = 0.35f;

    Animator ani;
    LineRenderer lr;
    bool canAttack = true;
    bool isAttacking = false;
    EnemyHealth health;
    float chargeTimer;
    private void Start()
    {
        ani = GetComponent<Animator>();
        lr = GetComponent<LineRenderer>();
        health = GetComponent<EnemyHealth>();
    }

    private void FixedUpdate()
    {
        if (health.isStunned)
        {
            lr.enabled = false;
            return;
        }
        if (isAttacking)
        {
            chargeTimer -= Time.deltaTime;

            if (chargeTimer <= 0f)
            {
                SoundManager.Instance.PlaySound2D("TrafficCharge");
                chargeTimer = chargeInterval;
            }


            transform.localScale = new Vector3(-Mathf.Sign(GameObject.FindWithTag("Player").transform.position.x - transform.position.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            Vector2 playerPos = GameObject.FindWithTag("Player").transform.position;
            Vector2 dir = (playerPos - (Vector2)transform.position).normalized;
            Color red = new Color(1, 0, 0, 0.1f);
            Color yellow = new Color(1, 1, 0, 0.75f);
            Color color = Color.Lerp(red, yellow, Mathf.PingPong(Time.time * 2, 1));
            lr.startWidth = 0.2f;
            lr.endWidth = 0.2f;
            lr.enabled = true;
            lr.startColor = color;
            lr.endColor = color;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, (Vector2)transform.position + (60 * dir));
        }
        else
        {
            chargeTimer = 0f;
        }
    }
    public IEnumerator AttackPlayer(GameObject player, float delay)
    {
        if (canAttack)
        {
            canAttack = false;
            
            Invoke("ResetAttack", attackSpeed);

            ani.SetBool("isIdle", false);
            

            yield return new WaitForSeconds(delay - 0.5f);
            isAttacking = true;

            ani.SetTrigger("Charge");
 

            yield return new WaitForSeconds(chargeDuration);
            Vector2 dir = (player.transform.position - transform.position).normalized;
            isAttacking = false;
            yield return new WaitForSeconds(0.25f);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, LayerMask.GetMask("Ground", "Player"));
            SoundManager.Instance.PlaySound2D("TrafficShoot");
            lr.enabled = true;
            lr.startColor = Color.yellow;
            lr.endColor = Color.yellow;
            lr.startWidth = 0.4f;
            lr.endWidth = 0.4f;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, (Vector2)transform.position + (60 * dir));

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
