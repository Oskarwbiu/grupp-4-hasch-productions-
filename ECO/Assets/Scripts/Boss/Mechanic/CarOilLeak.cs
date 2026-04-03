using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CarOilLeak : MonoBehaviour
{
    [SerializeField] GameObject oilBlob;
    [SerializeField] AnimationCurve trajectory;
    [SerializeField] AnimationCurve axisCorrection;
    [SerializeField] AnimationCurve oilSpeed;
    [SerializeField] float damagePerBlob = 1f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileMaxHeight = 10f;
    [SerializeField] int amountProjectiles = 5;
    [SerializeField] float delayBetweenProjectiles = 1f;

    GameObject player;
    Animator ani;
    private void Start()
    {
        ani = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        StartCoroutine(OilLeak());
    }

    IEnumerator OilLeak()
    {
        ani.SetTrigger("Oil");
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenProjectiles);
            Vector2 playerPos = player.transform.position;
            GameObject currentBlob = Instantiate(oilBlob, transform.position, Quaternion.identity);
            currentBlob.GetComponent<OilScript>().InitializeProjectile(playerPos, projectileSpeed, trajectory, axisCorrection, oilSpeed, projectileMaxHeight, damagePerBlob);
        }
    }

}
