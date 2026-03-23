using System.Collections;
using UnityEngine;

public class BossSummon : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] float delay = 1f;
    [SerializeField] Transform spawnPos;
    [SerializeField] string bossTrack;
    [SerializeField] GameObject door;
    [SerializeField] Bossbar bossbar;

    bool hasSpawned = false;
    private void Start()
    {
        door.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MusicManager.Instance.PlayMusic(bossTrack);
            door.SetActive(true);
            StartCoroutine(spawnBoss());
            
        }
    }

    IEnumerator spawnBoss()
    {
        if (hasSpawned)
        {
            yield break;
        }
        hasSpawned = true;
        Debug.Log("start delay");
        yield return new WaitForSeconds(delay);
        Debug.Log("start spawning");
        Instantiate(boss, spawnPos.position, Quaternion.identity);
       
    }

}
