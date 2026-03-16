using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCheats : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rb;
    public float flySpeed = 5;
    float gravity;
    bool isNoClipping = false;
    public bool isGodMode = false;
    public bool canLevelskip = false;

    private void Start()
    {
        Debug.Log("GodMode Pref: " + PlayerPrefs.GetInt("GodMode", 0));
        Debug.Log("LevelSkip Pref: " + PlayerPrefs.GetInt("LevelSkip", 0));
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

    }

    private void Update()
    {
        if (isNoClipping)
        {
            rb.linearVelocity = moveInput * flySpeed/3;
        }
        
    }



    public void NoClip(bool enabled)
    {
        if (enabled)
        {
            isNoClipping = true;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            GetComponent<PlayerJump>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log("noclip on");
        }
        else
        {
            isNoClipping = false;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = gravity;
            GetComponent<PlayerJump>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
            Debug.Log("noclip off");
        }
    }

    public void GodMode(bool enabled)
    {
        if (enabled)
        {
            Debug.Log("godmode on");
        }
        else
        {
            Debug.Log("godmode off");
        }
        isGodMode = enabled;
    }
   
    public bool NoClipBool()
    {
        return isNoClipping;
    }
        
    void OnSkipLevel()
    {
        Debug.Log("skip level = " + canLevelskip);
        if (canLevelskip)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
            CheckpointManager.Instance.ResetCheckpoints();
        }
    }


}
