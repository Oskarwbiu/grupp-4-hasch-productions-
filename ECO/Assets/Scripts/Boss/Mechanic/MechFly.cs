using System.Collections;
using UnityEngine;

public class MechFly : MonoBehaviour
{
    [SerializeField] GameObject bombPrefab;
    [SerializeField] float bombDropInterval = 0.5f;
    [SerializeField] float flySpeed = 5f;

    Collider2D arenaBounds;
    Rigidbody2D rb;

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
        StartCoroutine(Fly());
    }

    IEnumerator Fly()
    {
        while (true)
            {
            rb.linearVelocityX = -flySpeed * 2;

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
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
