using UnityEngine;

public class ScientistScript : MonoBehaviour
{
    [SerializeField] Collider2D areaBounds;


    dialogueManager dialogue;
    Animator ani;
    Rigidbody2D rb;

    private void Start()
    {
        dialogue = FindFirstObjectByType<dialogueManager>();
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    public void GetDamaged()
    {
        rb.GetComponent<Collider2D>().enabled = false;
        dialogue.EndDialogue();
        rb.gravityScale = 0;
        transform.position += Vector3.up;
        rb.transform.localScale = new Vector3(2, 2, 2);
        rb.linearVelocityX = 10;
        areaBounds.enabled = true;
        ani.SetBool("crawl", true);
    }
}
