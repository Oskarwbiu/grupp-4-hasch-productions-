using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MechAirStrike : MonoBehaviour
{
    [SerializeField] GameObject airstrike;
    [SerializeField] GameObject warning;

    GameObject player;
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        while (true)
        { 

            Rigidbody2D currentAirstrike = Instantiate(airstrike, transform.position + (Vector3.up * 2), Quaternion.identity).GetComponent<Rigidbody2D>();

            Vector2 force = new Vector2(0, 60f);

            currentAirstrike.AddForce(force, ForceMode2D.Impulse);
            yield return new WaitUntil(() => currentAirstrike.transform.position.y > transform.position.y + 30);
            currentAirstrike.linearVelocity = Vector2.zero;
            Vector2 PlayerPos = player.transform.position;
            currentAirstrike.position = new Vector2(Random.Range(PlayerPos.x - 4, PlayerPos.x + 4), currentAirstrike.position.y);


            RaycastHit2D ray = Physics2D.Raycast(currentAirstrike.position, Vector2.down, 999, 1 << 6);
            Debug.DrawRay(currentAirstrike.position, Vector2.down * 999);
            Instantiate(warning, ray.point, Quaternion.identity);
            currentAirstrike.transform.rotation = Quaternion.Euler(currentAirstrike.transform.rotation.x, currentAirstrike.transform.rotation.y, currentAirstrike.transform.rotation.z + 180f);
            yield return null;


        }
    }
}
