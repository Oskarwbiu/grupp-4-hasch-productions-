using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    private UIDocument pauseDocument;

    private VisualElement pauseVE;

    private Button resumeButton;
    private Button settingsButton;
    private Button mainMenuButton;

    private void Start()
    {
        

        pauseDocument = GetComponent<UIDocument>();
        pauseVE = pauseDocument.rootVisualElement as VisualElement;

        VisualElement root = pauseDocument.rootVisualElement;

        Button resumeButton = root.Q<Button>("Resume");
        Button settingsButton = root.Q<Button>("Settings");
        Button mainMenuButton = root.Q<Button>("MainMenu");


    }
    public void Pause()
    {

    }

    public void UnPause()
    {

    }
}
