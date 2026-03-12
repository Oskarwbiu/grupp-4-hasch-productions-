using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] float detectionRange = 5f;
    [SerializeField] LayerMask detectionLayer;
    

    Vector2 dir;
    RaycastHit2D hit;
    Rigidbody2D rb;
    TrafficLightAttack attackScript;
    GameObject player;
    private void Start()
    {
        attackScript = GetComponent<TrafficLightAttack>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        Vision();
        Friction();
    }
    void Friction()
    {

        if (Mathf.Abs(rb.linearVelocity.y) <= 0.1)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, rb.linearVelocity.y);
        }
    }

    void Vision()
    {
        for (int i = 0; i < 120; i++)
        {
            dir = Quaternion.Euler(0, 0, i * 3) * new Vector2(Mathf.Sign(-transform.localScale.x/Mathf.Abs(transform.localScale.x)), 0);
            hit = Physics2D.Raycast(transform.position, dir, detectionRange, detectionLayer);
            Debug.DrawRay(transform.position, dir * detectionRange, Color.red, 0.05f);
            if (hit.collider == null)
            {
                continue;
            }

            if (hit.collider.CompareTag("Player"))
            {
                player = hit.collider.gameObject;
                StartCoroutine(attackScript.AttackPlayer(player.transform.position, 0.5f));

            }

        }
        

    }
}
