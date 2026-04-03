using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class MechanicAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float attackspeed = 1.0f;
    [SerializeField] float attackCooldown = 2f;
    [SerializeField] float damage = 1f;
    [SerializeField] Collider2D arenaBounds;
    [Header("Fly Dash")]
    [SerializeField] float dashFlySpeed = 10f;
    [SerializeField] float dashDelay = 1f;
    [SerializeField] GameObject bullet;
    [Header("Hell From Above")]
    [SerializeField] GameObject mechAirStrike;
    [SerializeField] GameObject carOilLeak;
    [SerializeField] GameObject fridge;
    [SerializeField] float minAmountOfFridges = 8f;
    [SerializeField] float maxAmountOfFridges = 8f;
    [SerializeField] float carMechDuration = 10f;
    [Header("Mech Summon")]
    [SerializeField] GameObject mechDash;
    [SerializeField] GameObject mechFly;
    [SerializeField] GameObject trafficMinion;
    [SerializeField] float mechDuration = 10f;
    [Header("Double Spin Shot")]
    [SerializeField] GameObject mechSpin;
    [SerializeField] float flySpeed = 5f;
    [SerializeField] float spinDuration = 10f;

    float dashCounter = 0;
    bool isDamaging;
    bool hasDamaged;
    bool activateShockwave;
    int lastAttack;
    int phase;
    MechanicHealth healthScript;
    Rigidbody2D rb;
    GameObject player;
    ShakeManager cameraShake;
    Animator ani;
    Coroutine currentAttack;
    PlayerHealth playerHealth;
    BoxCollider2D hitbox;
    float BoundsTop => arenaBounds.bounds.max.y;
    float BoundsBottom => arenaBounds.bounds.min.y;
    float BoundsRight => arenaBounds.bounds.max.x;
    float BoundsLeft => arenaBounds.bounds.min.x;
    float BoundsCenterX => arenaBounds.bounds.center.x;
    float BoundsCenterY => arenaBounds.bounds.center.y;

    private void Start()
    {
        healthScript = GetComponent<MechanicHealth>();
        arenaBounds = GameObject.FindWithTag("BossBounds").GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        cameraShake = FindFirstObjectByType<ShakeManager>();
        ani = GetComponent<Animator>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        hitbox = GetComponent<BoxCollider2D>();
        StartCoroutine(StartingSequence());
    }

    IEnumerator StartingSequence()
    {
        transform.position = new Vector2(BoundsCenterX, BoundsCenterY);
        yield return new WaitForSeconds(1.5f);
        ChooseAttack();
    }





    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDamaging && !hasDamaged && playerHealth != null && collision.gameObject.CompareTag("Player"))
        {
            hasDamaged = true;
            playerHealth.GetDamaged(damage);
            Invoke("ResetDamage", attackspeed);
        }
    }


    public void IsHurt()
    {
        StopAllCoroutines();
        ResetToCenter();
        
    }

    void ResetDamage()
    {
        hasDamaged = false;
    }

    private void FixedUpdate()
    {
        if (healthScript.isDead)
        {
            GameObject[] minions = GameObject.FindGameObjectsWithTag("Minions");
            foreach (GameObject minion in minions)
            {
                Destroy(minion);
            }
                StopAllCoroutines();

        }
        if (isDamaging)
        {
            float xDistance = Mathf.Abs(transform.position.x - player.transform.position.x);
            float yDistance = Mathf.Abs(transform.position.y - player.transform.position.y);
            if (xDistance <= 0.5f && yDistance <= 1f && !hasDamaged)
            {
                hasDamaged = true;
                playerHealth.GetDamaged(damage);
                Invoke("ResetDamage", attackspeed);
            }
        }
    }

    void ResetToCenter()
    {
        ani.SetTrigger("StopAttack");
        healthScript.ResetInvIncibility(true);
        Invoke("ChooseAttack", attackCooldown);
    }

    void ChooseAttack()
    {
        int attack = Random.Range(0, 3);
        if (lastAttack == attack)
        {
            ChooseAttack();
            return;
        }
        else
        {
            lastAttack = attack;
        }
        if (dashCounter >= 5f)
        {
            attack = 0;
        }

        Debug.Log("Attack: " + attack);

        if (healthScript.isDead)
        {
            return;
        }

        switch (attack)
        {
            case 0:
                currentAttack = StartCoroutine(DoubleSpinShot());
                break;
            case 1:
                currentAttack = StartCoroutine(CarMechSummon());
                break;
            case 2:
                currentAttack = StartCoroutine(MechSummon());
                break;

        }

    }


    void ShootBullet(float direction)
    {
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y + hitbox.offset.y - 1f);
        Rigidbody2D currentBullet = Instantiate(bullet, spawnPos, Quaternion.identity).GetComponent<Rigidbody2D>();
        currentBullet.AddForceX(direction * dashFlySpeed * 5, ForceMode2D.Impulse);
        currentBullet.transform.localScale = new Vector2(-direction * currentBullet.transform.localScale.x, currentBullet.transform.localScale.y);
        SoundManager.Instance.PlaySound2D("LaserShoot");
    }

    IEnumerator CarMechSummon()
    {
        dashCounter++;

        SoundManager.Instance.PlaySound2D("MechanicRain");

        yield return new WaitForSeconds(0.4f);

        Vector3 spawnPos = new Vector3(BoundsLeft + 3, BoundsBottom + 2f);


        GameObject car = Instantiate(carOilLeak, spawnPos, Quaternion.identity);

        car.transform.localScale += Vector3.left * 2;

        spawnPos = new Vector3(BoundsRight - 3, BoundsBottom + 2f);

        Instantiate(carOilLeak, spawnPos, Quaternion.identity);

        spawnPos = new Vector3(BoundsCenterX, BoundsBottom + 2f);

        Instantiate(mechAirStrike, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(carMechDuration);

        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minions");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }
        
        yield return new WaitForSeconds(1f);
        minions = GameObject.FindGameObjectsWithTag("BossProjectile");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }
        minions = GameObject.FindGameObjectsWithTag("Warning");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }

        yield return new WaitForSeconds(1f);
        float amountOfFridges = Random.Range(minAmountOfFridges, maxAmountOfFridges + 1);

        float distance = BoundsRight - BoundsLeft;

        float distanceBetweenFridges = (distance - 1f) / amountOfFridges;


        for (int i = 0; i < amountOfFridges; i++)
        {
            Instantiate(fridge, new Vector3(BoundsLeft + 0.5f + (distanceBetweenFridges * i-1), BoundsTop - 2f, 0), Quaternion.identity);
        }

        yield return new WaitForSeconds(1.5f);

        minions = GameObject.FindGameObjectsWithTag("Minions");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }

        Invoke("ChooseAttack", attackCooldown);

    }

    IEnumerator MechSummon()
    {
        dashCounter++;
        Vector3 spawnPos = new Vector3(BoundsLeft + 3, BoundsTop - 1f);

        SoundManager.Instance.PlaySound2D("MechanicMech");

        yield return new WaitForSeconds(0.4f);

        Instantiate(mechFly, spawnPos, Quaternion.identity);

        spawnPos = new Vector3(BoundsRight - 3, BoundsTop - 1f);

        Instantiate(mechFly, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(mechDuration/2);

        spawnPos = new Vector3(BoundsLeft + 3, BoundsBottom + 2f);

        Instantiate(mechDash, spawnPos, Quaternion.identity);

        spawnPos = new Vector3(BoundsRight - 3, BoundsBottom + 2f);

        GameObject mech = Instantiate(mechDash, spawnPos, Quaternion.identity);
        mech.transform.localScale = new Vector3(-1, 1, 1);



        yield return new WaitForSeconds(mechDuration/2);

        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minions");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }

        yield return new WaitForSeconds(1.5f);

        spawnPos = new Vector3(BoundsRight - 3, BoundsTop - 1f);

        Instantiate(trafficMinion, spawnPos, Quaternion.identity);

        spawnPos = new Vector3(BoundsLeft + 3, BoundsTop - 1f);

        Instantiate(trafficMinion, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        minions = GameObject.FindGameObjectsWithTag("Minions");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }

        Invoke("ChooseAttack", attackCooldown);
    }

    IEnumerator DoubleSpinShot()
    {
        dashCounter = 0;
        Vector3 spawnPos = new Vector3(BoundsLeft + 3, BoundsBottom + 2f);

        SoundManager.Instance.PlaySound2D("MechanicSpin");

        yield return new WaitForSeconds(0.4f);

        Instantiate(mechSpin, spawnPos, Quaternion.identity);

        spawnPos = new Vector3(BoundsRight - 3, BoundsBottom + 2f);

        Instantiate(mechSpin, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(spinDuration);

        

        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minions");
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }
        yield return new WaitForSeconds(0.5f);

        ani.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        healthScript.ResetInvIncibility(false);
       

        ShootBullet(1f);
        yield return new WaitForSeconds(0.5f);
        ShootBullet(-1f);
        yield return new WaitForSeconds(1f);
        ShootBullet(1f);
        yield return new WaitForSeconds(0.5f);
        ShootBullet(-1f);
        yield return new WaitForSeconds(1f);
        ShootBullet(1f);
        yield return new WaitForSeconds(0.5f);
        ShootBullet(-1f);


        yield return new WaitForSeconds(dashDelay * 2);
        ResetToCenter();


    }

}
