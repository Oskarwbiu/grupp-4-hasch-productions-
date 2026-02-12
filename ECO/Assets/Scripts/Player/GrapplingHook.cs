using System.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    Rigidbody2D rb;
    LineRenderer lr;
    SpringJoint2D dj;
    public LayerMask grappleLayer;
    public LayerMask hitLayer;
    [SerializeField] float reelSpeed = 0.5f;
    [SerializeField] float maxDistance = 80f;
    [SerializeField] float pullForce = 0.5f;
    [SerializeField] float pullCooldown = 0.2f;
    [SerializeField] float stunDuration = 2f;
    [SerializeField] float minDistance = 2f;
    [SerializeField] GameObject visualIndicator;

    GameObject currentVisualIndicator;
    GameObject spriteObject;
    bool isGrappling;
    RaycastHit2D point;
    float moveInput;
    float jumpInput;
    Vector2 grappleDirection;
    bool isGrounded;
    Coroutine slowUpdate;
    Collider2D objectHit;
    bool canPull = true;
    bool isPulling;
    EnemyHealth hitAI;

    void Start()
    {
        currentVisualIndicator = Instantiate(visualIndicator);
        spriteObject = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        dj = GetComponent<SpringJoint2D>();
        dj.enabled = false;
        lr.enabled = false;
        slowUpdate = StartCoroutine(SlowUpdate());
    }


    IEnumerator SlowUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if (moveInput != 1 || !canPull)
            {

               Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());



                grappleDirection = (mousePos - (Vector2)transform.position).normalized;

                point = Physics2D.Raycast(transform.position, grappleDirection, maxDistance, hitLayer);
                objectHit = point.collider;

                if (objectHit != null && (grappleLayer.value & (1 << objectHit.gameObject.layer)) != 0)
                {

                    currentVisualIndicator.SetActive(true);
                    if (objectHit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        currentVisualIndicator.transform.position = point.transform.position;
                    }
                    else
                    {
                        currentVisualIndicator.transform.position = point.point;
                    }
                }
                else
                {
                    currentVisualIndicator.SetActive(false);
                    
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (objectHit != null)
        {
            if (objectHit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                currentVisualIndicator.transform.position = point.transform.position;
            }
        }
        if (moveInput == 1)
        {
            if (objectHit != null && (grappleLayer.value & (1 << objectHit.gameObject.layer)) != 0)
            {
                
                if (lr.enabled)
                {
                    
                    lr.SetPosition(0, transform.position);
                    lr.SetPosition(1, point.point);
                }

                Vector2 pullDirection = transform.position - objectHit.transform.position;
                if (objectHit.gameObject.layer == LayerMask.NameToLayer("Enemy") && canPull)
                {
                    isPulling = true;
                    lr.enabled = true;
                    canPull = false;
                    isGrappling = false;
                    objectHit.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(pullDirection.x * pullForce * 5 + rb.linearVelocityX, Mathf.Abs(pullDirection.x) + ((pullDirection.y * 3) * pullForce * 5 + rb.linearVelocityY)));
                   
                    

                    Invoke("disableGrapple", 0.5f);
                }
                else if (canPull)
                {
                    isPulling = false;
                    isGrappling = true;
                    lr.enabled = true;
                    dj.enabled = true;
                    dj.connectedAnchor = point.point;
                }
            }
            else
            {
                lr.enabled = false;
            }

        }
        else if (moveInput == 0)
        {
            dj.enabled = false;
            lr.enabled = false;
        }

        if (dj.distance >= minDistance && dj.enabled && jumpInput == 1)
        {
            dj.distance = dj.distance - reelSpeed;
        }
        RotatePlayer();

    }

    void disableGrapple()
    {
        objectHit = null;
        lr.enabled = false;
        currentVisualIndicator.SetActive(false);
        Invoke("EnablePull", pullCooldown);
    }
    void EnablePull()
    {
        canPull = true;
    }

    void RotatePlayer()
    {
        if (isGrappling && point.collider != null && !isPulling)
        {
            Vector2 direction = point.point - (Vector2)transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            spriteObject.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
        else
        {
            spriteObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        }
    }

    void OnGrapple(InputValue value)
    {
        isGrounded = GetComponent<PlayerJump>().IsGrounded();
        moveInput = value.Get<float>();
        if (moveInput == 1)
        {
            dj.distance = point.distance;
        }
        else if (moveInput == 0)
        {
            isGrappling = false;
        }
    }

    void OnReel(InputValue value)
    {
        jumpInput = value.Get<float>();

    }
    public bool IsGrappling()
    {
        return isGrappling;
    }
}
