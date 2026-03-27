using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField] private string musicName;
    void Start()
    {
        MusicManager.Instance.PlayMusic(musicName);
    }

    
}
