using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GrappleFlower : MonoBehaviour
{
    Animator ani;
    Light2D spotLight;


    float startvalue;
    float endvalue;
    private void Start()
    {
        ani = GetComponent<Animator>();
        spotLight = GetComponent<Light2D>();
        spotLight.intensity = 0f;

    }

    private void Update()
    {
        float intensity = Mathf.Lerp(startvalue, endvalue, Mathf.PingPong(Time.time, 1));
        spotLight.intensity = intensity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Indicator"))
        {
            ani.SetTrigger("Open");
            startvalue = 0.5f;
            endvalue = 1.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Indicator"))
        {
            ani.SetTrigger("Close");
            startvalue = 0;
            endvalue = 0;
        }
    }


}
