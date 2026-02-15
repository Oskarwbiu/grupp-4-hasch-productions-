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
        AnimatorStateInfo currentClip = ani.GetCurrentAnimatorStateInfo(0);


        if (currentClip.IsName("flyUp") || currentClip.IsName("flyDown") || currentClip.IsName("land") || currentClip.IsName("Die") || currentClip.IsName("readyCannons") || currentClip.IsName("dash") || currentClip.IsName("readyMissiles"))
        {

            if (currentClip.normalizedTime < 1.0f)
            {
                return;
            }

        }

        string[] paramsToReset = { "isIdle", "isShooting", "isFalling", "isFlying", "isShootingMissiles", "readyDash" };

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
        ani.ResetTrigger(newParam);
        ani.SetTrigger(newParam);
    }
}
