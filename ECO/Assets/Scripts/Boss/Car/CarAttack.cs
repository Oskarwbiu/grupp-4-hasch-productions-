using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CarAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float attackspeed = 1.0f;
    [SerializeField] float attackCooldown = 2f;
    [SerializeField] Collider2D arenaBounds;
    [SerializeField] int phase2Multiplier = 2;
    [Header("Dash Attack")]
    [SerializeField] float dashForce = 25f;
    [SerializeField] float knockBack = 2f;
    [SerializeField] float dashingDamage = 2f;
    [SerializeField] int amountOfDashes = 1;
    [SerializeField] float crashedDuration = 2f;
    [Header("Traffic Attack")]
    [SerializeField] GameObject[] trafficLights;
    [SerializeField] int amountOfAttacks = 3;
    [SerializeField] float delayBetweenAttacks = 1f;
    [Header("Oil Leak")]
    [SerializeField] GameObject oilBlob;
    [SerializeField] AnimationCurve trajectory;
    [SerializeField] AnimationCurve axisCorrection;
    [SerializeField] AnimationCurve oilSpeed;
    [SerializeField] float damagePerBlob = 1f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileMaxHeight = 10f;
    [SerializeField] int amountProjectiles = 5;
    [SerializeField] float delayBetweenProjectiles = 1f;

    Rigidbody2D rb;
    Coroutine currentAttack;
    GameObject player;
    int lastAttack = -1;
    bool isDashing = false;
    float currentDamage = 1;
    bool isTouchingPlayer = false;
    bool canAttack = true;
    bool hasHitPlayer = false;

    float BoundsTop => arenaBounds.bounds.max.y;
    float BoundsBottom => arenaBounds.bounds.min.y;
    float BoundsRight => arenaBounds.bounds.max.x;
    float BoundsLeft => arenaBounds.bounds.min.x;
    float BoundsCenterX => arenaBounds.bounds.center.x;
    float BoundsCenterY => arenaBounds.bounds.center.y;


    private void Start()
    {
        trafficLights = GameObject.FindGameObjectsWithTag("Minions");
        arenaBounds = GameObject.FindWithTag("BossBounds").GetComponent<Collider2D>();
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        Invoke("ChooseAttack", 1f);
    }

    private void FixedUpdate()
    {
        if (isTouchingPlayer && canAttack && isDashing)
        {
            canAttack = false;
            hasHitPlayer = true;
            PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.GetDamaged(currentDamage, 0.4f, 0.4f, 3f);
            }
            Debug.Log("damage player");
            if (isDashing)
            {
                Vector2 knockBackForce = new Vector2(Mathf.Sign(rb.linearVelocity.x), 0) * knockBack + (Vector2.up * (knockBack / 2));
                player.GetComponent<Rigidbody2D>().AddForce(knockBackForce);
            }
            Invoke("ResetAttack", attackspeed);
        }
        isTouchingPlayer = false;
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void ChooseAttack()
    {
        transform.localScale = new Vector3(-1, 1, 1);
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
                currentAttack = StartCoroutine(TrafficAttack());
                break;
            case 2:
                currentAttack = StartCoroutine(OilLeak());
                break;



        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
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

    IEnumerator DashAttack()
    {
        


        isDashing = true;
        currentDamage = dashingDamage;
        for (int i = 0; i < amountOfDashes; i++)
        {
            yield return new WaitForSeconds(0.6f);
            rb.linearVelocityX = -dashForce;
            yield return new WaitUntil(() => transform.position.x < BoundsLeft + 5f);
            rb.linearVelocityX = 0;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            yield return new WaitForSeconds(0.6f);
            rb.linearVelocityX = dashForce * 2;

            yield return new WaitUntil(() => transform.position.x > BoundsRight - 2.5f);
            rb.linearVelocityX = 0;
        }

        Debug.Log("DashAttack Finished");

        rb.linearVelocityX = 0;
        currentDamage = 1;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        isDashing = false;

        if (!hasHitPlayer)
        {
            Debug.Log("Car Crashed");
            yield return new WaitForSeconds(crashedDuration);
        }
        hasHitPlayer = false;

        Invoke("ChooseAttack", attackCooldown);
    }

    IEnumerator TrafficAttack()
    {
        for (int i = 0; i < amountOfAttacks - 1; i++)
        {
            yield return new WaitForSeconds(delayBetweenAttacks);
            ShootLights(true);
            yield return new WaitForSeconds(0.15f);
            ShootLights(false);
            
        }
        yield return new WaitForSeconds(delayBetweenAttacks/2);
        ShootLights(true);
        yield return new WaitForSeconds(0.15f);
        ShootLights(false);


        Debug.Log("TrafficAttack Finished");
        Invoke("ChooseAttack", attackCooldown);
    }

    public void ShootLights(bool shoot)
    {
        foreach (GameObject light in trafficLights)
        {
            if (shoot)
            {
                light.GetComponent<TrafficLight>().detectionRange = 100f;
            }
            else
            {
                light.GetComponent<TrafficLight>().detectionRange = 0.1f;
            }
            
        }
    }


    IEnumerator OilLeak()
    {
        for (int i = 0; i < amountProjectiles; i++)
        {
            yield return new WaitForSeconds(delayBetweenProjectiles);
            Vector2 playerPos = player.transform.position;
            GameObject currentBlob = Instantiate(oilBlob, transform.position, Quaternion.identity);
            currentBlob.GetComponent<OilScript>().InitializeProjectile(playerPos, projectileSpeed, trajectory, axisCorrection, oilSpeed, projectileMaxHeight, damagePerBlob);
        }

        Debug.Log("WheelThrow Finished");
        Invoke("ChooseAttack", attackCooldown);
    }

    public void InitiatePhase2()
    {
        amountProjectiles *= phase2Multiplier;
        amountOfAttacks *= phase2Multiplier;
        amountOfDashes *= phase2Multiplier;

    }

}
