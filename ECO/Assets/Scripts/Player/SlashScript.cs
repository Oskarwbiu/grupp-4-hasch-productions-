using Unity.VisualScripting;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
    Animator ani;
    void Start()
    {
        ani = GetComponent<Animator>();
        float duration = ani.GetCurrentAnimatorStateInfo(0).length;
        Invoke("Suicide", duration);
    }

  void Suicide()
    {
        Destroy(gameObject);
    }
}
