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
    [SerializeField] bool exitLevelAfterKill = true;
    [SerializeField] float exitDelay;
    [SerializeField] Level level;

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
        GameObject spawnedBoss = Instantiate(boss, spawnPos.position, Quaternion.identity);
        if (exitLevelAfterKill)
        {
            StartCoroutine(ExitLevel(spawnedBoss));
        }
       
    }
    public void GoToNextLevel()
    {
        StopAllCoroutines();
        StartCoroutine(ExitLevel(null));
    }

    IEnumerator ExitLevel(GameObject spawnedBoss)
    {
        yield return new WaitUntil(() => spawnedBoss == null);
        Debug.Log("Boss defeated, starting exit delay");
        yield return new WaitForSeconds(exitDelay);
        Debug.Log("Loading next level");
        FindFirstObjectByType<LevelExit>().StartLevelCoroutine(level);
    }

}
