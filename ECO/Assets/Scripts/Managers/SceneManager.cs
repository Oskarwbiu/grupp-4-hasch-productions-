using UnityEngine;
using static System.TimeZoneInfo;

public class SceneManager : MonoBehaviour
{

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LoadScene(Scene level)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)level);
    }
    
}
