using UnityEngine;

public class ScientistScript : MonoBehaviour
{
    [SerializeField] Collider2D areaBounds;

    Animator ani;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        MusicManager.Instance.PlayMusic("Cloudy");
    }

    public void GetDamaged()
    {
        rb.GetComponent<Collider2D>().enabled = false;
        rb.gravityScale = 0;
        transform.position += Vector3.up;
        rb.transform.localScale = new Vector3(2, 2, 2);
        rb.linearVelocityX = 10;
        areaBounds.enabled = true;
        ani.SetBool("crawl", true);
    }
}
