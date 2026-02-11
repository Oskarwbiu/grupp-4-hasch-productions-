    using System.Collections;
using UnityEngine;

public class StationaryEnemyAI : MonoBehaviour
{
    //raycast
    [SerializeField] float detectionRange = 5f;
    [SerializeField] LayerMask detectionLayer;
    bool isLookingForPlayer = true;
    bool seePlayer = false;
    RaycastHit2D hit;
    GameObject player;

    //lookAround
    [SerializeField] WaitForSeconds lookaround;
    Rigidbody2D rb;
    Vector2 dir;
    Coroutine flipCoroutine;
    bool isFlipping = false;

    Vector2 directionToPlayer;

    PlayerHealth playerHealth;
    //lazer
    [SerializeField] private float defDistanceRay = 100f;
    public Transform laserFirePoint;
    public LineRenderer m_lineRenderer;
    Transform m_transform;
    [SerializeField] float lazerDamage = 1f;
    [SerializeField] float shootInterval = 0.5f;
    float shootTimer = 0f;
    [SerializeField] float laserPulseDuration = 0.1f;
    Coroutine laserPulseCoroutine;
    bool canShoot = true;
    [SerializeField] float stunCooldownDuration = 2f;
    Coroutine stunCooldownCoroutine;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }
    void ShootLazer()
    {
        if (laserFirePoint == null || directionToPlayer == Vector2.zero) return;

        RaycastHit2D _hit = Physics2D.Raycast(laserFirePoint.position, directionToPlayer, defDistanceRay, detectionLayer);
        Debug.DrawRay(laserFirePoint.position, directionToPlayer * defDistanceRay, Color.green, 0.5f);

        if (_hit.collider != null)
        {
            if (_hit.collider.CompareTag("Player"))
            {
                playerHealth.GetDamaged(lazerDamage);
            }

            Draw2DRay(laserFirePoint.position, _hit.point);
        }
        else
        {
            Draw2DRay(laserFirePoint.position, (Vector2)laserFirePoint.position + directionToPlayer * defDistanceRay);
        }

        // start pulse effect
        if (laserPulseCoroutine != null)
            StopCoroutine(laserPulseCoroutine);
        laserPulseCoroutine = StartCoroutine(LaserPulse());
    }
    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        if (m_lineRenderer == null) return;
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

    IEnumerator LaserPulse()
    {
        if (m_lineRenderer != null)
        {
            m_lineRenderer.enabled = true;
            yield return new WaitForSeconds(laserPulseDuration);
            m_lineRenderer.enabled = false;
        }
    }
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            if (m_lineRenderer != null)
                m_lineRenderer.enabled = false;
        }

    private void FixedUpdate()
    {
        Vision();

        if (seePlayer && player != null)
        {
            Getdirection();

            if (!CheckLineOfSightToPlayer())
            {
                ResetToLooking();
            }
            else if (canShoot)
            {
                shootTimer += Time.fixedDeltaTime;
                if (shootTimer >= shootInterval)
                {
                    ShootLazer();
                    shootTimer = 0f;
                }
            }
        }
        else
        {
            shootTimer = 0f;
        }
    }

    void Vision()
    {
        if (isLookingForPlayer)
        {
            bool found = false;
            for (int i = 0; i < 4; i++)
            {
                dir = Quaternion.Euler(0, 0, (i * 3) - 6f) * new Vector2(-Mathf.Sign(transform.localScale.x), 0);
                hit = Physics2D.Raycast(transform.position, dir, detectionRange, detectionLayer);

                Debug.DrawRay(transform.position, dir * detectionRange, Color.red, 0.05f);

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    player = hit.collider.gameObject;
                    isLookingForPlayer = false;
                    seePlayer = true;
                    Getdirection();
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                if (flipCoroutine == null)
                    flipCoroutine = StartCoroutine(FlipHorizontalDir());
            }
        }

    }

    void Getdirection()
    {
        if (seePlayer)
        {
            directionToPlayer = new Vector2(Mathf.Sign(player.transform.position.x - transform.position.x), 0);
        }
    }

    bool CheckLineOfSightToPlayer()
    {
        if (player == null) return false;
        Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;
        RaycastHit2D _hit = Physics2D.Raycast(transform.position, dirToPlayer, detectionRange, detectionLayer);
        Debug.DrawRay(transform.position, dirToPlayer * detectionRange, Color.yellow, 0.05f);
        if (_hit.collider != null && _hit.collider.CompareTag("Player"))
            return true;
        return false;
    }
    public void StunEnemy(float stunDuration)
    {
        StopAllCoroutines();
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        canShoot = false;
        if (m_lineRenderer != null)
            m_lineRenderer.enabled = false;
        Invoke("UnStun", stunDuration);
    }

    public void UnStun()
    {
        canShoot = true;
        // start cooldown before resuming player detection
        if (stunCooldownCoroutine != null)
            StopCoroutine(stunCooldownCoroutine);
        stunCooldownCoroutine = StartCoroutine(StunCooldown());
    }

    IEnumerator StunCooldown()
    {
        yield return new WaitForSeconds(stunCooldownDuration);
        // resume looking for player
        ResetToLooking();
        stunCooldownCoroutine = null;
    }

    

    void ResetToLooking()
    {
        player = null;
        seePlayer = false;
        isLookingForPlayer = true;
        directionToPlayer = Vector2.zero;
        // disable laser
        if (m_lineRenderer != null)
            m_lineRenderer.enabled = false;
        // start flipping if not already
        if (flipCoroutine == null)
            flipCoroutine = StartCoroutine(FlipHorizontalDir());
    }
    IEnumerator FlipHorizontalDir()
    {
        if (!isFlipping)
        {
            isFlipping = true;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            yield return new WaitForSeconds(2f);
            isFlipping = false;
            // allow starting again
            flipCoroutine = null;
        }
      
    }
    public Transform PlayerPos()
    {
        if (player != null)
        {
            return player.transform;
        }
        else
        {
            return null;
        }
    }


}

