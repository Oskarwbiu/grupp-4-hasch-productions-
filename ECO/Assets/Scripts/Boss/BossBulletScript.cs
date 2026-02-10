using UnityEngine;

public class BossBulletScript : MonoBehaviour
{
    [SerializeField] float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindAnyObjectByType<PlayerHealth>().GetDamaged(damage);
        }
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
