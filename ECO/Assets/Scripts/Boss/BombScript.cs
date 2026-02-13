using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField] float bombRadius;
    [SerializeField] GameObject explosion;
    [SerializeField] float explosionDamage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(explosion, gameObject.transform.position, Quaternion.Euler(325, 0, 0));
        SoundManager.Instance.PlaySound2D("MissileShoot");

        Collider2D[] bombField = Physics2D.OverlapCircleAll(transform.position, bombRadius);

        foreach (Collider2D hitObjects in bombField)
        {
            if (hitObjects.CompareTag("Player"))
            {
                PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
                if (health != null)
                {
                    health.GetDamaged(explosionDamage);
                }
            }
        }

        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Warning"))
        {
            Destroy(collision.gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bombRadius);
    }
}
