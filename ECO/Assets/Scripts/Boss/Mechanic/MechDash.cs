using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MechDash : MonoBehaviour
{
    [SerializeField] float dashForce = 25f;
    [SerializeField] float damage = 1f;

    Collider2D arenaBounds;
    MechAnimation mechAnimation;
    Animator ani;
    Rigidbody2D rb;
    PlayerHealth playerHealth;

    float BoundsTop => arenaBounds.bounds.max.y;
    float BoundsBottom => arenaBounds.bounds.min.y;
    float BoundsRight => arenaBounds.bounds.max.x;
    float BoundsLeft => arenaBounds.bounds.min.x;
    float BoundsCenterX => arenaBounds.bounds.center.x;
    float BoundsCenterY => arenaBounds.bounds.center.y;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        mechAnimation = GetComponent<MechAnimation>();
        arenaBounds = GameObject.FindWithTag("BossBounds").GetComponent<Collider2D>();

        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        float dir = BoundsCenterX - transform.position.x;
        float destination1 = Mathf.Sign(dir) == 1f ? BoundsRight - 2.5f : BoundsLeft + 2.5f;
        float destination2 = Mathf.Sign(dir) == 1f ? BoundsLeft + 2.5f : BoundsRight - 2.5f;
        

        mechAnimation.PlayAnimation("readyDash");
        yield return new WaitForSeconds(0.4f);
        SoundManager.Instance.PlaySound2D("MissileReady");
        yield return new WaitForSeconds(0.6f);
        ani.SetBool("readyDash", false);
        mechAnimation.PlayTrigger("dash");
        SoundManager.Instance.PlaySound2D("MechDash");
        rb.linearVelocityX = Mathf.Sign(dir) * dashForce;

  


        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - destination1) <= 1);
        rb.linearVelocityX = 0;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        mechAnimation.PlayAnimation("readyDash");
        SoundManager.Instance.PlaySound2D("MissileReady");
        yield return new WaitForSeconds(0.6f);
        ani.SetBool("readyDash", false);
        mechAnimation.PlayTrigger("dash");

        rb.linearVelocityX = -Mathf.Sign(dir) * dashForce * 2;
        SoundManager.Instance.PlaySound2D("MechDash");
        

        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - destination2) <= 1);
        rb.linearVelocityX = 0;
        mechAnimation.PlayAnimation("isIdle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerHealth == null)
            {
                playerHealth = FindFirstObjectByType<PlayerHealth>();
            }
            playerHealth.GetDamaged(damage);
        }
    }



}
