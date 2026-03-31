using System.Collections;
using UnityEngine;

public class MechSpin : MonoBehaviour
{
    [SerializeField] float spinShotInterval = 0.1f;
    [SerializeField] float spinShotForce = 15f;
    [SerializeField] GameObject spinShotPrefab;

    void Start()
    {
        StartCoroutine(Spin());
    }

    
    IEnumerator Spin()
    {
        yield return new WaitForSeconds(spinShotInterval);
        float direction = 1;
        Vector2 spawnPos = Vector2.down;
        while (true) 
        {
            
           transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            
            Rigidbody2D currentBullet = Instantiate(spinShotPrefab, (Vector2)transform.position + spawnPos, Quaternion.identity).GetComponent<Rigidbody2D>();
            currentBullet.AddForceX(direction * spinShotForce, ForceMode2D.Impulse);
            currentBullet.transform.localScale = new Vector2(-currentBullet.transform.localScale.x, currentBullet.transform.localScale.y);
            SoundManager.Instance.PlaySound2D("LaserShoot");

            yield return new WaitForSeconds(spinShotInterval);
            direction = -direction;
           
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            

            currentBullet = Instantiate(spinShotPrefab, (Vector2)transform.position + spawnPos, Quaternion.identity).GetComponent<Rigidbody2D>();
            currentBullet.AddForceX(direction * spinShotForce, ForceMode2D.Impulse);
            SoundManager.Instance.PlaySound2D("LaserShoot");

            direction = -direction;
            
            yield return new WaitForSeconds(spinShotInterval);

        }
    }


}
