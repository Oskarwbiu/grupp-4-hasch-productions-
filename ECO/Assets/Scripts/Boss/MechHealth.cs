using UnityEngine;

public class MechHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 51f;
    [SerializeField] float invincibilityDuration = 1f;

    bool isInvincible = false;
    Animator ani;

    private void Start()
    {
        currentHealth = maxHealth;
        ani = GetComponent<Animator>();
    }
    public void TakeDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        isInvincible=true;
        Invoke("ResetInvIncibility", invincibilityDuration);

        if (currentHealth <= maxHealth/2)
        {
            GetComponent<MechAttack>().InitiatePhase2();
        }

        if (currentHealth <= 0)
        {
            GetComponent<MechAttack>().StopAllCoroutines();
            GetComponent<MechAttack>().enabled = false;
            GetComponent<MechAnimation>().enabled = false;
            FindFirstObjectByType<EndScreen>().ShowScreen();
            ani.SetTrigger("Die");
            float duration = ani.GetCurrentAnimatorStateInfo(0).length;
        
            Invoke("Die", duration);
        }


    }

    void ResetInvIncibility()
    {
        isInvincible = false ;
    }

    void Die()
    {
        MusicManager.Instance.PlayMusic("Backrooms", 1);
        Destroy(gameObject);
    }
    



}
