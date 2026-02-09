using System.Collections;
using UnityEngine;

public class StationaryEnemyAI : MonoBehaviour
{
    [SerializeField] float lookaroundInterval = 0.5f;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] LayerMask detectionLayer;
    bool isLookingForPlayer = true;
    bool seePlayer = false;
    [SerializeField] WaitForSeconds lookaround;
    Rigidbody2D rb;
    RaycastHit2D hit;
    GameObject player;
    Vector2 dir;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        Vision();

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
                if (hit.collider == null)
                {

                    continue;
                }

                if (hit.collider.CompareTag("Player"))
                {
                    rb.linearVelocityX = 0;
                    player = hit.collider.gameObject;

                }

            }
        }

        if (player == null)
        {
            Debug.Log("No Player Detected");
            FlipHorizontalDir();
        }
        if (player != null)
        {
                seePlayer = true;
                isLookingForPlayer = false;
                Getdirection();
                Debug.Log("Player Detected");
        }

    }

    void Getdirection()
    {
        if (isLookingForPlayer && seePlayer)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        }
    }

    IEnumerator FlipHorizontalDir()
    {
        Debug.Log("oiageh");
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        yield return new WaitForSeconds(2f);
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

