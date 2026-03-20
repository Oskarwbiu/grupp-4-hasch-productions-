using System.Collections;
using UnityEngine;

public class FireAttack : MonoBehaviour
{
    [SerializeField] public float attackDuration = 5f;
    [SerializeField] private float damage = 1;
    [SerializeField] private float particleEmissionInterval = 0.5f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private ParticleSystem fireParticleSystem;
    [SerializeField] private ParticleSystem fireParticleSystem2;
    [SerializeField] LayerMask attackLayer;

    Rigidbody2D rb;
    public bool isAttacking = false;
    EnemyHealth health;

    private void Start()
    {
        health = GetComponent<EnemyHealth>();
    }
    public IEnumerator Attack()
    {
        Coroutine particleCoroutine = StartCoroutine(EmitParticles());
        yield return new WaitForSeconds(0.2f);
        isAttacking = true;
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
        StopCoroutine(particleCoroutine);
    }

    private void FixedUpdate()
    {
        if (health.isStunned)
        {
           return;
        }
        if (isAttacking)
        {
            Damage();
        }
    }

    void Damage()
    {
        for (int i = 0; i < 12; i++)
        {
            Vector2 dir = Quaternion.Euler(0, 0, (i * 3) -19f) * new Vector2(Mathf.Sign(transform.localScale.x/Mathf.Abs(transform.localScale.x)), 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, attackRange, attackLayer);
            Debug.DrawRay(transform.position, dir * attackRange, Color.red, 0.05f);
            if (hit.collider == null)
            {

                continue;
            }

            if (hit.collider.CompareTag("Player"))
            {
                FindFirstObjectByType<PlayerHealth>().GetDamaged(damage);

            }
        }
    }

    IEnumerator EmitParticles()
    {
        while (true)
        {
            if (health.isStunned)
            {
                yield return null;
                continue;
            }
            ParticleSystem particle =Instantiate(fireParticleSystem, transform.position, fireParticleSystem.transform.rotation);
            var shape = particle.shape;
            shape.rotation = new Vector3(0, Mathf.Sign(transform.localScale.x) * 90, 0);
            yield return new WaitForSeconds(particleEmissionInterval);
            particle = Instantiate(fireParticleSystem2, transform.position, fireParticleSystem2.transform.rotation);
            shape = particle.shape;
            shape.rotation = new Vector3(0, Mathf.Sign(transform.localScale.x) * 90, 0);
            yield return new WaitForSeconds(particleEmissionInterval);
        }
    }


}
