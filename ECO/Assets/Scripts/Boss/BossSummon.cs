using System.Collections;
using UnityEngine;

public class BossSummon : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] float delay = 1f;
    [SerializeField] Transform spawnPos;
    [SerializeField] string bossTrack;
    [SerializeField] GameObject door;

    private void Start()
    {
        door.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MusicManager.Instance.PlayMusic(bossTrack);
            StartCoroutine(spawnBoss());
            
        }
    }

    IEnumerator spawnBoss()
    {
        Debug.Log("start delay");
        yield return new WaitForSeconds(delay);
        Debug.Log("start spawning");
        door.SetActive(true);

        Instantiate(boss, spawnPos.position, Quaternion.identity);
    }

}
