using UnityEditor.SearchService;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] Scene level;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene((int)level);
        }
    }
}
