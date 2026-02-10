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

    //lazer
    [SerializeField] private float defDistanceRay = 100f;
    public Transform laserFirePoint;
    public LineRenderer m_lineRenderer;
    Transform m_transform;
    [SerializeField] float lazerDamage = 1f;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();

    }
    void ShootLazer()
    {
            RaycastHit2D _hit = Physics2D.Raycast(laserFirePoint.position, directionToPlayer, defDistanceRay, detectionLayer);
            Debug.DrawRay(laserFirePoint.position, directionToPlayer * defDistanceRay, Color.green, 0.5f);
            Debug.DrawRay(laserFirePoint.position, _hit.point);

        if (_hit.collider != null && _hit.collider.CompareTag("Player"))
            {
                FindFirstObjectByType<PlayerHealth>().GetDamaged(lazerDamage);
                Draw2DRay(laserFirePoint.position, _hit.point);
        }
    
    }
    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
    m_lineRenderer.SetPosition(0, startPos);
    m_lineRenderer.SetPosition(1, endPos);
    }
        void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vision();
        ShootLazer();

    }

    void Vision()
    {
        if (isLookingForPlayer)
        {
             for (int i = 0; i < 12; i++)
            { 
                dir = Quaternion.Euler(0, 0, (i * 3) - 19f) * new Vector2(Mathf.Sign(transform.localScale.x / Mathf.Abs(transform.localScale.x)), 0);
                hit = Physics2D.Raycast(transform.position, dir, detectionRange, detectionLayer);

                Debug.DrawRay(transform.position, dir * detectionRange, Color.red, 0.05f);
                if (player == null)
                {
                    flipCoroutine = StartCoroutine(FlipHorizontalDir());
                    continue;
                }

                if (hit.collider.CompareTag("Player"))
                {
                    player = hit.collider.gameObject;
                    isLookingForPlayer = false;
                    seePlayer = true;
                    Getdirection();
                    }

            }
        }

    }

    void Getdirection()
    {
        if (seePlayer)
        {
            directionToPlayer = (player.transform.position - transform.position).normalized;
        }
    }
    void ShootPlayer()
    {
        if (seePlayer)
        {

        }
    }

    IEnumerator FlipHorizontalDir()
    {
        if (!isFlipping)
        {
            isFlipping = true;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y); 
            yield return new WaitForSeconds(2f);
            isFlipping = false;
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

