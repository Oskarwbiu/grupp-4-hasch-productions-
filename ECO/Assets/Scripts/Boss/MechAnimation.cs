using Unity.VisualScripting;
using UnityEngine;

public class MechAnimation : MonoBehaviour
{
    Animator ani;
    private void Start()
    {
        ani = GetComponent<Animator>();
    }
    public void PlayAnimation(string newParam)
    {

        string[] paramsToReset = { "isIdle", "isShooting", "isFalling", "isFlying"};

        if (newParam != "" && ani.GetBool(newParam))
        {
            return;
        }




        foreach (var p in paramsToReset)
        {

            ani.SetBool(p, p == newParam);
        }
    }

    public void PlayTrigger(string newParam)
    {
        ani.SetTrigger(newParam);
    }
}
