using UnityEngine;
using UnityEngine.Audio;
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
    
    [SerializeField] PlayerCheats cheatsScript;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] AudioMixer audioMixer;

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


        Slider masterVolume = settingsPanel.Q<Slider>("MasterVolume");
        Slider musicVolume = settingsPanel.Q<Slider>("MusicVolume");
        Slider sfxVolume = settingsPanel.Q<Slider>("SFXVolume");

        float masterVol = 0;
        audioMixer.GetFloat("MasterVolume", out masterVol);
        masterVolume.value = masterVol + 80;
        masterVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("MasterVolume", evt.newValue - 80));
        
        float musicVol = 0;
        audioMixer.GetFloat("MusicVolume", out musicVol);
        musicVolume.value = musicVol + 80;
        musicVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("MusicVolume", evt.newValue - 80));

        float sfxVol = 0;
        audioMixer.GetFloat("SFXVolume", out sfxVol);
        sfxVolume.value = sfxVol + 80;
        sfxVolume.RegisterCallback<ChangeEvent<float>>(evt => audioMixer.SetFloat("SFXVolume", evt.newValue - 80));

        Toggle muteToggle = settingsPanel.Q<Toggle>("MuteToggle");
        muteToggle.value = AudioListener.pause;
        muteToggle.RegisterCallback<ChangeEvent<bool>>(evt => AudioListener.pause = evt.newValue);

        Toggle fullscreenToggle = settingsPanel.Q<Toggle>("FullscreenToggle");
        fullscreenToggle.value = Screen.fullScreen;
        fullscreenToggle.RegisterValueChangedCallback(evt => Screen.fullScreen = evt.newValue);

        

        Toggle godmodeToggle = settingsPanel.Q<Toggle>("GodmodeToggle");
        godmodeToggle.value = cheatsScript.GodModeBool();
        godmodeToggle.RegisterValueChangedCallback(evt => cheatsScript.GodMode(evt.newValue));

        Toggle noClipToggle = settingsPanel.Q<Toggle>("NoClipToggle");
        noClipToggle.value = cheatsScript.NoClipBool();
        noClipToggle.RegisterValueChangedCallback(evt => cheatsScript.NoClip(evt.newValue));

        float flySpeed = 0;
        Slider flySpeedSlider = settingsPanel.Q<Slider>("SpeedSlider");
        flySpeedSlider.value = flySpeed;
        flySpeedSlider.RegisterCallback<ChangeEvent<float>>(evt => cheatsScript.flySpeed = evt.newValue);

        Toggle levelSkipToggle = settingsPanel.Q<Toggle>("LevelSkipToggle");
        levelSkipToggle.value = cheatsScript.canLevelskip;
        levelSkipToggle.RegisterValueChangedCallback(evt => cheatsScript.canLevelskip = evt.newValue);

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
