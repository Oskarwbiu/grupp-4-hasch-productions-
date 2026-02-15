using UnityEngine;
using static System.TimeZoneInfo;

public class LevelManager : MonoBehaviour
{

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LoadScene(Level level)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)level);
    }
    
}
