using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField] float bombRadius;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] float explosionDamage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(explosion, gameObject.transform.position, Quaternion.Euler(325, 0, 0));
        

        Collider2D[] bombField = Physics2D.OverlapCircleAll(transform.position, bombRadius);

        foreach (Collider2D hitObjects in bombField)
        {
            if (hitObjects.CompareTag("Player"))
            {
                FindFirstObjectByType<PlayerHealth>().GetDamaged(explosionDamage);
            }
        }

        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bombRadius);
    }
}
