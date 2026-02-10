using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    private void Start()
    {
        MusicManager.Instance.PlayMusic("MainMenu");
    }

    
    void Update()
    {
        
    }
}
