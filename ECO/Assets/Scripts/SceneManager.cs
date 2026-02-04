using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public enum Level
    {
        Intro,
        End
    }
    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
}
