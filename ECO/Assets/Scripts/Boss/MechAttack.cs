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
    [SerializeField] BoxCollider2D triggerBox;
    [SerializeField] BoxCollider2D hitBox;
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
    [Header("Airstrike Attack")]
    [SerializeField] float amount = 10f;
    [SerializeField] GameObject airstrike;
    [SerializeField] GameObject warning;

    Coroutine currentAttack;
    Coroutine revealAnimation;
    Rigidbody2D rb;
    GameObject player;
    Animator ani;
    float gravityScale;
    bool isTouchingPlayer = false;
    bool canAttack = true;
    bool isDashing = false;
    float currentDamage = 1;
    int lastAttack = -1;
    int phase = 0;
    MechAnimation mechAnimation;
    float BoundsTop => arenaBounds.bounds.max.y;
    float BoundsBottom => arenaBounds.bounds.min.y;
    float BoundsRight => arenaBounds.bounds.max.x;
    float BoundsLeft => arenaBounds.bounds.min.x;
    float BoundsCenterX => arenaBounds.bounds.center.x;
    float BoundsCenterY => arenaBounds.bounds.center.y;


    void Start()
    {

        arenaBounds = GameObject.FindWithTag("BossBounds").GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        mechAnimation = rb.GetComponent<MechAnimation>();
        gravityScale = rb.gravityScale;
        ani = GetComponent<Animator>();
        revealAnimation = StartCoroutine(RevealAnimation());
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
        else
        {
            isTouchingPlayer = false;
        }
    }

    private void FixedUpdate()
    {
        if (isTouchingPlayer && canAttack && isDashing)
        {
            canAttack = false;
            PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
                if (playerHealth != null)
            {
                playerHealth.GetDamaged(currentDamage);
            }
            Debug.Log("damage player");
            if (isDashing)
            {
                Vector2 knockBackForce = new Vector2(Mathf.Sign(player.transform.position.x - transform.position.x), 0) * knockBack + (Vector2.up * (knockBack/2));
                player.GetComponent<Rigidbody2D>().AddForce(knockBackForce);
            }
            Invoke("ResetAttack", attackspeed);
        }
        isTouchingPlayer = false;
    }

    private void ResetAttack()
    {
        canAttack=true;
    }

    IEnumerator RevealAnimation()
    {
        yield return null;
        float endPos = BoundsRight - 5f;
        rb.linearVelocityY = 75;

        yield return new WaitUntil(() => rb.transform.position.y > BoundsTop + 10);
        yield return new WaitForSeconds(2f);

        rb.linearVelocityY = 0;
        rb.transform.position = new Vector2(endPos, transform.position.y);
        rb.gravityScale *= 3;

        mechAnimation.PlayAnimation("isFalling");

        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => rb.linearVelocity.y >= -0.1);

        rb.gravityScale = gravityScale;
        mechAnimation.PlayTrigger("land");
        SoundManager.Instance.PlaySound2D("MechLand");

        yield return new WaitForSeconds(0.5f);

        ChooseAttack();
        Debug.Log("Reveal animation finished, starting attacks.");
    }

    void ChooseAttack()
    {
        transform.localScale = new Vector3(-1,1,1);
        int attack = Random.Range(0, 3 + phase);
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
            case 3:
                currentAttack = StartCoroutine(Airstrike());
                break;

        }

    }
    IEnumerator DashAttack()
    {
        hitBox.size /= 1.5f;
        triggerBox.size /= 1.5f;
        hitBox.offset = new Vector2(0,-0.8f);
        
        yield return new WaitForSeconds(0.4f);
        SoundManager.Instance.PlaySound2D("MissileReady");
        yield return new WaitForSeconds(0.6f);
        ani.SetBool("isIdle", false);
        mechAnimation.PlayTrigger("dash");
        SoundManager.Instance.PlaySound2D("MechDash");
        rb.linearVelocityX = -dashForce;
        
        
        isDashing = true;
        currentDamage = dashingDamage;

        yield return new WaitUntil(() => transform.position.x < BoundsLeft + 5f);
        rb.linearVelocityX = 0;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        mechAnimation.PlayAnimation("isIdle");
        SoundManager.Instance.PlaySound2D("MissileReady");
        yield return new WaitForSeconds(0.6f);
        ani.SetBool("isIdle", false);
        mechAnimation.PlayTrigger("dash");
        rb.linearVelocityX = dashForce * 2;
        SoundManager.Instance.PlaySound2D("MechDash");

        yield return new WaitUntil(() => transform.position.x > BoundsRight - 2.5f);
        rb.linearVelocityX = 0;
        mechAnimation.PlayAnimation("isIdle");

        Debug.Log("DashAttack Finished");

        currentDamage = 1;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        isDashing = false;
        transform.position += Vector3.up;
        hitBox.size *= 1.5f;
        triggerBox.size *= 1.5f;
        hitBox.offset = new Vector2(0, 0);
        Invoke("ChooseAttack", attackCooldown);
    }



    IEnumerator FlyAttack()
    {
        ani.SetBool("isIdle", false);
        rb.gravityScale = 0;
        rb.linearVelocityY = flySpeed;
        mechAnimation.PlayTrigger("flyUp");
        SoundManager.Instance.PlaySound2D("MechFly");
        yield return new WaitForSeconds(0.6f);
        mechAnimation.PlayAnimation("isFlying");

        yield return new WaitUntil(() => transform.position.y > BoundsTop - 1.5f);

        rb.linearVelocityY = 0;
        yield return new WaitForSeconds(0.5f);

        rb.linearVelocityX = -flySpeed * 2;

        for (int i = 0; i < flyTimes; i++)
        {
            while (!(transform.position.x < BoundsLeft + 8f))
            {
                yield return new WaitForSeconds(bombDropInterval);
                SoundManager.Instance.PlaySound2D("BombDrop");
                Instantiate(bombPrefab, transform.position, Quaternion.identity);
            }

            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            rb.linearVelocityX = -rb.linearVelocity.x;

            while (!(transform.position.x > BoundsRight - 2.5f))
            {
                yield return new WaitForSeconds(bombDropInterval);
                SoundManager.Instance.PlaySound2D("BombDrop");
                Instantiate(bombPrefab, transform.position, Quaternion.identity);
            }
        }
        rb.linearVelocityX = 0;

        rb.gravityScale = gravityScale;
        mechAnimation.PlayTrigger("flyDown");
        ani.SetBool("isFlying", false);
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => rb.linearVelocity.y >= -0.1);
        mechAnimation.PlayTrigger("land");
        SoundManager.Instance.PlaySound2D("MechLand");

        Debug.Log("Fly Attack Finished");

        Invoke("ChooseAttack", attackCooldown);
    }

    IEnumerator SpinShot()
    {
        Vector2 startPos = transform.position;
        int startPhase = phase;

        // Jump to middle
        mechAnimation.PlayTrigger("flyUp");
        rb.linearVelocityY = 75f;
        SoundManager.Instance.PlaySound2D("MechFly");

        yield return new WaitUntil(() => rb.transform.position.y > BoundsTop + 8);
        yield return new WaitForSeconds(1f);
        rb.linearVelocityY = 0;

        rb.transform.position = new Vector2(BoundsCenterX, transform.position.y);
        rb.gravityScale *= 3;
        mechAnimation.PlayAnimation("isFalling");
        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => rb.linearVelocity.y >= -0.1);
        mechAnimation.PlayTrigger("land");
        SoundManager.Instance.PlaySound2D("MechLand");

        rb.gravityScale = gravityScale;
        if (phase == 1)
        {
            yield return new WaitForSeconds(0.4f);
            ani.SetBool("isIdle", false);
            mechAnimation.PlayTrigger("readyCannons");
            SoundManager.Instance.PlaySound2D("MissileReady");
            yield return new WaitForSeconds(0.7f);
            mechAnimation.PlayAnimation("isShooting");
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Shoot bullets
        float direction = 1;
        Vector2 spawnPos = Vector2.down;
        for (int i = 0; i <= spinShotsPerAttack; i++)
        {
            if (startPhase != 1)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            Rigidbody2D currentBullet = Instantiate(spinShotPrefab, (Vector2)transform.position + spawnPos, Quaternion.identity).GetComponent<Rigidbody2D>();
            currentBullet.AddForceX(direction * spinShotForce, ForceMode2D.Impulse);
            currentBullet.transform.localScale = new Vector2(-currentBullet.transform.localScale.x, currentBullet.transform.localScale.y);
            SoundManager.Instance.PlaySound2D("LaserShoot");

            yield return new WaitForSeconds(spinShotInterval);
            direction = -direction;
            if (startPhase != 1)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            
            currentBullet = Instantiate(spinShotPrefab, (Vector2)transform.position + spawnPos, Quaternion.identity).GetComponent<Rigidbody2D>();
            currentBullet.AddForceX(direction * spinShotForce, ForceMode2D.Impulse);
            SoundManager.Instance.PlaySound2D("LaserShoot");

            direction = -direction;
            if (startPhase == 1 && spawnPos == Vector2.down)
            {
                spawnPos = Vector2.up;
            }
            else
            {
                spawnPos = Vector2.down;
            }
            yield return new WaitForSeconds(spinShotInterval);
            
        }
        if (phase == 1)
        {
            ani.SetBool("isShooting", false);
        }
        else
        {
            mechAnimation.PlayAnimation("isIdle");
        }
        yield return new WaitForSeconds(0.5f);
        mechAnimation.PlayTrigger("flyUp");
        rb.linearVelocityY = 75f;
        SoundManager.Instance.PlaySound2D("MechFly");

        // Jump back
        
        yield return new WaitUntil(() => rb.transform.position.y > BoundsTop + 8);
        yield return new WaitForSeconds(1f);

        rb.linearVelocityY = 0;
        rb.transform.position = new Vector2(startPos.x, transform.position.y);
        rb.gravityScale *= 3;

        mechAnimation.PlayAnimation("isFalling");
        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => rb.linearVelocity.y >= -0.1);
        mechAnimation.PlayTrigger("land");
        SoundManager.Instance.PlaySound2D("MechLand");

        rb.gravityScale = gravityScale;
        yield return new WaitForSeconds(0.5f);
        mechAnimation.PlayAnimation("isIdle");
        Debug.Log("Spin Shot Attack Finished");

        Invoke("ChooseAttack", attackCooldown);
    }

    IEnumerator Airstrike()
    {
        Vector2 startPos = transform.position;
        ani.SetBool("isIdle", false);

        // Jump to middle
        mechAnimation.PlayTrigger("flyUp");
        rb.linearVelocityY = 75f;
        SoundManager.Instance.PlaySound2D("MechFly");

        yield return new WaitUntil(() => rb.transform.position.y > BoundsTop + 8);
        yield return new WaitForSeconds(1f);
        rb.linearVelocityY = 0;

        rb.transform.position = new Vector2(BoundsCenterX, transform.position.y);
        rb.gravityScale *= 3;

        mechAnimation.PlayAnimation("isFalling");
        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => rb.linearVelocity.y >= -0.1);
        mechAnimation.PlayTrigger("land");
        SoundManager.Instance.PlaySound2D("MechLand");
        yield return new WaitForSeconds(0.4f);
        mechAnimation.PlayTrigger("readyMissiles");
        yield return new WaitForSeconds(0.5f);
        mechAnimation.PlayAnimation("isShootingMissiles");

        rb.gravityScale = gravityScale;
        
        // Airstrike
        for (int i = 0; i <= amount; i++)
        {


            Rigidbody2D currentAirstrike = Instantiate(airstrike, transform.position + (Vector3.up * 2), Quaternion.identity).GetComponent<Rigidbody2D>();

            Vector2 force = new Vector2(-22 + i * 45/amount, 60f);

            currentAirstrike.AddForce(force, ForceMode2D.Impulse);
            yield return new WaitUntil(() => currentAirstrike.transform.position.y > BoundsTop + 30);
            currentAirstrike.linearVelocity = Vector2.zero;
            Vector2 PlayerPos = player.transform.position;
            currentAirstrike.position = new Vector2(Random.Range(PlayerPos.x - 4, PlayerPos.x + 4), currentAirstrike.position.y);

            
            RaycastHit2D ray = Physics2D.Raycast(currentAirstrike.position, Vector2.down, 999, 1 << 6);
            Debug.DrawRay(currentAirstrike.position, Vector2.down * 999);
            Instantiate(warning, ray.point, Quaternion.identity);
            currentAirstrike.transform.rotation = Quaternion.Euler(currentAirstrike.transform.rotation.x,currentAirstrike.transform.rotation.y,currentAirstrike.transform.rotation.z + 180f);
            yield return null;

            
        }
        ani.SetBool("isShootingMissiles", false);
        
        yield return new WaitForSeconds(0.5f);
        mechAnimation.PlayTrigger("flyUp");
        rb.linearVelocityY = 75f;
        SoundManager.Instance.PlaySound2D("MechFly");
        // Jump back
        yield return new WaitUntil(() => rb.transform.position.y > BoundsTop + 8);
        yield return new WaitForSeconds(1f);

        rb.linearVelocityY = 0;
        rb.transform.position = new Vector2(startPos.x, transform.position.y);
        rb.gravityScale *= 3;

        mechAnimation.PlayAnimation("isFalling");
        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => rb.linearVelocity.y >= -0.1);
        mechAnimation.PlayTrigger("land");
        SoundManager.Instance.PlaySound2D("MechLand");

        rb.gravityScale = gravityScale;

        Debug.Log("Airstrike Attack Finished");

        Invoke("ChooseAttack", attackCooldown);
    }

    public void InitiatePhase2()
    {
        phase = 1;
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
 