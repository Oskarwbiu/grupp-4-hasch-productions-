using UnityEngine;

public class enemyAIJumpCrouch : MonoBehaviour
{

    RaycastHit2D jumpCast;
    GameObject crouchHit;
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        jumpCast = Physics2D.Raycast(transform.position, Vector2.up, 1f, LayerMask.GetMask("Player"));
        if (jumpCast.collider != null)
        {
            Jump(true);
        }
        else
        {
            Jump(false);
        }
        crouchHit = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - 0.5f), 0.3f, LayerMask.GetMask("Player"))?.gameObject;
        if (crouchHit != null)
        {
            Crouch(true);
        }
        else
        {
            Crouch(false);
        }
    }
    void Crouch(bool shouldCrouch)
    {

    }
    void Jump(bool shouldJump)
    {

    }
}

