using UnityEngine;

public class GrappleFlower : MonoBehaviour
{
    Animator ani;

    private void Start()
    {
        ani = GetComponent<Animator>();
        Debug.Log("started");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Open");
        if (collision.gameObject.CompareTag("Indicator"))
        {
            ani.SetTrigger("Open");
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Close");
        if (collision.gameObject.CompareTag("Indicator"))
        {
            ani.SetTrigger("Close");
           
        }
    }


}
