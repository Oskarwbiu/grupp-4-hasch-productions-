using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private UIDocument pauseDocument;

    private VisualElement pauseVE;

    private Button resumeButton;
    private Button settingsButton;
    private Button mainMenuButton;
    private Button exitButton;

    private VisualElement pauseMenu;
    private VisualElement settingsMenu;

    [SerializeField] PlayerInput playerInput;

    private void Awake()
    {


        pauseDocument = GetComponent<UIDocument>();
        pauseVE = pauseDocument.rootVisualElement as VisualElement;

        VisualElement root = pauseDocument.rootVisualElement;
        pauseMenu = root.Q<VisualElement>("PauseMenu");
        settingsMenu = root.Q<VisualElement>("SettingsMenu");
        

        VisualElement image = pauseMenu.Q<VisualElement>();
        VisualElement settingsRoot = settingsMenu.Q<VisualElement>();

        VisualElement settingsPanel = settingsRoot.Q<VisualElement>().Q<VisualElement>();

        resumeButton = image.Q<Button>("Resume");
        settingsButton = image.Q<Button>("Settings");
        mainMenuButton = image.Q<Button>("MainMenu");
        exitButton = settingsPanel.Q<Button>();

        resumeButton.RegisterCallback<ClickEvent>(evt => ResumeGame());
        settingsButton.RegisterCallback<ClickEvent>(evt => EnableSettings());
        mainMenuButton.RegisterCallback<ClickEvent>(evt => LoadMainMenu());
        exitButton.RegisterCallback<ClickEvent>(evt => ExitSettings());

    }

    private void Start()
    {
        pauseDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void Pause()
    {
        playerInput.enabled = false;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        playerInput.enabled = true;
        pauseDocument.rootVisualElement.style.display = DisplayStyle.None;
        Time.timeScale = 1;
        pauseVE.Blur();
    }

    void EnableSettings()
    {
        pauseMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
    }

    void ExitSettings()
    {
        pauseMenu.style.display = DisplayStyle.Flex;
        settingsMenu.style.display = DisplayStyle.None;
    }
    void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)Scene.MAINMENU);
        Time.timeScale = 1;
    }
}
