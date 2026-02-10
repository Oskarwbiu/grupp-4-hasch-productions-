using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MechAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float attackspeed = 1.0f;
    [SerializeField] float attackCooldown = 2f;
    [SerializeField] Collider2D arenaBounds;
    [Header("Dash Attack")]
    [SerializeField] float dashForce = 25f;
    [SerializeField] float knockBack = 2f;
    [SerializeField] float dashingDamage = 2f;
    [Header("Fly Attack")]
    [SerializeField] float flySpeed = 5f;
    [SerializeField] int flyTimes = 1;
    [SerializeField] float bombDropInterval = 0.5f;
    [SerializeField] GameObject bombPrefab;
    [Header("Spin Shot Attack")]
    [SerializeField] float spinShotInterval = 0.1f;
    [SerializeField] float spinShotsPerAttack = 12f;
    [SerializeField] float spinShotForce = 15f;
    [SerializeField] GameObject spinShotPrefab;

    Coroutine currentAttack;
    Coroutine revealAnimation;
    Rigidbody2D rb;
    GameObject player;
    float gravityScale;
    bool isAttacked = false;
    bool isTouchingPlayer = false;
    bool canAttack = false;
    bool isDashing = false;
    float currentDamage = 1;
    int lastAttack = -1;

    float BoundsTop => arenaBounds.bounds.max.y;
    float BoundsBottom => arenaBounds.bounds.min.y;
    float BoundsRight => arenaBounds.bounds.max.x;
    float BoundsLeft => arenaBounds.bounds.min.x;
    float BoundsCenterX => arenaBounds.bounds.center.x;
    float BoundsCenterY => arenaBounds.bounds.center.y;


    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        gravityScale = rb.gravityScale;
        revealAnimation = StartCoroutine(RevealAnimation());
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            if (isDashing)
            {
                Vector2 knockBackForce = collision.transform.position - transform.position * knockBack;
                collision.GetComponent<Rigidbody2D>().AddForce(knockBackForce);
            }
        }
        else
        {
            isTouchingPlayer = false;
        }
    }

    private void FixedUpdate()
    {
        if (isTouchingPlayer && canAttack)
        {
            canAttack = false;
            FindFirstObjectByType<PlayerHealth>().GetDamaged(currentDamage);
            Invoke("ResetAttack", attackspeed);
        }
    }

    private void ResetAttack()
    {
        canAttack=true;
    }

    IEnumerator RevealAnimation()
    {
        yield return null;
        ChooseAttack();
        Debug.Log("Reveal animation finished, starting attacks.");
    }

    void ChooseAttack()
    {
        int attack = Random.Range(0, 3);
        if (lastAttack == attack)
        {
            ChooseAttack();
            return;
        }
        else
        {
            lastAttack = attack;
        }

        Debug.Log("Chosen attack: " + attack);
        switch (attack)
        {
            case 0:
                currentAttack = StartCoroutine(DashAttack());
                break;
            case 1:
                currentAttack = StartCoroutine(FlyAttack());
                break;
            case 2:
                currentAttack = StartCoroutine(SpinShot());
                break;

        }

    }
    IEnumerator DashAttack()
    {
        isDashing = true;
        transform.rotation = Quaternion.Euler(0,0,90);
        currentDamage = dashingDamage;
        yield return new WaitForSeconds(0.5f);
        rb.linearVelocityX = -dashForce;

        transform.rotation = Quaternion.Euler(0, 0, -90);
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        yield return new WaitUntil(() => transform.position.x < BoundsLeft + 3.5f);
        rb.linearVelocityX = 0;
        yield return new WaitForSeconds(0.3f);

        rb.linearVelocityX = dashForce * 2;

        yield return new WaitUntil(() => transform.position.x > BoundsRight - 3.5f);
        rb.linearVelocityX = 0;

        Debug.Log("DashAttack Finished");

        currentDamage = 1;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        isDashing = false;
        Invoke("ChooseAttack", attackCooldown);
    }



    IEnumerator FlyAttack()
    {

        rb.gravityScale = 0;
        rb.linearVelocityY = flySpeed;


        yield return new WaitUntil(() => transform.position.y > BoundsTop - 1.5f);

        rb.linearVelocityY = 0;
        yield return new WaitForSeconds(0.5f);

        rb.linearVelocityX = -flySpeed * 2;

        for (int i = 0; i < flyTimes; i++)
        {
            while (!(transform.position.x < BoundsLeft + 5f))
            {
                yield return new WaitForSeconds(bombDropInterval);
                Instantiate(bombPrefab, transform.position, Quaternion.identity);
                if (isAttacked)
                {
                    rb.gravityScale = gravityScale;
                    // stun the boss for a short time
                    break;
                }
            }

            rb.linearVelocityX = -rb.linearVelocity.x;

            while (!(transform.position.x > BoundsRight - 5f))
            {
                yield return new WaitForSeconds(bombDropInterval);
                Instantiate(bombPrefab, transform.position, Quaternion.identity);
                if (isAttacked)
                {
                    rb.gravityScale = gravityScale;
                    // stun the boss for a short time
                    break;
                }
            }
        }
        rb.linearVelocityX = 0;

        rb.gravityScale = gravityScale/2;

        yield return new WaitForSeconds(0.15f);

        yield return new WaitUntil(() => rb.linearVelocity.y >= -0.1);
        rb.gravityScale = gravityScale;

        Debug.Log("Fly Attack Finished");

        Invoke("ChooseAttack", attackCooldown);
    }

    IEnumerator SpinShot()
    {
        rb.linearVelocityY = 75f;

        yield return new WaitUntil(() => rb.transform.position.y > BoundsTop + 8);
        yield return new WaitForSeconds(1f);
        rb.linearVelocityY = 0;

        rb.transform.position = new Vector2(BoundsCenterX, transform.position.y);
        rb.gravityScale *= 3;

        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => rb.linearVelocity.y >= -0.1);

        rb.gravityScale = gravityScale;

        float direction = 1;
        for (int i = 0; i <= spinShotsPerAttack; i++)
        {
            Rigidbody2D currentBullet = Instantiate(spinShotPrefab, (Vector2)transform.position + Vector2.down, Quaternion.identity).GetComponent<Rigidbody2D>();
            currentBullet.AddForceX(direction * spinShotForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(spinShotInterval);
            direction = -direction;
        }

        Debug.Log("Spin Shot Attack Finished");

        Invoke("ChooseAttack", attackCooldown);
    }

    void OnDrawGizmos()
    {
        if (arenaBounds != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(new Vector3(BoundsLeft + 2.5f, 0), new Vector3(BoundsLeft + 2.5f, 20, 0));
            Gizmos.DrawLine(new Vector3(BoundsRight - 2.5f, 0), new Vector3(BoundsRight - 2.5f, 20, 0));
        }
    }
}
 