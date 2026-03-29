using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MechanicAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float attackspeed = 1.0f;
    [SerializeField] float attackCooldown = 2f;
    [SerializeField] float damage = 1f;
    [SerializeField] Collider2D arenaBounds;
    [Header("Fly Dash")]
    [SerializeField] float dashFlySpeed = 10f;
    [SerializeField] float dashDelay = 1f;
    [SerializeField] GameObject bullet;
    [Header("Car & Mech Summon")]
    [SerializeField] GameObject mechAirStrike;
    [SerializeField] GameObject carOilLeak;
    [SerializeField] float carMechDuration = 10f;
    [Header("Car & Mech Summon")]
    [SerializeField] GameObject mechDash;
    [SerializeField] GameObject mechFly;
    [SerializeField] GameObject trafficMinion;
    [SerializeField] float mechDuration = 10f;

    bool isDamaging;
    bool hasDamaged;
    bool activateShockwave;
    int lastAttack;
    int phase;
    Rigidbody2D rb;
    GameObject player;
    ShakeManager cameraShake;
    Animator ani;
    Coroutine currentAttack;
    PlayerHealth playerHealth;
    LineRenderer lr;
    float BoundsTop => arenaBounds.bounds.max.y;
    float BoundsBottom => arenaBounds.bounds.min.y;
    float BoundsRight => arenaBounds.bounds.max.x;
    float BoundsLeft => arenaBounds.bounds.min.x;
    float BoundsCenterX => arenaBounds.bounds.center.x;
    float BoundsCenterY => arenaBounds.bounds.center.y;

    private void Start()
    {
        arenaBounds = GameObject.FindWithTag("BossBounds").GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        cameraShake = FindFirstObjectByType<ShakeManager>();
        ani = GetComponent<Animator>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        ChooseAttack();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDamaging && !hasDamaged && playerHealth != null && collision.gameObject.CompareTag("Player"))
        {
            hasDamaged = true;
            playerHealth.GetDamaged(damage);
            Invoke("ResetDamage", attackspeed);
        }
    }

    void ResetDamage()
    {
        hasDamaged = false;
    }

    private void FixedUpdate()
    {
        if (isDamaging)
        {
            float xDistance = Mathf.Abs(transform.position.x - player.transform.position.x);
            float yDistance = Mathf.Abs(transform.position.y - player.transform.position.y);
            if (xDistance <= 0.5f && yDistance <= 1f && !hasDamaged)
            {
                hasDamaged = true;
                playerHealth.GetDamaged(damage);
                Invoke("ResetDamage", attackspeed);
            }
        }
    }

    void ChooseAttack()
    {
        transform.localScale = new Vector3(-1, 1, 1);
        int attack = Random.Range(0, 2);
        if (lastAttack == attack)
        {
            ChooseAttack();
            return;
        }
        else
        {
            lastAttack = attack;
        }

        switch (attack)
        {
            case 0:
                currentAttack = StartCoroutine(FlyDash());
                break;
            case 1:
                currentAttack = StartCoroutine(CarMechSummon());
                break;

        }

    }

    IEnumerator FlyDash()
    {
        rb.linearVelocityY = dashFlySpeed;
        yield return new WaitUntil(() => rb.transform.position.y > BoundsTop + 8);

        rb.transform.position = new Vector3(BoundsRight + 5, BoundsBottom + 2, 0);
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashDelay);

        isDamaging = true;
        rb.linearVelocityX = -dashFlySpeed * 5;

        yield return new WaitUntil(() => transform.position.x < BoundsLeft - 5f);

        rb.linearVelocityX = 0f;

        yield return new WaitForSeconds(dashDelay);

        rb.linearVelocityX = dashFlySpeed * 5;

        yield return new WaitUntil(() => transform.position.x > BoundsRight + 5f);

        yield return new WaitForSeconds(dashDelay + 1);

        rb.transform.position = new Vector3(BoundsCenterX, BoundsTop + 5, 0);

        rb.linearVelocityX = 0;
        rb.linearVelocityY = -dashFlySpeed * 5;

        yield return new WaitUntil(() => transform.position.y <= BoundsBottom + 1f);

        ShootBullet(-1f);
        ShootBullet(1f);


        isDamaging = false;
        rb.transform.position = new Vector2(BoundsCenterX, BoundsBottom + 1f);
        rb.linearVelocity = Vector2.zero;

        Debug.Log("Fly Dash Finished");

        Invoke("ChooseAttack", attackCooldown);
    }

    void ShootBullet(float direction)
    {
        Rigidbody2D currentBullet = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        currentBullet.AddForceX(direction * dashFlySpeed * 5, ForceMode2D.Impulse);
        currentBullet.transform.localScale = new Vector2(-currentBullet.transform.localScale.x, currentBullet.transform.localScale.y);
        SoundManager.Instance.PlaySound2D("LaserShoot");
    }

    IEnumerator CarMechSummon()
    {
        Vector3 spawnPos = new Vector3(BoundsLeft + 3, BoundsCenterY);


        Instantiate(mechAirStrike, spawnPos, Quaternion.identity);

        spawnPos = new Vector3(BoundsRight - 3, BoundsCenterY);

        Instantiate(carOilLeak, spawnPos, Quaternion.identity);
        

        yield return new WaitForSeconds(carMechDuration);

        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minions");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }
        Invoke("ChooseAttack", attackCooldown);
    }

    IEnumerator MechSummon()
    {
        Vector3 spawnPos = new Vector3(BoundsLeft + 3, BoundsCenterY);


        Instantiate(mechDash, spawnPos, Quaternion.identity);

        spawnPos = new Vector3(BoundsRight - 3, BoundsCenterY);

        Instantiate(mechDash, spawnPos, Quaternion.identity);

        Instantiate(mechFly, spawnPos, Quaternion.identity);


        yield return new WaitForSeconds(mechDuration);

        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minions");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }

        yield return new WaitForSeconds(1.5f);

        Instantiate(trafficMinion, spawnPos, Quaternion.identity);

        spawnPos = new Vector3(BoundsLeft + 3, BoundsCenterY);

        Instantiate(trafficMinion, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        minions = GameObject.FindGameObjectsWithTag("Minions");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }

        Invoke("ChooseAttack", attackCooldown);
    }

}
