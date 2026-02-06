using System.Collections;
using UnityEngine;

public class SpiritFollow : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Transform player;
    Coroutine updatePlayerPos;
    Vector3 playerPos;
    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        playerPos = new Vector3(player.position.x, player.position.y + 1, 0);
        Vector2 direction = (rb.transform.position - playerPos);
        if (transform.position.x - playerPos.x > 2 || transform.position.x - playerPos.x < -2)
        {
            rb.linearVelocityX = -direction.x * moveSpeed;
        }
        else
        {
            rb.linearVelocityX = 0;
        }
        if (transform.position.y - playerPos.y > 0.5 || transform.position.y - playerPos.y < -0.5)
        {
            rb.linearVelocityY = -direction.y * moveSpeed;
        }
        else
        {
            rb.linearVelocityY = -direction.y;
        }
    }

}