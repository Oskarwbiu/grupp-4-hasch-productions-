using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightEnemy : MonoBehaviour
{
    [SerializeField] float damage = 1f;
    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] float delay;
    [SerializeField] float intensity;
    [SerializeField] LayerMask detectionLayer;

    float currentSpeed;
    Rigidbody2D rb;
    Light2D enemyLight;
    GameObject player;
    bool isAttacking;
    bool die;
    bool lightOn = true;
    bool damagePlayer;
    bool hasDetectedPlayer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyLight = GetComponent<Light2D>();
        Vision();
        StartCoroutine(Move());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {
            Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    private void FixedUpdate()
    {
       Vision();

    }

    IEnumerator Move()
    {
        currentSpeed = speed;
        while (true)
        {
            if (!isAttacking && !die)
            {
                rb.linearVelocityX = currentSpeed;
                yield return new WaitForSeconds(1.5f);
                rb.linearVelocityX = 0;
                lightOn = false;
                enemyLight.intensity = 0;
                yield return new WaitForSeconds(1f);
                for (int i = 0; i < 5; i++)
                {
                    enemyLight.intensity = intensity;
                    yield return new WaitForSeconds(0.1f);
                    enemyLight.intensity = 0;
                    yield return new WaitForSeconds(0.1f);
                }
                
                enemyLight.intensity = intensity;
                lightOn = true;
                yield return new WaitForSeconds(0.5f);
                currentSpeed *= -1;
            }
        }
    }
    void Vision()
    {
        for (int i = 0; i < 15; i++)
        {
            
            Vector3 dir = Quaternion.Euler(0, 0, 67.5f + (i * 3)) * new Vector2(Mathf.Sign(-transform.localScale.x/Mathf.Abs(transform.localScale.x)), 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down, dir, range, detectionLayer);
            Debug.DrawRay(transform.position + Vector3.down, dir * range, Color.red);
            if (hit.collider == null || isAttacking || damagePlayer)
            {
                continue;
            }
            if (hit.collider.CompareTag("Player") && !hasDetectedPlayer && lightOn)
            {
                hasDetectedPlayer = true;
                StopAllCoroutines();
                StartCoroutine(Attack());
                player = hit.collider.gameObject;
                Debug.Log("Player detected!");
            }
            
        }
        
    }

    IEnumerator Attack()
    {
        enemyLight.color = Color.red;
        isAttacking = true;
        rb.linearVelocityX = 0;
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        rb.AddForce((player.transform.position - transform.position).normalized * speed * 7.5f, ForceMode2D.Impulse);
        damagePlayer = true;
        yield return new WaitForSeconds(0.5f);
        die = true;
    }
    private void OnBecameInvisible()
    {
        if (die)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && damagePlayer)
        {
            FindFirstObjectByType<PlayerHealth>().GetDamaged(damage);
            damagePlayer = false;
        }
    }
}
