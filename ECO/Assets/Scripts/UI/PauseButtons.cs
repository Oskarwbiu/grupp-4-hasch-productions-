using UnityEngine;
using UnityEngine.UIElements;

public class PauseButtons : MonoBehaviour
{
    [SerializeField] UIDocument _document;


    private void Start()
    {
        VisualElement root = _document.rootVisualElement;
        VisualElement pauseMenu = root.Q<VisualElement>("PauseMenu");

        Button resumeButton = root.Q<Button>("Resume");


        Button settingsButton = root.Q<Button>("Settings");


        Button mainMenuButton = root.Q<Button>("MainMenu");


    }
    
}
