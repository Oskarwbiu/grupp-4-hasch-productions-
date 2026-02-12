using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private UIDocument _document;
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Backrooms");

        VisualElement root = _document.rootVisualElement;

        Button quitButton = root.Q<Button>("QuitButton");
        quitButton.RegisterCallback<ClickEvent>(evt => Application.Quit());

        Button startButton = root.Q<Button>("StartButton");
        startButton.RegisterCallback<ClickEvent>(evt => SceneManager.LoadScene(1));
        startButton.RegisterCallback<ClickEvent>(evt => FindFirstObjectByType<HUDScript>().StartGame());
        startButton.RegisterCallback<ClickEvent>(evt => _document.enabled = false);
    }

    
    void Update()
    {
        
    }
}
